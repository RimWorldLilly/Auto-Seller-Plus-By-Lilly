using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RWASWidgets;
using Verse;

namespace RWAutoSell.Filters
{
	// Token: 0x0200002E RID: 46
	[NullableContext(1)]
	[Nullable(0)]
	public struct SettingSettings
	{
		// Token: 0x0600015D RID: 349 RVA: 0x0000E890 File Offset: 0x0000CA90
		public SettingSettings(string name, string value, SettingSettings.SettingType setting_type = SettingSettings.SettingType.Check, string value_type = "string", string value_format = "", string value_max = "", string value_min = "")
		{
			this.Label = name;
			this.DatVal = value;
			this.DatTp = value_type;
			this.DatST = setting_type;
			this.DatFormat = value_format;
			this.DatMax = value_max;
			this.DatMin = value_min;
		}

		// Token: 0x0600015E RID: 350 RVA: 0x0000E8C8 File Offset: 0x0000CAC8
		public int DrawRow(ref ASRectMonitor mon, List<string> Settings, DoerBoxWorker worker)
		{
			int num = 0;
			string text;
			Settings.FirstSubString(this.DatVal, out text, string.Empty);
			switch (this.DatST)
			{
			case SettingSettings.SettingType.Check:
			{
				bool flag;
				if (!bool.TryParse(text, out flag))
				{
					return 0;
				}
				num += 30;
				bool flag2 = flag;
				Widgets.CheckboxLabeled(mon.NextFromTop(30f), this.Label.Translate(), ref flag, false, null, null, false, false);
				if (flag != flag2)
				{
					Settings.ChangeSubString(this.DatVal, flag);
				}
				break;
			}
			case SettingSettings.SettingType.Radio:
				num += 30;
				if (this.DatFormat != null && this.DatFormat.Contains(','))
				{
					string[] array = this.DatFormat.Split(',', StringSplitOptions.None);
					ASTable astable = new ASTable(mon.NextFromTop(30f), array.Count<string>(), 1, 4f, 0f, true);
					int num2 = 0;
					foreach (string text2 in array)
					{
						string text3 = text2;
						string key = text2;
						if (text2.Contains(':'))
						{
							string[] array3 = text2.Split(':', StringSplitOptions.None);
							if (array3.Length == 2)
							{
								text3 = array3[0];
								key = array3[1];
							}
						}
						if (Widgets.RadioButtonLabeled(astable[num2, 0], key.Translate(), text == text3, false))
						{
							Settings.ChangeSubString(this.DatVal, text3);
						}
						num2++;
					}
				}
				break;
			case SettingSettings.SettingType.Range:
				num += 60;
				if (this.DatTp == "float")
				{
					float num3;
					float.TryParse(text, out num3);
					float min = 0f;
					float max = 1f;
					float.TryParse(this.DatMin, out min);
					float.TryParse(this.DatMax, out max);
					string label;
					if (this.DatFormat == "singlepercent")
					{
						label = num3.ToStringPercent("0.0");
					}
					else
					{
						label = num3.ToString();
					}
					float num4 = Widgets.HorizontalSlider(mon.NextFromTop(50f), num3, min, max, false, label, null, null, -1f);
					if (num4 != num3)
					{
						Settings.ChangeSubString(this.DatVal, num4);
					}
				}
				break;
			}
			return num;
		}

		// Token: 0x040000E4 RID: 228
		public string Label;

		// Token: 0x040000E5 RID: 229
		public string DatVal;

		// Token: 0x040000E6 RID: 230
		public string DatTp;

		// Token: 0x040000E7 RID: 231
		public SettingSettings.SettingType DatST;

		// Token: 0x040000E8 RID: 232
		public string DatFormat;

		// Token: 0x040000E9 RID: 233
		public string DatMax;

		// Token: 0x040000EA RID: 234
		public string DatMin;

		// Token: 0x0200007A RID: 122
		[NullableContext(0)]
		public enum SettingType
		{
			// Token: 0x040001EE RID: 494
			Check,
			// Token: 0x040001EF RID: 495
			Radio,
			// Token: 0x040001F0 RID: 496
			List,
			// Token: 0x040001F1 RID: 497
			Range
		}
	}
}
