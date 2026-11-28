using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using RWASFilterLib;
using RWAutoSell;
using UnityEngine;
using Verse;

namespace AutoSellerPlusByLilly;

public sealed class AutoSellerPlusMod : Mod
{
    public AutoSellerPlusMod(ModContentPack content) : base(content)
    {
        new Harmony("lilly.autosellerplus").PatchAll(typeof(AutoSellerPlusMod).Assembly);
    }
}

[HarmonyPatch(typeof(MainTabWindow_AutoSell), nameof(MainTabWindow_AutoSell.TopButtonBar))]
internal static class AutoSellerPlusButtonPatch
{
    private static readonly FieldInfo MainListField =
        typeof(MainTabWindow_AutoSell).GetField("MainList", BindingFlags.Instance | BindingFlags.NonPublic)
        ?? throw new MissingFieldException(typeof(MainTabWindow_AutoSell).FullName, "MainList");

    private static readonly PropertyInfo RootCompProperty =
        typeof(ASMainTabList).GetProperty("RootComp", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        ?? throw new MissingMemberException(typeof(ASMainTabList).FullName, "RootComp");

    [HarmonyPostfix]
    private static void Postfix(MainTabWindow_AutoSell __instance, Rect inRect)
    {
        if (MainListField.GetValue(__instance) is not { } mainList ||
            RootCompProperty.GetValue(mainList) is not IRuleComp comp)
        {
            return;
        }

        const float buttonWidth = 120f;
        const float gap = 8f;
        float buttonHeight = MainTabWindow_AutoSell.ButtonSize.y;
        float firstX = inRect.x + 10f + MainTabWindow_AutoSell.ButtonSize.x + 10f;
        var saveRect = new Rect(firstX, inRect.y, buttonWidth, buttonHeight);
        var loadRect = new Rect(firstX + buttonWidth + gap, inRect.y, buttonWidth, buttonHeight);

        if (Widgets.ButtonText(saveRect, Translator.Translate("AutoSellerPlus_SaveRules")))
        {
            Find.WindowStack.Add(new RulePresetSaveDialog(comp));
        }

        if (Widgets.ButtonText(loadRect, Translator.Translate("AutoSellerPlus_LoadRules")))
        {
            try
            {
                Find.WindowStack.Add(new RulePresetLoadDialog(comp));
            }
            catch (Exception exception)
            {
                Log.Error($"Auto Seller Plus could not open the preset list: {exception}");
                Messages.Message(
                    Translator.Translate("AutoSellerPlus_LoadFailed").ToString(),
                    MessageTypeDefOf.RejectInput);
            }
        }
    }
}

public sealed class RulePreset : IExposable
{
    public List<ASRule> Rules = new();

    public void ExposeData()
    {
        Scribe_Collections.Look(ref Rules, "rules", false, LookMode.Deep);
    }
}

internal static class RulePresetStorage
{
    private const string PresetFolderName = "Auto Seller Plus By Lilly";
    private const string DocumentRoot = "AutoSellerPlusPreset";

    public static string PresetDirectory =>
        Path.Combine(GenFilePaths.ConfigFolderPath, PresetFolderName, "Presets");

