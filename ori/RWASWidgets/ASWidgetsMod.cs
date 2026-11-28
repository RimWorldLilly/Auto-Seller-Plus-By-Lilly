using System;
using System.Runtime.CompilerServices;
using Verse;

namespace RWASWidgets
{
	// Token: 0x02000003 RID: 3
	[NullableContext(1)]
	[Nullable(0)]
	internal class ASWidgetsMod : Mod
	{
		// Token: 0x06000009 RID: 9 RVA: 0x00002544 File Offset: 0x00000744
		public ASWidgetsMod(ModContentPack content) : base(content)
		{
		}

		// Token: 0x0600000A RID: 10 RVA: 0x0000254D File Offset: 0x0000074D
		public static ASWidgetsModSettings GetSettings()
		{
			return ASWidgetsMod.GetSingleton.GetSettings<ASWidgetsModSettings>();
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x0600000B RID: 11 RVA: 0x00002559 File Offset: 0x00000759
		public static ASWidgetsMod GetSingleton
		{
			get
			{
				return LoadedModManager.GetMod<ASWidgetsMod>();
			}
		}
	}
}
