using System;
using System.Runtime.CompilerServices;

namespace RWASWidgets
{
	// Token: 0x02000008 RID: 8
	[NullableContext(1)]
	[Nullable(0)]
	public class GUICatListEventArgs : EventArgs
	{
		// Token: 0x06000033 RID: 51 RVA: 0x00002DB3 File Offset: 0x00000FB3
		public GUICatListEventArgs(CatTree Node)
		{
			this.Node = Node;
		}

		// Token: 0x04000014 RID: 20
		public CatTree Node;
	}
}