    public static FileInfo[] GetPresetFiles()
    {
        Directory.CreateDirectory(PresetDirectory);
        return new DirectoryInfo(PresetDirectory)
            .GetFiles("*.xml", SearchOption.TopDirectoryOnly)
            .OrderByDescending(file => file.LastWriteTimeUtc)
            .ThenBy(file => file.Name, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    public static bool TryGetPresetPath(string rawName, out string presetName, out string path)
    {
        presetName = rawName.Trim();
        if (presetName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
        {
            presetName = presetName[..^4];
        }

        path = string.Empty;
        if (string.IsNullOrWhiteSpace(presetName) ||
            presetName is "." or ".." ||
            presetName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 ||
            presetName.Contains('/') ||
            presetName.Contains('\\'))
        {
            return false;
        }

        path = Path.Combine(PresetDirectory, presetName + ".xml");
        return true;
    }

    public static bool Save(string path, IRuleComp comp, string displayName)
    {
        string temporaryPath = path + ".tmp";
        bool scribeStarted = false;

        try
        {
            var preset = new RulePreset();
            for (int index = 0; index < comp.Count(); index++)
            {
                if (comp.GetRuleAt(index) is not ASRule rule)
                {
                    Messages.Message(
                        Translator.Translate("AutoSellerPlus_InvalidRule").ToString(),
                        MessageTypeDefOf.RejectInput);
                    return false;
                }

                preset.Rules.Add(rule);
            }

            Directory.CreateDirectory(PresetDirectory);
            Scribe.saver.InitSaving(temporaryPath, DocumentRoot);
            scribeStarted = true;
            Scribe_Deep.Look(ref preset, "preset");
            Scribe.saver.FinalizeSaving();
            scribeStarted = false;

            File.Move(temporaryPath, path, true);
            Messages.Message(
                string.Format(Translator.Translate("AutoSellerPlus_Saved").ToString(), displayName),
                MessageTypeDefOf.PositiveEvent);
            return true;
        }
        catch (Exception exception)
        {
            if (scribeStarted)
            {
                Scribe.ForceStop();
            }

            TryDeleteTemporaryFile(temporaryPath);
            Log.Error($"Auto Seller Plus could not save preset '{displayName}': {exception}");
            Messages.Message(
                Translator.Translate("AutoSellerPlus_SaveFailed").ToString(),
                MessageTypeDefOf.RejectInput);
            return false;
        }
    }

    public static bool Load(string path, IRuleComp comp, string displayName, bool overwriteExistingRules)
    {
        bool scribeStarted = false;
        try
        {
            RulePreset? preset = null;
            Scribe.loader.InitLoading(path);
            scribeStarted = true;
            Scribe_Deep.Look(ref preset, "preset");
            Scribe.loader.FinalizeLoading();
            scribeStarted = false;

            if (preset?.Rules is null)
            {
                throw new InvalidDataException("Preset did not contain a rule list.");
            }

            if (preset.Rules.Any(rule => rule is null))
            {
                throw new InvalidDataException("Preset contains a null rule.");
            }

            var originalRules = new List<ASRule>();
            int originalCount = comp.Count();
            for (int index = 0; index < originalCount; index++)
            {
                if (comp.GetRuleAt(index) is not ASRule existingRule)
                {
                    throw new InvalidDataException("The current rule list contains an incompatible rule.");
                }

                originalRules.Add(existingRule);
            }

            try
            {
                if (overwriteExistingRules)
                {
                    for (int index = comp.Count() - 1; index >= 0; index--)
                    {
                        comp.RemoveAt(index);
                    }
                }

                foreach (ASRule rule in preset.Rules)
                {
                    comp.Add(rule);
                }
            }
            catch
            {
                if (overwriteExistingRules)
                {
                    for (int index = comp.Count() - 1; index >= 0; index--)
                    {
                        comp.RemoveAt(index);
                    }

                    foreach (ASRule originalRule in originalRules)
                    {
                        comp.Add(originalRule);
                    }
                }
                else
                {
                    for (int index = comp.Count() - 1; index >= originalCount; index--)
                    {
                        comp.RemoveAt(index);
                    }
                }

                throw;
            }

            ASLibMod.GuiRefresh = true;
            Messages.Message(
                string.Format(
                    Translator.Translate(overwriteExistingRules
                        ? "AutoSellerPlus_LoadedOverwrite"
                        : "AutoSellerPlus_Loaded").ToString(),
                    displayName),
                MessageTypeDefOf.PositiveEvent);
            return true;
        }
        catch (Exception exception)
        {
            if (scribeStarted)
            {
                Scribe.ForceStop();
            }

            Log.Error($"Auto Seller Plus could not load preset '{displayName}': {exception}");
            Messages.Message(
                Translator.Translate("AutoSellerPlus_LoadFailed").ToString(),
                MessageTypeDefOf.RejectInput);
            return false;
        }
    }

    private static void TryDeleteTemporaryFile(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (IOException exception)
        {
            Log.Error($"Auto Seller Plus could not remove temporary preset file '{path}': {exception}");
        }
        catch (UnauthorizedAccessException exception)
        {
            Log.Error($"Auto Seller Plus could not remove temporary preset file '{path}': {exception}");
        }
    }
}

internal sealed class RulePresetSaveDialog : Window
{
    private readonly IRuleComp comp;
    private string presetName = string.Empty;
    private FileInfo[] presetFiles;
    private Vector2 scrollPosition;
    private bool focusNameOnNextDraw = true;

    public RulePresetSaveDialog(IRuleComp comp)
    {
        this.comp = comp;
        presetFiles = RulePresetStorage.GetPresetFiles();
        forcePause = false;
        absorbInputAroundWindow = true;
        doCloseX = true;
    }

    public override Vector2 InitialSize => new(680f, 560f);

    public override void DoWindowContents(Rect inRect)
    {
        Widgets.Label(
            new Rect(0f, 0f, inRect.width, 28f),
            Translator.Translate("AutoSellerPlus_PresetName"));

        const string presetNameControl = "AutoSellerPlusPresetName";
        GUI.SetNextControlName(presetNameControl);
        presetName = Widgets.TextField(new Rect(0f, 32f, inRect.width, 34f), presetName);
        if (focusNameOnNextDraw)
        {
            GUI.FocusControl(presetNameControl);
            focusNameOnNextDraw = false;
        }

        Widgets.Label(
            new Rect(0f, 72f, inRect.width, 28f),
            Translator.Translate("AutoSellerPlus_SavedPresets"));

        var listRect = new Rect(0f, 102f, inRect.width, inRect.height - 150f);
        if (presetFiles.Length == 0)
        {
            Widgets.Label(
                new Rect(0f, 108f, inRect.width, 30f),
                Translator.Translate("AutoSellerPlus_NoPresets"));
        }
        else
        {
            const float rowHeight = 42f;
            var viewRect = new Rect(0f, 0f, listRect.width - 16f, presetFiles.Length * rowHeight);
            Widgets.BeginScrollView(listRect, ref scrollPosition, viewRect);

            for (int index = 0; index < presetFiles.Length; index++)
            {
                FileInfo file = presetFiles[index];
                string displayName = Path.GetFileNameWithoutExtension(file.Name);
                var rowRect = new Rect(0f, index * rowHeight, viewRect.width, rowHeight - 4f);
                float deleteX = rowRect.width - 76f;
                float overwriteX = deleteX - 94f;
                float dateX = overwriteX - 142f;

                Widgets.Label(
                    new Rect(8f, rowRect.y, dateX - 12f, rowRect.height),
                    displayName);
                Widgets.Label(
                    new Rect(dateX, rowRect.y, 136f, rowRect.height),
                    file.LastWriteTime.ToString("yyyy-MM-dd HH:mm"));

                if (Widgets.ButtonText(
                        new Rect(overwriteX, rowRect.y, 86f, rowRect.height),
                        Translator.Translate("AutoSellerPlus_Overwrite")))
                {
                    string presetFilePath = file.FullName;
                    string confirmation = string.Format(
                        Translator.Translate("AutoSellerPlus_OverwriteConfirm").ToString(),
                        displayName);
                    Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation(
                        confirmation,
                        () =>
                        {
                            if (RulePresetStorage.Save(presetFilePath, comp, displayName))
                            {
                                RefreshPresetFiles();
                            }
                        },
                        true,
                        Translator.Translate("AutoSellerPlus_OverwriteTitle").ToString()));
                }

                if (Widgets.ButtonText(
                        new Rect(deleteX, rowRect.y, 70f, rowRect.height),
                        Translator.Translate("AutoSellerPlus_Delete")))
                {
                    string presetFilePath = file.FullName;
                    string confirmation = string.Format(
                        Translator.Translate("AutoSellerPlus_DeleteConfirm").ToString(),
                        displayName);
                    Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation(
                        confirmation,
                        () => DeletePreset(presetFilePath, displayName),
                        true,
                        Translator.Translate("AutoSellerPlus_DeleteTitle").ToString()));
                }
            }

            Widgets.EndScrollView();
        }

        float buttonWidth = 120f;
        float buttonY = inRect.height - 40f;
        if (Widgets.ButtonText(
                new Rect(inRect.width - buttonWidth * 2f - 10f, buttonY, buttonWidth, 35f),
                Translator.Translate("AutoSellerPlus_Cancel")))
        {
            Close();
        }

        if (!Widgets.ButtonText(
                new Rect(inRect.width - buttonWidth, buttonY, buttonWidth, 35f),
                Translator.Translate("AutoSellerPlus_Save")))
        {
            return;
        }

        if (!RulePresetStorage.TryGetPresetPath(presetName, out string normalizedName, out string path))
        {
            Messages.Message(
                Translator.Translate("AutoSellerPlus_InvalidName").ToString(),
                MessageTypeDefOf.RejectInput);
            return;
        }

        if (File.Exists(path))
        {
            string confirmation = string.Format(
                Translator.Translate("AutoSellerPlus_OverwriteConfirm").ToString(),
                normalizedName);
            Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation(
                confirmation,
                () =>
                {
                    if (RulePresetStorage.Save(path, comp, normalizedName))
                    {
                        RefreshPresetFiles();
                    }
                },
                true,
                Translator.Translate("AutoSellerPlus_OverwriteTitle").ToString()));
            return;
        }

        if (RulePresetStorage.Save(path, comp, normalizedName))
        {
            RefreshPresetFiles();
            presetName = string.Empty;
            GUI.FocusControl(presetNameControl);
        }
    }

