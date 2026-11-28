using System;
using System.Runtime.CompilerServices;

namespace RWASWidgets
{
	// Token: 0x0200000F RID: 15
	[NullableContext(1)]
	[Nullable(0)]
	public class GUIListEventArgs : EventArgs
	{
		// Token: 0x0600005E RID: 94 RVA: 0x00004536 File Offset: 0x00002736
		public GUIListEventArgs(int index, string Item)
		{
			this.index = index;
			this.Item = Item;
		}

		// Token: 0x04000034 RID: 52
		public int index;

		// Token: 0x04000035 RID: 53
		public string Item;
	}
}
