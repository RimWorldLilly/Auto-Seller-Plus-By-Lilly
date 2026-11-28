using System;
using System.Runtime.CompilerServices;
using Verse;

namespace RWASWidgets.Windows
{
	// Token: 0x02000018 RID: 24
	public static class Utilitys
	{
		// Token: 0x060000F0 RID: 240 RVA: 0x00007055 File Offset: 0x00005255
		[NullableContext(1)]
		public static void Show(this Window This)
		{
			Find.WindowStack.Add(This);
		}
	}
}