    private void RefreshPresetFiles()
    {
        presetFiles = RulePresetStorage.GetPresetFiles();
        scrollPosition = Vector2.zero;
    }

    private void DeletePreset(string path, string displayName)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            RefreshPresetFiles();
        }
        catch (Exception exception)
        {
            Log.Error($"Auto Seller Plus could not delete preset '{displayName}': {exception}");
            Messages.Message(
                Translator.Translate("AutoSellerPlus_DeleteFailed").ToString(),
                MessageTypeDefOf.RejectInput);
        }
    }
}

internal sealed class RulePresetLoadDialog : Window
{
    private readonly IRuleComp comp;
    private readonly FileInfo[] presetFiles;
    private Vector2 scrollPosition;
    private bool overwriteExistingRules = true;

    public RulePresetLoadDialog(IRuleComp comp)
    {
        this.comp = comp;
        presetFiles = RulePresetStorage.GetPresetFiles();
        forcePause = false;
        absorbInputAroundWindow = true;
        doCloseX = true;
    }

    public override Vector2 InitialSize => new(520f, 500f);

    public override void DoWindowContents(Rect inRect)
    {
        Widgets.Label(
            new Rect(0f, 0f, inRect.width, 30f),
            Translator.Translate("AutoSellerPlus_LoadDialogTitle"));

        Widgets.CheckboxLabeled(
            new Rect(0f, 34f, inRect.width, 30f),
            Translator.Translate("AutoSellerPlus_OverwriteExistingRules").ToString(),
            ref overwriteExistingRules);

        if (presetFiles.Length == 0)
        {
            Widgets.Label(
                new Rect(0f, 72f, inRect.width, 30f),
                Translator.Translate("AutoSellerPlus_NoPresets"));
            return;
        }

        var listRect = new Rect(0f, 70f, inRect.width, inRect.height - 70f);
        float rowHeight = 38f;
        var viewRect = new Rect(0f, 0f, listRect.width - 16f, presetFiles.Length * rowHeight);
        Widgets.BeginScrollView(listRect, ref scrollPosition, viewRect);

        for (int index = 0; index < presetFiles.Length; index++)
        {
            FileInfo file = presetFiles[index];
            var rowRect = new Rect(0f, index * rowHeight, viewRect.width, rowHeight - 4f);
            if (Widgets.ButtonText(rowRect, Path.GetFileNameWithoutExtension(file.Name)) &&
                RulePresetStorage.Load(
                    file.FullName,
                    comp,
                    Path.GetFileNameWithoutExtension(file.Name),
                    overwriteExistingRules))
            {
                Close();
                break;
            }
        }

        Widgets.EndScrollView();
    }
}

