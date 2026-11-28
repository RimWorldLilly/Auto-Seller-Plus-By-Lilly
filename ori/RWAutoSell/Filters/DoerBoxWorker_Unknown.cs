using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace RWAutoSell.Filters
{
	// Token: 0x02000035 RID: 53
	[NullableContext(1)]
	[Nullable(0)]
	public class DoerBoxWorker_Unknown : DoerBoxWorker
	{
		// Token: 0x1700004D RID: 77
		// (get) Token: 0x06000195 RID: 405 RVA: 0x0000FD84 File Offset: 0x0000DF84
		public override string Doerclass
		{
			get
			{
				return this.doerclass;
			}
		}

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x06000196 RID: 406 RVA: 0x0000FD8C File Offset: 0x0000DF8C
		public override string DoerLabel
		{
			get
			{
				return this.Doerclass.Colorize(Color.grey);
			}
		}

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x06000197 RID: 407 RVA: 0x0000FD9E File Offset: 0x0000DF9E
		public override int Expandedsize
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x06000199 RID: 409 RVA: 0x0000FDB4 File Offset: 0x0000DFB4
		public void SetUnknownClass(Type type)
		{
			this.doerclass = type.FullName;
		}

		// Token: 0x0600019A RID: 410 RVA: 0x0000FDC2 File Offset: 0x0000DFC2
		public override bool CheckDoerWorker(Book b, string[] Data)
		{
			return true;
		}

		// Token: 0x040000F1 RID: 241
		private string doerclass = string.Empty;
	}
}
