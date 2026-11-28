using System;
using UnityEngine;

namespace RWASWidgets
{
	// Token: 0x02000013 RID: 19
	public struct ASRectMonitor
	{
		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000098 RID: 152 RVA: 0x00005A2D File Offset: 0x00003C2D
		// (set) Token: 0x06000099 RID: 153 RVA: 0x00005A35 File Offset: 0x00003C35
		public float TotalTopUsed { readonly get; private set; }

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x0600009A RID: 154 RVA: 0x00005A3E File Offset: 0x00003C3E
		// (set) Token: 0x0600009B RID: 155 RVA: 0x00005A46 File Offset: 0x00003C46
		public float TotalBottomUsed { readonly get; private set; }

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x0600009C RID: 156 RVA: 0x00005A4F File Offset: 0x00003C4F
		// (set) Token: 0x0600009D RID: 157 RVA: 0x00005A57 File Offset: 0x00003C57
		public float TotalLeftUsed { readonly get; private set; }

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x0600009E RID: 158 RVA: 0x00005A60 File Offset: 0x00003C60
		// (set) Token: 0x0600009F RID: 159 RVA: 0x00005A68 File Offset: 0x00003C68
		public float TotalRightUsed { readonly get; private set; }

		// Token: 0x060000A0 RID: 160 RVA: 0x00005A71 File Offset: 0x00003C71
		public static implicit operator Rect(ASRectMonitor d)
		{
			return d.Remaining();
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00005A7A File Offset: 0x00003C7A
		public static implicit operator ASRectMonitor(Rect b)
		{
			return new ASRectMonitor(b);
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x00005A82 File Offset: 0x00003C82
		public ASRectMonitor(Rect inrect)
		{
			this.rect = inrect;
			this.TotalTopUsed = 0f;
			this.TotalBottomUsed = 0f;
			this.TotalLeftUsed = 0f;
			this.TotalRightUsed = 0f;
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00005AB8 File Offset: 0x00003CB8
		public Rect NextFromTop(float Height)
		{
			Rect result = this.PeekFromTop(Height);
			this.rect.height = this.rect.height - Height;
			this.rect.y = result.yMax;
			this.TotalTopUsed += Height;
			return result;
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00005B01 File Offset: 0x00003D01
		public Rect PeekFromTop(float Height)
		{
			return new Rect(this.rect.x, this.rect.y, this.rect.width, Height);
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00005B2A File Offset: 0x00003D2A
		public Rect NextFromBottom(float Height)
		{
			Rect result = this.PeekFromBottom(Height);
			this.rect.height = this.rect.height - Height;
			this.TotalBottomUsed += Height;
			return result;
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00005B54 File Offset: 0x00003D54
		public Rect PeekFromBottom(float Height)
		{
			return new Rect(this.rect.x, this.rect.yMax - Height, this.rect.width, Height);
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00005B7F File Offset: 0x00003D7F
		public Rect NextFromRight(float Width)
		{
			Rect result = this.PeekFromRight(Width);
			this.rect.width = this.rect.width - Width;
			this.TotalRightUsed += Width;
			return result;
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00005BAE File Offset: 0x00003DAE
		public Rect PeekFromRight(float Width)
		{
			return new Rect(this.rect.xMax - Width, this.rect.y, Width, this.rect.height);
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00005BDC File Offset: 0x00003DDC
		public Rect NextFromLeft(float Width)
		{
			Rect result = this.PeekFromLeft(Width);
			this.rect.width = this.rect.width - Width;
			this.rect.x = this.rect.x + Width;
			this.TotalLeftUsed += Width;
			return result;
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00005C2C File Offset: 0x00003E2C
		public Rect GetRectFromTotal(bool FromLeft = false, bool FromRight = false, bool FromTop = false, bool FromBottom = false)
		{
			float num = this.rect.x;
			float num2 = this.rect.y;
			float num3 = this.rect.height;
			float num4 = this.rect.width;
			if (FromTop)
			{
				num2 -= this.TotalTopUsed;
				num3 = this.TotalTopUsed;
			}
			if (FromBottom)
			{
				num3 = this.TotalBottomUsed;
				if (FromTop)
				{
					num3 += this.TotalTopUsed + this.rect.height;
				}
				else
				{
					num2 = this.rect.yMax;
				}
			}
			if (FromLeft)
			{
				num -= this.TotalLeftUsed;
				num4 = this.TotalLeftUsed;
			}
			if (FromRight)
			{
				num4 = this.TotalRightUsed;
				if (FromLeft)
				{
					num4 += this.TotalLeftUsed + this.rect.height;
				}
				else
				{
					num2 = this.rect.yMax;
				}
			}
			return new Rect(num, num2, num4, num3);
		}

		// Token: 0x060000AB RID: 171 RVA: 0x00005CF9 File Offset: 0x00003EF9
		public Rect PeekFromLeft(float Width)
		{
			return new Rect(this.rect.x, this.rect.y, Width, this.rect.height);
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00005D22 File Offset: 0x00003F22
		public Rect Remaining()
		{
			return this.rect;
		}

		// Token: 0x060000AD RID: 173 RVA: 0x00005D2C File Offset: 0x00003F2C
		public Rect NextSplitHorizontal()
		{
			float height = Mathf.Ceil(this.rect.height / 2f);
			return this.NextFromTop(height);
		}

		// Token: 0x060000AE RID: 174 RVA: 0x00005D58 File Offset: 0x00003F58
		public Rect NextSplitVertical()
		{
			float width = Mathf.Ceil(this.rect.width / 2f);
			return this.NextFromLeft(width);
		}

		// Token: 0x0400004B RID: 75
		private Rect rect;
	}
}