[HarmonyPatch(typeof(RuleRowUI), nameof(RuleRowUI.DrawRow))]
internal static class AutoSellerPlusRuleRowLabelPatch
{
    private static readonly MethodInfo ObjectToString =
        typeof(object).GetMethod(nameof(ToString), Type.EmptyTypes)
        ?? throw new MissingMethodException(typeof(object).FullName, nameof(ToString));

    private static readonly MethodInfo FormatRuleLabel =
        typeof(AutoSellerPlusRuleRowLabelPatch).GetMethod(
            nameof(GetRuleDisplayLabel),
            BindingFlags.Static | BindingFlags.NonPublic)
        ?? throw new MissingMethodException(
            typeof(AutoSellerPlusRuleRowLabelPatch).FullName,
            nameof(GetRuleDisplayLabel));

    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (CodeInstruction instruction in instructions)
        {
            if (instruction.opcode == OpCodes.Callvirt &&
                instruction.operand is MethodInfo method &&
                method == ObjectToString)
            {
                instruction.opcode = OpCodes.Call;
                instruction.operand = FormatRuleLabel;
            }

            yield return instruction;
        }
    }

    private static string GetRuleDisplayLabel(object rule)
    {
        if (rule is ASRule autoSellRule)
        {
            return $"{rule} ( {autoSellRule.SellCount} / {autoSellRule.BuyPoint} )";
        }

        return rule?.ToString() ?? string.Empty;
    }
}

