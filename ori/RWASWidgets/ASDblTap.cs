using System;
using UnityEngine;

namespace RWASWidgets
{
	// Token: 0x0200000E RID: 14
	public class ASDblTap
	{
		// Token: 0x0600005B RID: 91 RVA: 0x000044CF File Offset: 0x000026CF
		public void Ticker()
		{
			if (this.click && this.ClickTime < 0.6f)
			{
				this.ClickTime += Time.deltaTime;
				return;
			}
			if (this.click)
			{
				this.click = false;
			}
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00004508 File Offset: 0x00002708
		public bool Click()
		{
			if (!this.click)
			{
				this.click = true;
				this.ClickTime = 0f;
				return false;
			}
			this.click = false;
			return true;
		}

		// Token: 0x04000032 RID: 50
		private float ClickTime;

		// Token: 0x04000033 RID: 51
		private bool click;
	}
}
