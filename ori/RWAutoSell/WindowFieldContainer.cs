using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace RWAutoSell
{
	// Token: 0x02000017 RID: 23
	[NullableContext(1)]
	[Nullable(0)]
	public class WindowFieldContainer
	{
		// Token: 0x060000B7 RID: 183 RVA: 0x00007E94 File Offset: 0x00006094
		public WindowFieldContainer(Type Window, FieldInfo TradableField, bool Whitelisted = false)
		{
			this.WindowType = Window;
			if (TradableField != null)
			{
				this.FieldName = TradableField.Name;
				this.ispublic = TradableField.IsPublic;
			}
			this.knownmod = false;
			this.WhiteListed = Whitelisted;
			this.dopostopen = true;
			this.dowindowcontent = true;
			this.dofill = true;
		}

		// Token: 0x0400007D RID: 125
		public Type WindowType;

		// Token: 0x0400007E RID: 126
		public string FieldName;

		// Token: 0x0400007F RID: 127
		public bool ispublic;

		// Token: 0x04000080 RID: 128
		public bool knownmod;

		// Token: 0x04000081 RID: 129
		public bool WhiteListed;

		// Token: 0x04000082 RID: 130
		public bool dopostopen;

		// Token: 0x04000083 RID: 131
		public bool dowindowcontent;

		// Token: 0x04000084 RID: 132
		public bool dofill;
	}
}
