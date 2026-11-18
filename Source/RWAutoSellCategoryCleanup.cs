using System.Collections;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace AutoSellerPlusByLilly;

/// <summary>
/// Removes stale ThingCategoryDef references from RWAutoSell's category-selection
/// collections after save data has been loaded. This deliberately does not touch
/// ASRule/filter collections or Auto Seller Plus rule presets.
/// </summary>
[HarmonyPatch]
internal static class RWAutoSellCategoryCleanupPatch
{
    private const string TargetAssemblyName = "RWAutoSell";
    private static readonly BindingFlags AllInstanceFields =
        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    private static readonly BindingFlags AllStaticFields =
        BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

    private static IEnumerable<MethodBase> TargetMethods()
    {
        Assembly? assembly = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(candidate =>
                string.Equals(candidate.GetName().Name, TargetAssemblyName, StringComparison.Ordinal));

        if (assembly is null)
        {
            yield break;
        }

        foreach (Type type in GetLoadableTypes(assembly))
        {
            if (type.Name is "ASGameComp" or "ASModSettings" or "ASLibModSettings")
            {
                MethodInfo? exposeData = type.GetMethod(
                    "ExposeData",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (exposeData is not null)
                {
                    yield return exposeData;
                }
            }

            // RWAutoSell builds its custom category picker through this list-box type.
            // Patch its list-building/selection methods so stale entries are removed
            // before the picker displays them.
            if (type.Name == "ASCatListBox")
            {
                foreach (MethodInfo method in type.GetMethods(
                             BindingFlags.Instance |
                             BindingFlags.Public | BindingFlags.NonPublic |
                             BindingFlags.DeclaredOnly))
                {
                    string name = method.Name;
                    if (name.Contains("List", StringComparison.OrdinalIgnoreCase) ||
                        name.Contains("Def", StringComparison.OrdinalIgnoreCase) ||
                        name.Contains("Draw", StringComparison.OrdinalIgnoreCase) ||
                        name.Contains("Click", StringComparison.OrdinalIgnoreCase))
                    {
                        yield return method;
                    }
                }
            }
        }
    }

    private static void Prefix(object? __instance, MethodBase __originalMethod)
    {
        if (__originalMethod.Name == "ExposeData")
        {
            return;
        }

        Cleanup(__originalMethod.DeclaringType, __instance);
    }

    private static void Postfix(object? __instance, MethodBase __originalMethod)
    {
        if (__originalMethod.Name != "ExposeData" || Scribe.mode != LoadSaveMode.PostLoadInit)
        {
            return;
        }

        Cleanup(__originalMethod.DeclaringType, __instance);
    }

    private static void Cleanup(Type? rootType, object? rootInstance)
    {
        if (rootType is null)
        {
            return;
        }

        int removed = 0;
        var visited = new HashSet<object>(ReferenceEqualityComparer.Instance);

        if (rootInstance is not null)
        {
            ScanObject(rootInstance, rootType, 0, visited, ref removed);
        }

        // Some versions keep the category-picker source list in static fields.
        if (rootType.Name == "ASCatListBox")
        {
            ScanStaticFields(rootType, visited, ref removed);
        }

        if (removed > 0)
        {
            Log.Message($"[Auto Seller Plus] Removed {removed} stale RWAutoSell category entr{(removed == 1 ? "y" : "ies")}.");
        }
    }

    private static void ScanObject(
        object instance,
        Type type,
        int depth,
        HashSet<object> visited,
        ref int removed)
    {
        if (depth > 4 || !visited.Add(instance))
        {
            return;
        }

        for (Type? current = type; current is not null; current = current.BaseType)
        {
            foreach (FieldInfo field in current.GetFields(AllInstanceFields | BindingFlags.DeclaredOnly))
            {
                object? value;
                try
                {
                    value = field.GetValue(instance);
                }
                catch
                {
                    continue;
                }

                if (value is null)
                {
                    continue;
                }

                if (value is IList list)
                {
                    removed += CleanCategoryList(field.Name, field.FieldType, list);
                    continue;
                }

                // Traverse only RWAutoSell-owned helper objects. This avoids walking
                // the game's Def database, maps, windows, or unrelated mod objects.
                if (depth < 2 &&
                    value.GetType().Name == "ASCatListBox" &&
                    value.GetType().Assembly.GetName().Name == TargetAssemblyName)
                {
                    ScanObject(value, value.GetType(), depth + 1, visited, ref removed);
                }
            }
        }
    }

    private static void ScanStaticFields(Type type, HashSet<object> visited, ref int removed)
    {
        for (Type? current = type; current is not null; current = current.BaseType)
        {
            foreach (FieldInfo field in current.GetFields(AllStaticFields | BindingFlags.DeclaredOnly))
            {
                object? value;
                try
                {
                    value = field.GetValue(null);
                }
                catch
                {
                    continue;
                }

                if (value is IList list)
                {
                    removed += CleanCategoryList(field.Name, field.FieldType, list);
                }
            }
        }
    }

    private static int CleanCategoryList(string fieldName, Type fieldType, IList list)
    {
        Type? elementType = GetListElementType(fieldType);
        bool categoryFieldName =
            fieldName.Contains("Cat", StringComparison.OrdinalIgnoreCase) ||
            fieldName.Contains("DefList", StringComparison.OrdinalIgnoreCase) ||
            fieldName.Equals("selecteddef", StringComparison.OrdinalIgnoreCase) ||
            fieldName.Equals("selecteddefs", StringComparison.OrdinalIgnoreCase);

        bool containsCategoryDefs = categoryFieldName &&
                                    elementType is not null &&
                                    typeof(ThingCategoryDef).IsAssignableFrom(elementType);
        bool containsDefNames = categoryFieldName && elementType == typeof(string);

        if (!containsCategoryDefs && !containsDefNames)
        {
            return 0;
        }

        if (list.IsReadOnly || list.IsFixedSize)
        {
            return 0;
        }

        int removed = 0;
        for (int index = list.Count - 1; index >= 0; index--)
        {
            object? entry = list[index];
            bool invalid;

            if (containsCategoryDefs)
            {
                invalid = entry is not ThingCategoryDef category ||
                          !IsCurrentCategoryDef(category);
            }
            else
            {
                invalid = entry is not string defName ||
                          string.IsNullOrWhiteSpace(defName) ||
                          DefDatabase<ThingCategoryDef>.GetNamedSilentFail(defName) is null;
            }

            if (!invalid)
            {
                continue;
            }

            try
            {
                list.RemoveAt(index);
                removed++;
            }
            catch (NotSupportedException)
            {
                return removed;
            }
            catch (InvalidOperationException)
            {
                return removed;
            }
        }

        return removed;
    }

    private static bool IsCurrentCategoryDef(ThingCategoryDef category)
    {
        if (string.IsNullOrWhiteSpace(category.defName))
        {
            return false;
        }

        ThingCategoryDef? current =
            DefDatabase<ThingCategoryDef>.GetNamedSilentFail(category.defName);

        return ReferenceEquals(current, category);
    }

    private static Type? GetListElementType(Type listType)
    {
        if (listType.IsArray)
        {
            return listType.GetElementType();
        }

        if (listType.IsGenericType &&
            listType.GetGenericTypeDefinition() == typeof(List<>))
        {
            return listType.GetGenericArguments()[0];
        }

        Type? enumerableType = listType.GetInterfaces()
            .FirstOrDefault(candidate =>
                candidate.IsGenericType &&
                candidate.GetGenericTypeDefinition() == typeof(IEnumerable<>));

        return enumerableType?.GetGenericArguments()[0];
    }

    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException exception)
        {
            return exception.Types.Where(type => type is not null).Cast<Type>();
        }
    }
}
