using System;
using System.Runtime.CompilerServices;
using RimWorld;
using RWASFilterLib;
using Verse;

namespace RWAutoSell.Mergers
{
	// Token: 0x0200002B RID: 43
	[NullableContext(1)]
	[Nullable(0)]
	public class IngredientMerger : IUnmerge
	{
		// Token: 0x17000037 RID: 55
		// (get) Token: 0x06000151 RID: 337 RVA: 0x0000E654 File Offset: 0x0000C854
		public string Label
		{
			get
			{
				return "RWAutoSell.FilterIngredient".Translate();
			}
		}

		// Token: 0x06000152 RID: 338 RVA: 0x0000E668 File Offset: 0x0000C868
		public bool AllowMerge(Thing thing, Thing thing2)
		{
			CompIngredients compIngredients = thing.TryGetComp<CompIngredients>();
			CompIngredients compIngredients2 = thing2.TryGetComp<CompIngredients>();
			if (compIngredients == null)
			{
				return compIngredients == null && compIngredients2 == null;
			}
			if (compIngredients2 == null)
			{
				return false;
			}
			if (compIngredients.ingredients.Count == 0 && compIngredients2.ingredients.Count == 0)
			{
				return true;
			}
			if (compIngredients.ingredients.Count == compIngredients2.ingredients.Count)
			{
				int num = 0;
				foreach (ThingDef item in compIngredients.ingredients)
				{
					if (compIngredients2.ingredients.Contains(item))
					{
						num++;
					}
				}
				return compIngredients.ingredients.Count == num;
			}
			return false;
		}

		// Token: 0x06000153 RID: 339 RVA: 0x0000E738 File Offset: 0x0000C938
		public bool AppliesTo(Thing thing)
		{
			return thing.def.HasComp(typeof(CompIngredients));
		}
	}
}
