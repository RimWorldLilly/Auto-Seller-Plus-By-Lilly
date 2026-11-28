using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace RWAutoSell
{
	// Token: 0x0200001B RID: 27
	[NullableContext(1)]
	[Nullable(0)]
	public class ASDialog_EnableWarn<[Nullable(2)] T> : Window
	{
		// Token: 0x060000C6 RID: 198 RVA: 0x00008EFC File Offset: 0x000070FC
		public ASDialog_EnableWarn() : base(null)
		{
			this.forcePause = true;
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x060000C7 RID: 199 RVA: 0x00008F22 File Offset: 0x00007122
		public override Vector2 InitialSize
		{
			get
			{
				return new Vector2(350f, 300f);
			}
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x00008F33 File Offset: 0x00007133
		public ASDialog_EnableWarn(Action<T> action, T inval, string title, string desc) : this()
		{
			this.Input = inval;
			this.doaction = action;
			this.Title = title;
			this.Desc = desc;
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x00008F58 File Offset: 0x00007158
		public override void OnAcceptKeyPressed()
		{
			this.doaction(this.Input);
			base.OnAcceptKeyPressed();
		}

		// Token: 0x060000CA RID: 202 RVA: 0x00008F74 File Offset: 0x00007174
		public override void DoWindowContents(Rect inRect)
		{
			Rect rect = new Rect(inRect.x, inRect.y, inRect.width, (float)((int)(inRect.height / 3f)));
			Text.Font = GameFont.Medium;
			Widgets.Label(rect, this.Title);
			Text.Font = GameFont.Small;
			Widgets.Label(new Rect(inRect.x, rect.yMax, inRect.width, (float)((int)(inRect.height / 3f) * 2 - 45)), this.Desc);
			if (Widgets.ButtonText(new Rect(inRect.width / 2f - 165f, inRect.yMax - 40f, 160f, 40f), "AcceptButton".Translate(), true, true, true, null))
			{
				this.OnAcceptKeyPressed();
				this.Close(true);
			}
			if (Widgets.ButtonText(new Rect(inRect.width / 2f + 5f, inRect.yMax - 40f, 160f, 40f), "CancelButton".Translate(), true, true, true, null))
			{
				this.Close(true);
			}
		}

		// Token: 0x0400008B RID: 139
		private Action<T> doaction;

		// Token: 0x0400008C RID: 140
		private T Input;

		// Token: 0x0400008D RID: 141
		private string Title = string.Empty;

		// Token: 0x0400008E RID: 142
		private string Desc = string.Empty;
	}
}
