using System;
using UnityEngine;
using Verse;

namespace RWASWidgets
{
	// Token: 0x0200000D RID: 13
	public struct ASCurvedLine
	{
		// Token: 0x06000057 RID: 87 RVA: 0x000042B8 File Offset: 0x000024B8
		private Vector2 GetCurvePosition(float step)
		{
			float num = (1f - step) * (1f - step) * (1f - step);
			float num2 = 3f * Mathf.Pow(1f - step, 2f) * step;
			float num3 = 3f * (1f - step) * Mathf.Pow(step, 2f);
			float num4 = Mathf.Pow(step, 3f);
			return new Vector2(this.target.x * num + this.MidTarget.x * num2 + this.MidDestination.x * num3 + this.Destination.x * num4, this.target.y * num + this.MidTarget.y * num2 + this.MidDestination.y * num3 + this.Destination.y * num4);
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00004390 File Offset: 0x00002590
		public ASCurvedLine(Vector2 t, Vector2 d, Color col, float width = 1f)
		{
			this.CurveColour = col;
			this.target = t;
			this.Destination = d;
			float num = Mathf.Abs(t.x - d.x) * 0.5f;
			this.MidTarget = new Vector2(t.x + num, t.y);
			this.MidDestination = new Vector2(d.x - num, d.y);
			this.Step = 1f / (Vector2.Distance(t, d) / 3f);
			this.width = width;
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00004420 File Offset: 0x00002620
		public ASCurvedLine(Vector2 t, Vector2 d, Vector2 mt, Vector2 md, Color col, float width = 1f)
		{
			this.CurveColour = col;
			this.target = t;
			this.Destination = d;
			this.MidTarget = mt;
			this.MidDestination = md;
			this.Step = 1f / (Vector2.Distance(t, d) / 3f);
			this.width = width;
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00004474 File Offset: 0x00002674
		public void DrawWidget()
		{
			for (float num = 0f; num < 1f; num += this.Step)
			{
				Widgets.DrawLine(this.GetCurvePosition(num), this.GetCurvePosition(Mathf.Clamp(num + this.Step, 0f, 1f)), this.CurveColour, this.width);
			}
		}

		// Token: 0x0400002B RID: 43
		private Vector2 target;

		// Token: 0x0400002C RID: 44
		private Vector2 Destination;

		// Token: 0x0400002D RID: 45
		private Vector2 MidTarget;

		// Token: 0x0400002E RID: 46
		private Vector2 MidDestination;

		// Token: 0x0400002F RID: 47
		private readonly float Step;

		// Token: 0x04000030 RID: 48
		private Color CurveColour;

		// Token: 0x04000031 RID: 49
		private float width;
	}
}
