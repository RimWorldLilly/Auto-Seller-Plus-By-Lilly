using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace RWASWidgets
{
	// Token: 0x0200000C RID: 12
	[NullableContext(1)]
	[Nullable(0)]
	public class ASColourWidget
	{
		// Token: 0x0600004C RID: 76 RVA: 0x00003884 File Offset: 0x00001A84
		public bool DrawWidget(Rect InRect)
		{
			float num = Mathf.Min(InRect.height - 30f, InRect.width);
			float height = InRect.height - num;
			Rect rect = new Rect(InRect.x, InRect.y, InRect.width, height);
			Widgets.DrawBoxSolid(rect, this.retcol);
			Color color = GUI.color;
			GUI.color = Color.white;
			Widgets.DrawBox(rect, 1, null);
			GUI.color = color;
			bool result = false;
			Rect position = new Rect(InRect.x, InRect.y + rect.height, 30f, InRect.height - rect.height);
			GUI.BeginGroup(position);
			Rect rect2 = new Rect(0f, 0f, 30f, position.height);
			GUI.DrawTexture(rect2, this.VPicker);
			if (Mouse.IsOver(rect2) && Event.current.button == 0 && Event.current.type == EventType.mouseUp)
			{
				this.val = 1f - Event.current.mousePosition.y / rect2.height;
				Color lhs = Color.HSVToRGB(this.hue, this.sat, this.val);
				if (lhs != Color.clear)
				{
					result = true;
					this.picker.SetPixels(this.RefreshWheel(this.val));
					this.picker.Apply(false);
				}
				this.retcol = lhs;
			}
			Color color2 = GUI.color;
			GUI.color = Color.white;
			Widgets.DrawBox(rect2, 1, null);
			GUI.color = Color.black;
			Widgets.DrawBox(new Rect(0f, (1f - this.val) * rect2.height, rect2.width, 3f), 2, null);
			GUI.color = color2;
			GUI.EndGroup();
			Rect position2 = new Rect(position.xMax, InRect.y + rect.height, InRect.width - position.width, InRect.height - rect.height);
			float num2 = Mathf.Min(position2.width, position2.height);
			GUI.BeginGroup(position2);
			Rect rect3 = new Rect(0f, 0f, num2, num2);
			GUI.DrawTexture(rect3, this.pickerout);
			Rect position3 = rect3.ContractedBy(2f);
			GUI.BeginGroup(position3);
			Rect rect4 = new Rect(0f, 0f, position3.width, position3.height);
			GUI.DrawTexture(rect4, this.picker);
			if (Mouse.IsOver(rect4) && Event.current.button == 0 && Event.current.type == EventType.mouseUp)
			{
				Vector2 mousePosition = Event.current.mousePosition;
				Vector2 vector = mousePosition / num2;
				vector *= (float)this.picker.height;
				vector.y = (float)this.picker.height - vector.y;
				Color pixel = this.picker.GetPixel((int)vector.x, (int)vector.y);
				if (pixel != Color.clear)
				{
					result = true;
					this.sat = this.SatFromPoint(mousePosition, rect4);
					this.hue = this.HueFromPoint(mousePosition, rect4);
					this.VPicker.SetPixels(this.RefreshVSlider(this.hue, this.sat));
					this.VPicker.Apply(false);
					this.retcol = pixel;
				}
			}
			Color color3 = GUI.color;
			GUI.color = Color.black;
			Vector2 vector2 = this.CirclePoint(this.hue, this.sat, rect4);
			GUI.DrawTexture(new Rect(vector2.x - 4f, vector2.y - 4f, 8f, 8f), WidgetTextures.Circle);
			GUI.color = color3;
			GUI.EndGroup();
			GUI.EndGroup();
			return result;
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00003C68 File Offset: 0x00001E68
		private Vector2 CirclePoint(float Hue, float Sat, Rect rect)
		{
			float num = rect.height / 2f;
			float x = (float)((double)(this.sat * num) * Math.Sin((double)this.hue * 6.283185307179586)) + num;
			float y = rect.height - ((float)((double)(this.sat * num) * Math.Cos((double)this.hue * 6.283185307179586)) + num);
			return new Vector2(x, y);
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00003CD8 File Offset: 0x00001ED8
		private float SatFromPoint(Vector2 point, Rect PickerArea)
		{
			float num = PickerArea.width / 2f;
			float num2 = PickerArea.height / 2f;
			float f = point.x - num;
			float f2 = point.y - num2;
			return Mathf.Sqrt(Mathf.Pow(f, 2f) + Mathf.Pow(f2, 2f)) / num;
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00003D30 File Offset: 0x00001F30
		private float HueFromPoint(Vector2 point, Rect PickerArea)
		{
			float num = PickerArea.width / 2f;
			float num2 = PickerArea.height / 2f;
			double num3 = (double)(point.x - num);
			float num4 = point.y - num2;
			double num5 = Math.Atan2(num3, (double)num4);
			if (num5 < 0.0)
			{
				num5 += 6.283185307179586;
			}
			float num6 = (float)(num5 / 6.283185307179586);
			num6 = 1f - num6 + 0.5f;
			if (num6 > 1f)
			{
				num6 -= 1f;
			}
			return num6;
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000050 RID: 80 RVA: 0x00003DBE File Offset: 0x00001FBE
		// (set) Token: 0x06000051 RID: 81 RVA: 0x00003DC8 File Offset: 0x00001FC8
		public Color SelectedColor
		{
			get
			{
				return this.retcol;
			}
			set
			{
				this.hue = this.HueFromCol(value);
				this.val = Mathf.Max(new float[]
				{
					value.r,
					value.g,
					value.b
				});
				this.sat = this.SatFromCol(value);
				this.picker.SetPixels(this.RefreshWheel(this.val));
				this.picker.Apply(false);
				this.VPicker.SetPixels(this.RefreshVSlider(this.hue, this.sat));
				this.VPicker.Apply(false);
				this.retcol = value;
			}
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00003E70 File Offset: 0x00002070
		private Color[] RefreshVSlider(float hue, float sat)
		{
			Color[] array = new Color[this.VPicker.height * this.VPicker.width];
			for (int i = 0; i < this.VPicker.height; i++)
			{
				float v = (float)i / (float)this.VPicker.height;
				for (int j = 0; j < this.VPicker.width; j++)
				{
					array[j + i * this.VPicker.width] = Color.HSVToRGB(hue, sat, v);
				}
			}
			return array;
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00003EF4 File Offset: 0x000020F4
		public ASColourWidget(int texsize = 256)
		{
			this.size = texsize;
			this.picker = new Texture2D(this.size, this.size, TextureFormat.RGBA32, false, true);
			Color[] array = this.RefreshWheel(this.val);
			this.picker.SetPixels(array);
			this.picker.Apply(false);
			this.retcol = this.picker.GetPixel(this.picker.width / 2, this.picker.width / 2);
			this.pickerout = new Texture2D(this.size, this.size, TextureFormat.RGBA32, false, true);
			Color[] array2 = new Color[array.Count<Color>()];
			for (int i = 0; i < array.Count<Color>(); i++)
			{
				if (array[i] != Color.clear)
				{
					array2[i] = Color.white;
				}
				else
				{
					array2[i] = Color.clear;
				}
			}
			this.pickerout.SetPixels(array2);
			this.pickerout.Apply(false);
			this.VPicker = new Texture2D(this.size, this.size, TextureFormat.RGBA32, false, true);
			this.VPicker.SetPixels(this.RefreshVSlider(this.hue, this.sat));
			this.VPicker.Apply(false);
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00004044 File Offset: 0x00002244
		private float HueFromCol(Color col)
		{
			float num = 0f;
			float num2 = Mathf.Min(new float[]
			{
				col.r,
				col.g,
				col.b
			});
			float num3 = Mathf.Max(new float[]
			{
				col.r,
				col.g,
				col.b
			});
			float num4 = num3 - num2;
			if (col.r == num3)
			{
				num = (col.g - col.b) / num4;
			}
			else if (col.g == num3)
			{
				num = 2f + (col.b - col.r) / num4;
			}
			else if (col.b == num3)
			{
				num = 4f + (col.r - col.g) / num4;
			}
			num *= 60f;
			if (num < 0f)
			{
				num += 360f;
			}
			return num / 360f;
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00004128 File Offset: 0x00002328
		private float SatFromCol(Color col)
		{
			float num = Mathf.Max(new float[]
			{
				col.r,
				col.g,
				col.b
			});
			float num2 = Mathf.Min(new float[]
			{
				col.r,
				col.g,
				col.b
			});
			float num3 = num - num2;
			if (num3 == 0f || num == 0f)
			{
				return 0f;
			}
			return num3 / num;
		}

		// Token: 0x06000056 RID: 86 RVA: 0x000041A0 File Offset: 0x000023A0
		private Color[] RefreshWheel(float V = 0.5f)
		{
			int num = this.picker.width / 2;
			int num2 = this.picker.height / 2;
			int height = this.picker.height;
			Color[] array = new Color[this.picker.height * this.picker.width];
			for (int i = 0; i < array.Count<Color>(); i++)
			{
				int num3 = i % this.picker.width;
				int num4 = i / this.picker.width;
				int num5 = num3 - num;
				int num6 = num4 - num2;
				float num7 = Mathf.Sqrt(Mathf.Pow((float)num6, 2f) + Mathf.Pow((float)num5, 2f));
				if (num7 > (float)num)
				{
					array[i] = Color.clear;
				}
				else
				{
					double num8 = Math.Atan2((double)num5, (double)num6);
					if (num8 < 0.0)
					{
						num8 += 6.283185307179586;
					}
					float s = num7 / (float)num;
					float h = (float)(num8 / 6.283185307179586);
					array[i] = Color.HSVToRGB(h, s, V);
				}
			}
			return array;
		}

		// Token: 0x04000023 RID: 35
		private Texture2D picker;

		// Token: 0x04000024 RID: 36
		private Texture2D pickerout;

		// Token: 0x04000025 RID: 37
		private Texture2D VPicker;

		// Token: 0x04000026 RID: 38
		private Color retcol = Color.clear;

		// Token: 0x04000027 RID: 39
		private float sat;

		// Token: 0x04000028 RID: 40
		private float hue;

		// Token: 0x04000029 RID: 41
		private float val;

		// Token: 0x0400002A RID: 42
		private int size;
	}
}
