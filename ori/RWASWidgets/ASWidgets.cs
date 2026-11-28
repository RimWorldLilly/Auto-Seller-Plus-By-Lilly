using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace RWASWidgets
{
	// Token: 0x02000002 RID: 2
	[NullableContext(1)]
	[Nullable(0)]
	public static class ASWidgets
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public static bool ButtonImageNoBad(Rect butRect, Texture2D tex)
		{
			return ASWidgets.ButtonImageNoBad(butRect, tex, Color.white);
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002060 File Offset: 0x00000260
		public static bool ButtonImageNoBad(Rect butRect, Texture2D tex, Color baseColor)
		{
			if (tex != null && tex != BaseContent.BadTex)
			{
				Color color = GUI.color;
				bool result = Widgets.ButtonImage(butRect, tex, baseColor, true, null);
				GUI.color = color;
				return result;
			}
			return Widgets.ButtonInvisible(butRect, false);
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000020A1 File Offset: 0x000002A1
		[NullableContext(0)]
		public static void AlignedTextBox<T>(Rect inrect, [Nullable(1)] string Label, ref T Value, [Nullable(1)] ref string Cache, float Min, float Max) where T : struct
		{
			Widgets.TextFieldNumeric<T>(ASWidgets.LabelCalc(inrect, Label), ref Value, ref Cache, Min, Max);
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000020B5 File Offset: 0x000002B5
		public static string AlignedTextBox(Rect inrect, string Label, string Value)
		{
			return Widgets.TextField(ASWidgets.LabelCalc(inrect, Label), Value);
		}

		// Token: 0x06000005 RID: 5 RVA: 0x000020C4 File Offset: 0x000002C4
		private static Rect LabelCalc(Rect inrect, string Label)
		{
			float num = Text.CalcSize(Label).x * 1.04f + 2f;
			Rect rect = new Rect(inrect.x, inrect.y, num, inrect.height);
			Widgets.Label(rect, Label);
			return new Rect(rect.xMax + 5f, inrect.y, inrect.width - (num + 10f), inrect.height);
		}

		// Token: 0x06000006 RID: 6 RVA: 0x0000213C File Offset: 0x0000033C
		[NullableContext(0)]
		public static void TextFieldStruct<T>(Rect rect, ref T field, [Nullable(1)] ref string text, T Min, T Max) where T : struct
		{
			string text2 = text;
			text = Widgets.TextField(rect, text);
			string text3 = "";
			foreach (char c in text)
			{
				int num;
				if (c == '-' || c == '.' || int.TryParse(c.ToString(), out num))
				{
					ReadOnlySpan<char> str = text3;
					char c2 = c;
					text3 = str + new ReadOnlySpan<char>(ref c2);
				}
			}
			if (text3.Count((char x) => x == '-') > 1)
			{
				char c3 = text3[0];
				string text5 = c3.ToString();
				for (int j = 1; j < text3.Count<char>(); j++)
				{
					if (text3[j] != '-')
					{
						ReadOnlySpan<char> str2 = text5;
						c3 = text3[j];
						text5 = str2 + new ReadOnlySpan<char>(ref c3);
					}
				}
				text3 = text5;
			}
			if (text3.Count((char x) => x == '.') > 1)
			{
				int num2 = text3.FirstIndexOf((char x) => x == '.');
				string text6 = text3.Substring(0, num2 + 1);
				foreach (char c4 in text3.Substring(num2 + 1))
				{
					if (c4 != '.')
					{
						ReadOnlySpan<char> str3 = text6;
						char c5 = c4;
						text6 = str3 + new ReadOnlySpan<char>(ref c5);
					}
				}
				text3 = text6;
			}
			text = text3;
			T t = default(T);
			bool flag = true;
			try
			{
				TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
				t = (T)((object)converter.ConvertFromString(text));
				if ((double)converter.ConvertTo(t, typeof(double)) > (double)converter.ConvertTo(Max, typeof(double)))
				{
					t = Max;
					text = Max.ToString();
				}
				else if ((double)converter.ConvertTo(t, typeof(double)) < (double)converter.ConvertTo(Min, typeof(double)))
				{
					t = Max;
					text = Min.ToString();
				}
				field = t;
			}
			catch
			{
				flag = false;
			}
			if (flag)
			{
				return;
			}
			if (text.Length > 1)
			{
				text = text2;
				return;
			}
			if (text.Length <= 1)
			{
				try
				{
					field = (T)((object)TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(0));
				}
				catch
				{
					field = default(T);
				}
				text = string.Empty;
			}
		}

		// Token: 0x06000007 RID: 7 RVA: 0x00002428 File Offset: 0x00000628
		public static void TextFieldPercent(Rect rect, ref float field, ref string text, float Min, float Max)
		{
			Rect rect2 = new Rect(rect.x, rect.y, rect.width - 25f, rect.height);
			Widgets.Label(new Rect(rect2.xMax, rect.y, 25f, rect2.height), "%");
			int num = Mathf.RoundToInt(field * 100f);
			ASWidgets.TextFieldStruct<int>(rect2, ref num, ref text, (int)(Min * 100f), (int)(Max * 100f));
			field = (float)num / 100f;
		}

		// Token: 0x06000008 RID: 8 RVA: 0x000024BC File Offset: 0x000006BC
		public static void ASCheckbox(Rect rect, TaggedString label, ref bool checkOn)
		{
			TextAnchor anchor = Text.Anchor;
			Text.Anchor = TextAnchor.MiddleLeft;
			Rect rect2 = rect;
			rect2.xMax -= 24f;
			Widgets.Label(rect2, label);
			if (Widgets.ButtonInvisible(rect, true))
			{
				checkOn = !checkOn;
			}
			Widgets.CheckboxDraw(rect.x + rect.width - 24f, rect.y + (rect.height - 24f) / 2f, checkOn, false, 24f, null, null);
			Text.Anchor = anchor;
		}
	}
}
