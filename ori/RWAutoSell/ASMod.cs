using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace RWAutoSell
{
	// Token: 0x02000015 RID: 21
	[NullableContext(1)]
	[Nullable(0)]
	public class ASMod : Mod
	{
		// Token: 0x060000A8 RID: 168 RVA: 0x00007414 File Offset: 0x00005614
		public ASMod(ModContentPack content) : base(content)
		{
			int num = Assembly.GetExecutingAssembly().GetName().Version.Major * 1000;
			num += Assembly.GetExecutingAssembly().GetName().Version.Minor * 100;
			num += Assembly.GetExecutingAssembly().GetName().Version.Build * 10;
			num += Assembly.GetExecutingAssembly().GetName().Version.Revision;
			this.CurrentBuild = num;
			Harmony harmony = new Harmony("com.SupesSolutions.TradeUnMergerPatcher");
			MethodInfo method = typeof(MergePatcher).GetMethod("MergeAllowedTradeableMatching", BindingFlags.Static | BindingFlags.Public);
			MethodInfo method2 = typeof(TransferableUtility).GetMethod("TransferAsOne", BindingFlags.Static | BindingFlags.Public);
			harmony.Patch(method2, null, new HarmonyMethod(method), null, null);
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x000074E8 File Offset: 0x000056E8
		public static ASModSettings GetSettings()
		{
			return ASMod.GetSingleton.GetSettings<ASModSettings>();
		}

		// Token: 0x060000AA RID: 170 RVA: 0x000074F4 File Offset: 0x000056F4
		public override string SettingsCategory()
		{
			if (Prefs.DevMode)
			{
				return "AutoSeller Expert Settings";
			}
			return base.SettingsCategory();
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x060000AB RID: 171 RVA: 0x00007509 File Offset: 0x00005709
		public static ASMod GetSingleton
		{
			get
			{
				return LoadedModManager.GetMod<ASMod>();
			}
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00007510 File Offset: 0x00005710
		public override void DoSettingsWindowContents(Rect inRect)
		{
			Text.Font = GameFont.Medium;
			Widgets.Label(new Rect(20f, 60f, inRect.width - 60f, 100f), "Don't change things here unless you know what you're doing or have been instructed to do so.  also, any changes made here require the game to be restarted");
			Text.Font = GameFont.Small;
			Widgets.Label(new Rect(20f, 160f, inRect.width - 60f, 28f), "Dialog Hooks:");
			if (Prefs.DevMode)
			{
				Widgets.Label(new Rect(inRect.width / 3f * 2f, 160f, inRect.width - 60f - inRect.width / 3f, 28f), "Dev Flags:");
			}
			Widgets.DrawLineHorizontal(20f, 190f, inRect.width - 60f);
			Dictionary<string, bool> whiteList = ASMod.GetSettings().WhiteList;
			int num = 0;
			foreach (string text in whiteList.Keys.ToList<string>())
			{
				Rect rect = new Rect(20f, (float)num * 30f + 195f, inRect.width / 3f, 28f);
				if (num % 2 == 0)
				{
					Widgets.DrawHighlight(rect);
				}
				bool value = whiteList[text];
				Widgets.CheckboxLabeled(rect, text, ref value, false, null, null, false, false);
				whiteList[text] = value;
				num++;
			}
			if (Prefs.DevMode)
			{
				Widgets.CheckboxLabeled(new Rect(inRect.width / 3f * 2f, (float)num * 30f + 195f, inRect.width / 3f, 28f), "Debug: Show Report during patch", ref ASMod.GetSettings().devlettershow, false, null, null, false, false);
				Widgets.CheckboxLabeled(new Rect(inRect.width / 3f * 2f, (float)num * 30f + 223f, inRect.width / 3f, 28f), "Debug: Show Non-Standard Test Buttons", ref ASMod.GetSettings().devTestAdditional, false, null, null, false, false);
				Widgets.CheckboxLabeled(new Rect(inRect.width / 3f * 2f, (float)num * 30f + 251f, inRect.width / 3f, 28f), "Debug: Log AI Trade", ref ASMod.GetSettings().devAILog, false, null, null, false, false);
				Widgets.CheckboxLabeled(new Rect(inRect.width / 3f * 2f, (float)num * 30f + 279f, inRect.width / 3f, 28f), "Debug: AI Trade Ka-ching Sound", ref ASMod.GetSettings().Kaching, false, null, null, false, false);
			}
			base.DoSettingsWindowContents(inRect);
		}

		// Token: 0x060000AD RID: 173 RVA: 0x000077EC File Offset: 0x000059EC
		public override void WriteSettings()
		{
			base.WriteSettings();
		}

		// Token: 0x060000AE RID: 174 RVA: 0x000077F4 File Offset: 0x000059F4
		public void KnownModbinds(Assembly a, Dictionary<string, bool> whitelist)
		{
			if (a.GetName().Name == "DynamicTradeInterface" && ModLister.GetActiveModWithIdentifier("zeracronius.dynamictradeinterface", false) != null)
			{
				Type type = a.GetType("DynamicTradeInterface.UserInterface.Window_DynamicTrade");
				bool whitelisted;
				if (!whitelist.TryGetValue(type.FullName, out whitelisted))
				{
					whitelist.Add(type.FullName, false);
				}
				WindowFieldContainer windowFieldContainer = new WindowFieldContainer(type, null, whitelisted);
				windowFieldContainer.knownmod = true;
				windowFieldContainer.dofill = false;
				windowFieldContainer.dowindowcontent = true;
				windowFieldContainer.dopostopen = false;
				ASMod.tpl.Add(windowFieldContainer);
			}
		}

		// Token: 0x060000AF RID: 175 RVA: 0x00007880 File Offset: 0x00005A80
		public void BindWindows()
		{
			if (!this.loaded)
			{
				Dictionary<string, bool> whiteList = ASMod.GetSettings().WhiteList;
				int num = whiteList.Count<KeyValuePair<string, bool>>();
				ASMod.tpl.Clear();
				foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
				{
					List<string> list = new List<string>();
					try
					{
						foreach (Type type in from tp in assembly.GetExportedTypes()
						where !tp.IsAbstract && !tp.IsInterface && typeof(Window).IsAssignableFrom(tp)
						select tp)
						{
							list.Add(type.FullName);
							foreach (FieldInfo tradableField in type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where((FieldInfo el) => el.FieldType == typeof(List<Tradeable>)))
							{
								bool whitelisted;
								if (!whiteList.TryGetValue(type.FullName, out whitelisted))
								{
									whiteList.Add(type.FullName, false);
								}
								WindowFieldContainer item = new WindowFieldContainer(type, tradableField, whitelisted);
								ASMod.tpl.Add(item);
							}
						}
						this.KnownModbinds(assembly, whiteList);
					}
					catch
					{
					}
				}
				int build = ASMod.GetSettings().Build;
				if (build != this.CurrentBuild)
				{
					if (build < 2020)
					{
						if (ASMod.GetSettings().WhiteList.ContainsKey("RimWorld.Dialog_Trade"))
						{
							ASMod.GetSettings().WhiteList["RimWorld.Dialog_Trade"] = true;
						}
						if (ASMod.GetSettings().WhiteList.ContainsKey("Glitterworld_Prime.DialogPrimeTrade"))
						{
							ASMod.GetSettings().WhiteList["Glitterworld_Prime.DialogPrimeTrade"] = true;
						}
					}
					if (build < 2048 && ASMod.GetSettings().WhiteList.ContainsKey("DynamicTradeInterface.UserInterface.Window_DynamicTrade"))
					{
						ASMod.GetSettings().WhiteList["DynamicTradeInterface.UserInterface.Window_DynamicTrade"] = true;
					}
					ASMod.GetSettings().Build = this.CurrentBuild;
				}
				if (num != whiteList.Count<KeyValuePair<string, bool>>() || build != this.CurrentBuild)
				{
					this.WriteSettings();
				}
				this.PatchWhitelist();
			}
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x00007B0C File Offset: 0x00005D0C
		private void PatchWhitelist()
		{
			Harmony harmony = new Harmony("com.SupesSolutions.TradeWindowPatcher");
			MethodInfo method = typeof(DialogPatcher).GetMethod("PostOpenTrade");
			MethodInfo method2 = typeof(DialogPatcher).GetMethod("PostDoWindowContents");
			MethodInfo method3 = typeof(DialogPatcher).GetMethod("Transpiler");
			MethodInfo method4 = typeof(DialogPatcher).GetMethod("DWCTranspiler");
			foreach (WindowFieldContainer windowFieldContainer in ASMod.tpl)
			{
				if (windowFieldContainer.WhiteListed)
				{
					if (windowFieldContainer.dopostopen)
					{
						MethodInfo method5 = windowFieldContainer.WindowType.GetMethod("PostOpen", BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
						harmony.Patch(method5, null, new HarmonyMethod(method), null, null);
					}
					if (windowFieldContainer.dowindowcontent)
					{
						MethodInfo method6 = windowFieldContainer.WindowType.GetMethod("DoWindowContents");
						if (method6 != null)
						{
							if (windowFieldContainer.knownmod)
							{
								if (windowFieldContainer.WindowType.FullName == "DynamicTradeInterface.UserInterface.Window_DynamicTrade")
								{
									harmony.Patch(method6, null, new HarmonyMethod(method2), null, null);
								}
							}
							else
							{
								harmony.Patch(method6, null, new HarmonyMethod(method2), new HarmonyMethod(method4), null);
							}
						}
					}
					if (windowFieldContainer.dofill)
					{
						MethodInfo method7 = windowFieldContainer.WindowType.GetMethod("FillMainRect", BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
						if (method7 != null)
						{
							harmony.Patch(method7, null, null, new HarmonyMethod(method3), null);
						}
					}
				}
			}
			this.loaded = true;
		}

		// Token: 0x04000071 RID: 113
		private readonly int CurrentBuild;

		// Token: 0x04000072 RID: 114
		public bool loaded;

		// Token: 0x04000073 RID: 115
		public List<ASMapComp> mapComps = new List<ASMapComp>();

		// Token: 0x04000074 RID: 116
		public static List<WindowFieldContainer> tpl = new List<WindowFieldContainer>();
	}
}
