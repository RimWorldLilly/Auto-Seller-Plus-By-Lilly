using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RimWorld;
using RWASFilterLib;
using UnityEngine;
using Verse;

namespace RWAutoSell.Filters
{
	// Token: 0x02000052 RID: 82
	[NullableContext(1)]
	[Nullable(0)]
	public class FilterTime : IBaseFilter, IWikiHelp
	{
		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x060002F6 RID: 758 RVA: 0x0001C11E File Offset: 0x0001A31E
		public string[] Tags
		{
			get
			{
				return new string[]
				{
					"RWAutoSell.TagItems",
					"RWAutoSell.TagFood",
					"RWAutoSell.TagAnimals",
					"RWAutoSell.TagPeople"
				};
			}
		}

		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x060002F7 RID: 759 RVA: 0x0001C146 File Offset: 0x0001A346
		public string Label
		{
			get
			{
				return "RWAutoSell.FilterTime".Translate();
			}
		}

		// Token: 0x170000BA RID: 186
		// (get) Token: 0x060002F8 RID: 760 RVA: 0x0001C157 File Offset: 0x0001A357
		public Texture2D MiniGraphic
		{
			get
			{
				return ASTextures.FTime;
			}
		}

		// Token: 0x170000BB RID: 187
		// (get) Token: 0x060002F9 RID: 761 RVA: 0x0001C15E File Offset: 0x0001A35E
		public string HelpPage
		{
			get
			{
				return "https://github.com/SupesSolutions/RWAutoSeller/wiki/Time-Filter";
			}
		}

		// Token: 0x060002FA RID: 762 RVA: 0x0001C168 File Offset: 0x0001A368
		public bool CheckItem(Transferable t, List<string> Data)
		{
			if (ASLibMod.ActiveComp != null && Data.Count<string>() != 0)
			{
				int index = GenLocalDate.DayOfYear(ASLibMod.ActiveComp.map);
				if (Data[0][index] == '1')
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060002FB RID: 763 RVA: 0x0001C1A8 File Offset: 0x0001A3A8
		public List<string> DataFromTradeable(Transferable t)
		{
			return new List<string>();
		}

		// Token: 0x060002FC RID: 764 RVA: 0x0001C1AF File Offset: 0x0001A3AF
		public List<string> DataOptions(List<string> Data, Map map)
		{
			return Data;
		}

		// Token: 0x060002FD RID: 765 RVA: 0x0001C1B4 File Offset: 0x0001A3B4
		public void DrawWidget(Rect inrect, List<string> Data)
		{
			if (this.AllowRender && Data.Count > 0 && Data[0].Count<char>() == 60 && ASLibMod.ActiveComp != null)
			{
				if (Event.current.type == EventType.mouseDown)
				{
					if (Event.current.button == 0)
					{
						this.linedragid = 0;
					}
					else if (Event.current.button == 1)
					{
						this.linedragid = 1;
					}
				}
				bool flag = false;
				char[] array = Data[0].ToArray<char>();
				int num = Mathf.FloorToInt(inrect.width / 18f);
				float num2 = Mathf.Floor((float)num * 0.9f);
				Map map = ASLibMod.ActiveComp.map;
				Vector2 longLat = Find.WorldGrid.LongLatOf(map.Tile);
				for (int i = 0; i < 60; i++)
				{
					int num3 = i % 15;
					int num4 = num3 / 5;
					int num5 = i / 15;
					Quadrum quadrum = (Quadrum)num5;
					Season season = GenDate.Season((long)(i * 60000), longLat);
					Rect rect = new Rect(inrect.x + (float)(num3 * num) + (float)(num4 * num), inrect.y + (float)num5 * ((float)num + 30f) + 30f, num2, num2);
					TooltipHandler.TipRegion(rect, new TipSignal(string.Concat(new string[]
					{
						season.Label(),
						" ",
						quadrum.Label(),
						" ",
						(num3 + 1).ToString()
					})));
					Rect rect2 = new Rect(inrect.x + (float)(num3 * num) + (float)(num4 * num), inrect.y + (float)num5 * ((float)num + 30f) + 30f, num2, num2);
					switch (season)
					{
					case Season.Undefined:
						Widgets.DrawBoxSolid(rect2, Color.black * 0.5f);
						break;
					case Season.Spring:
						Widgets.DrawBoxSolid(rect2, Color.green * 0.5f);
						break;
					case Season.Summer:
					case Season.PermanentSummer:
						Widgets.DrawBoxSolid(rect2, Color.yellow * 0.5f);
						break;
					case Season.Fall:
						Widgets.DrawBoxSolid(rect2, this.Orange * 0.5f);
						break;
					case Season.Winter:
					case Season.PermanentWinter:
						Widgets.DrawBoxSolid(rect2, Color.blue * 0.5f);
						break;
					}
					if (num3 == 14)
					{
						Rect rect3 = new Rect(inrect.x + 10f, inrect.y + (float)num5 * ((float)num + 30f) + 10f, 270f, num2);
						TooltipHandler.TipRegion(rect3, new TipSignal("RWAutoSell.FilterTimeTip".Translate()));
						Widgets.Label(rect3, quadrum.Label());
					}
					Color color = GUI.color;
					if (array[i] == '1')
					{
						GUI.color = Color.cyan;
					}
					else
					{
						GUI.color = Color.red;
					}
					Widgets.DrawBox(rect.ExpandedBy(1f), 1, null);
					GUI.color = color;
					Widgets.Label(rect.ContractedBy(2f), (i + 1).ToString());
					if (GenLocalDate.DayOfYear(map) == i)
					{
						GUI.color = Color.yellow;
						Widgets.DrawBox(rect, 1, null);
						GUI.color = color;
					}
					if (Mouse.IsOver(rect) && this.linedragid != -1)
					{
						if (this.linedragid == 0)
						{
							array[i] = '1';
							flag = true;
						}
						else if (this.linedragid == 1)
						{
							array[i] = '0';
							flag = true;
						}
					}
				}
				if (flag)
				{
					Data[0] = new string(array);
				}
				if (Event.current.type == EventType.mouseUp)
				{
					this.linedragid = -1;
				}
			}
		}

		// Token: 0x060002FE RID: 766 RVA: 0x0001C544 File Offset: 0x0001A744
		public void RenderStarted(List<string> Data, Map map)
		{
			if (Data.Count == 0 || Data[0].Count<char>() != 60)
			{
				char[] array = new char[60];
				for (int i = 0; i < 60; i++)
				{
					array[i] = '1';
				}
				Data.Add(new string(array));
			}
			this.AllowRender = true;
			this.linedragid = -1;
		}

		// Token: 0x060002FF RID: 767 RVA: 0x0001C59D File Offset: 0x0001A79D
		public void RenderStopped(List<string> Data, Map map)
		{
			this.AllowRender = false;
			this.linedragid = -1;
		}

		// Token: 0x0400018F RID: 399
		private Color Orange = new Color(1f, 0.5f, 0f);

		// Token: 0x04000190 RID: 400
		private bool AllowRender;

		// Token: 0x04000191 RID: 401
		private int linedragid = -1;
	}
}
