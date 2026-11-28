using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace RWAutoSell
{
	// Token: 0x0200000E RID: 14
	[NullableContext(1)]
	[Nullable(0)]
	[StaticConstructorOnStartup]
	internal class ASTextures
	{
		// Token: 0x04000021 RID: 33
		public static readonly Texture2D Suspend = ContentFinder<Texture2D>.Get("UI/Buttons/Suspend", true);

		// Token: 0x04000022 RID: 34
		public static readonly Texture2D Clone = ContentFinder<Texture2D>.Get("UI/Buttons/Copy", true);

		// Token: 0x04000023 RID: 35
		public static Texture2D ASyx = ContentFinder<Texture2D>.Get("RWAutoSell/ASSymbol64", true);

		// Token: 0x04000024 RID: 36
		public static Texture2D Chain = ContentFinder<Texture2D>.Get("RWAutoSell/chain", true);

		// Token: 0x04000025 RID: 37
		public static Texture2D AdvMenu = ContentFinder<Texture2D>.Get("RWAutoSell/AdvancedMenu", true);

		// Token: 0x04000026 RID: 38
		public static Texture2D Undo = ContentFinder<Texture2D>.Get("RWAutoSell/Undo", true);

		// Token: 0x04000027 RID: 39
		public static Texture2D Redo = ContentFinder<Texture2D>.Get("RWAutoSell/Redo", true);

		// Token: 0x04000028 RID: 40
		public static Texture2D FCat = ContentFinder<Texture2D>.Get("RWAutoSell/FilterCat", true);

		// Token: 0x04000029 RID: 41
		public static Texture2D FAge = ContentFinder<Texture2D>.Get("RWAutoSell/FilterAge", true);

		// Token: 0x0400002A RID: 42
		public static Texture2D FBond = ContentFinder<Texture2D>.Get("RWAutoSell/FilterBonded", true);

		// Token: 0x0400002B RID: 43
		public static Texture2D FGender = ContentFinder<Texture2D>.Get("RWAutoSell/FilterGender", true);

		// Token: 0x0400002C RID: 44
		public static Texture2D FHealth = ContentFinder<Texture2D>.Get("RWAutoSell/FilterHealth", true);

		// Token: 0x0400002D RID: 45
		public static Texture2D FNamed = ContentFinder<Texture2D>.Get("RWAutoSell/FilterNamed", true);

		// Token: 0x0400002E RID: 46
		public static Texture2D FPKind = ContentFinder<Texture2D>.Get("RWAutoSell/FilterPawnKind", true);

		// Token: 0x0400002F RID: 47
		public static Texture2D FQuality = ContentFinder<Texture2D>.Get("RWAutoSell/FilterQuality", true);

		// Token: 0x04000030 RID: 48
		public static Texture2D FSick = ContentFinder<Texture2D>.Get("RWAutoSell/FilterSick", true);

		// Token: 0x04000031 RID: 49
		public static Texture2D FStuff = ContentFinder<Texture2D>.Get("RWAutoSell/FilterStuff", true);

		// Token: 0x04000032 RID: 50
		public static Texture2D FTrait = ContentFinder<Texture2D>.Get("RWAutoSell/FilterTrait", true);

		// Token: 0x04000033 RID: 51
		public static Texture2D FLStack = ContentFinder<Texture2D>.Get("RWAutoSell/FilterLStack", true);

		// Token: 0x04000034 RID: 52
		public static Texture2D FApparel = ContentFinder<Texture2D>.Get("RWAutoSell/FilterApparel", true);

		// Token: 0x04000035 RID: 53
		public static Texture2D FPrice = ContentFinder<Texture2D>.Get("RWAutoSell/FilterBuyPrice", true);

		// Token: 0x04000036 RID: 54
		public static Texture2D FWorkTag = ContentFinder<Texture2D>.Get("RWAutoSell/FilterWorkTag", true);

		// Token: 0x04000037 RID: 55
		public static Texture2D FSkill = ContentFinder<Texture2D>.Get("RWAutoSell/FilterSkill", true);

		// Token: 0x04000038 RID: 56
		public static Texture2D FAnimal = ContentFinder<Texture2D>.Get("RWAutoSell/FilterAnimal", true);

		// Token: 0x04000039 RID: 57
		public static Texture2D FStat = ContentFinder<Texture2D>.Get("RWAutoSell/FilterStat", true);

		// Token: 0x0400003A RID: 58
		public static Texture2D FTime = ContentFinder<Texture2D>.Get("RWAutoSell/FilterTime", true);

		// Token: 0x0400003B RID: 59
		public static Texture2D FPerish = ContentFinder<Texture2D>.Get("RWAutoSell/FilterPerish", true);

		// Token: 0x0400003C RID: 60
		public static Texture2D FCap = ContentFinder<Texture2D>.Get("RWAutoSell/FilterCapacity", true);

		// Token: 0x0400003D RID: 61
		public static Texture2D FSerum = ContentFinder<Texture2D>.Get("RWAutoSell/FilterSerum", true);

		// Token: 0x0400003E RID: 62
		public static Texture2D FWeapon = ContentFinder<Texture2D>.Get("RWAutoSell/FilterWeapon", true);

		// Token: 0x0400003F RID: 63
		public static Texture2D FSmelt = ContentFinder<Texture2D>.Get("RWAutoSell/FilterSmelt", true);

		// Token: 0x04000040 RID: 64
		public static Texture2D FZone = ContentFinder<Texture2D>.Get("RWAutoSell/FilterZone", true);

		// Token: 0x04000041 RID: 65
		public static Texture2D FArea = ContentFinder<Texture2D>.Get("RWAutoSell/FilterArea", true);

		// Token: 0x04000042 RID: 66
		public static Texture2D FIng = ContentFinder<Texture2D>.Get("RWAutoSell/FilterIngredient", true);

		// Token: 0x04000043 RID: 67
		public static Texture2D FChem = ContentFinder<Texture2D>.Get("RWAutoSell/FilterChem", true);

		// Token: 0x04000044 RID: 68
		public static Texture2D FPrisoner = ContentFinder<Texture2D>.Get("RWAutoSell/FilterPrisoner", true);

		// Token: 0x04000045 RID: 69
		public static Texture2D FGene = ContentFinder<Texture2D>.Get("RWAutoSell/FilterGene", true);

		// Token: 0x04000046 RID: 70
		public static Texture2D FPawnXeno = ContentFinder<Texture2D>.Get("RWAutoSell/FilterXeno", true);

		// Token: 0x04000047 RID: 71
		public static Texture2D FIdeo = ContentFinder<Texture2D>.Get("RWAutoSell/FilterIdeo", true);

		// Token: 0x04000048 RID: 72
		public static Texture2D FTech = ContentFinder<Texture2D>.Get("RWAutoSell/FilterTechPrint", true);

		// Token: 0x04000049 RID: 73
		public static Texture2D FBook = ContentFinder<Texture2D>.Get("RWAutoSell/FilterBook", true);

		// Token: 0x0400004A RID: 74
		public static Texture2D Group = ContentFinder<Texture2D>.Get("RWAutoSell/FilterGroup", true);
	}
}
