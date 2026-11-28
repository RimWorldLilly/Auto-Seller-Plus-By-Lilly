using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RWASFilterLib;
using UnityEngine;

namespace RWAutoSell
{
	// Token: 0x02000007 RID: 7
	[NullableContext(1)]
	public interface ISectionUI
	{
		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000035 RID: 53
		IRuleComp RootComp { get; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000036 RID: 54
		// (set) Token: 0x06000037 RID: 55
		RuleRowUI DragRow { get; set; }

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000038 RID: 56
		List<RuleRowUI> Rows { get; }

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000039 RID: 57
		// (set) Token: 0x0600003A RID: 58
		ISectionUI ParentUI { get; set; }

		// Token: 0x0600003B RID: 59
		void EvaluateSize(Rect rect);

		// Token: 0x0600003C RID: 60
		void AddAction(RowActions action);
	}
}
