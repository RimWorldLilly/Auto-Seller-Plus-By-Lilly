using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RimWorld;
using RWASFilterLib;
using RWASWidgets;
using UnityEngine;
using Verse;

namespace RWAutoSell.Filters
{
	// Token: 0x02000039 RID: 57
	[NullableContext(1)]
	[Nullable(0)]
	public class FilterApparel : IBaseFilter, IWikiHelp
	{
		// Token: 0x1700005B RID: 91
		// (get) Token: 0x060001BC RID: 444 RVA: 0x00011040 File Offset: 0x0000F240
		public string[] Tags
		{
			get
			{
				return new string[]
				{
					"RWAutoSell.TagItems"
				};
			}
		}

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x060001BD RID: 445 RVA: 0x00011050 File Offset: 0x0000F250
		public Texture2D MiniGraphic
		{
			get
			{
				return ASTextures.FApparel;
			}
		}

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x060001BE RID: 446 RVA: 0x00011057 File Offset: 0x0000F257
		public string HelpPage
		{
			get
			{
				return "https://github.com/SupesSolutions/RWAutoSeller/wiki/Apparel-Filter";
			}
		}

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x060001BF RID: 447 RVA: 0x0001105E File Offset: 0x0000F25E
		public string Label
		{
			get
			{
				return "RWAutoSell.FilterApparel".Translate();
			}
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x00011070 File Offset: 0x0000F270
		public bool CheckItem(Transferable t, List<string> Data)
		{
			if (t.AnyThing.def.IsApparel)
			{
				Apparel apparel = t.AnyThing as Apparel;
				int num = 0;
				int num2 = 0;
				foreach (string text in Data)
				{
					string a = text.Substring(0, 4);
					if (a == "tnt." && int.TryParse(text.Substring(4), out num))
					{
						if (num == 1 && !apparel.WornByCorpse)
						{
							return false;
						}
						if (num == 2 && apparel.WornByCorpse)
						{
							return false;
						}
					}
					if (a == "frd." && int.TryParse(text.Substring(4), out num2) && num2 != 0)
					{
						float num3;
						if (t.ThingDef.useHitPoints)
						{
							num3 = Mathf.Min((float)apparel.HitPoints / (float)apparel.MaxHitPoints, 999f);
						}
						else
						{
							num3 = 1f;
						}
						if (num3 > 0.2f && num2 == 1)
						{
							return false;
						}
						if (num3 > 0.5f && num2 == 2)
						{
							return false;
						}
					}
				}
				return true;
			}
			return false;
		}

		// Token: 0x060001C1 RID: 449 RVA: 0x000111B4 File Offset: 0x0000F3B4
		public List<string> DataFromTradeable(Transferable t)
		{
			List<string> list = new List<string>();
			if (t.ThingDef.IsApparel)
			{
				Apparel apparel = t.AnyThing as Apparel;
				if (apparel.WornByCorpse)
				{
					list.Add("tnt.1");
				}
				else
				{
					list.Add("tnt.2");
				}
				if (t.ThingDef.useHitPoints)
				{
					float num = Mathf.Min((float)apparel.HitPoints / (float)apparel.MaxHitPoints, 999f);
					if (num < 0.2f)
					{
						list.Add("frd.1");
					}
					else if (num < 0.5f)
					{
						list.Add("frd.2");
					}
					else
					{
						list.Add("frd.0");
					}
				}
				else
				{
					list.Add("frd.0");
				}
			}
			return list;
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x0001126D File Offset: 0x0000F46D
		public List<string> DataOptions(List<string> Data, Map map)
		{
			return Data;
		}

		// Token: 0x060001C3 RID: 451 RVA: 0x00011270 File Offset: 0x0000F470
		public void DrawWidget(Rect inrect, List<string> Data)
		{
			if (this.AllowRender)
			{
				ASTable astable = new ASTable(new Rect(inrect.x, inrect.y, inrect.width, 230f), 1, 7, 4f, 0f, false);
				bool flag = this.IsTainted != 0;
				Widgets.CheckboxLabeled(astable.GetRectangle(0, 0, -1, -1, false), "RWAutoSell.FilterAppCkTainted".Translate(), ref flag, false, null, null, false, false);
				if (flag)
				{
					if (this.IsTainted == 0 | Widgets.RadioButtonLabeled(astable.GetRectangle(0, 1, -1, -1, false), "RWAutoSell.FilterAppIsTainted".Translate(), this.IsTainted == 1, false))
					{
						this.IsTainted = 1;
					}
					if (Widgets.RadioButtonLabeled(astable.GetRectangle(0, 2, -1, -1, false), "RWAutoSell.FilterAppNotTainted".Translate(), this.IsTainted == 2, false))
					{
						this.IsTainted = 2;
					}
				}
				else
				{
					this.IsTainted = 0;
				}
				flag = (this.IsTattered != 0);
				Widgets.CheckboxLabeled(astable.GetRectangle(0, 4, -1, -1, false), "RWAutoSell.FilterAppCkDamage".Translate(), ref flag, false, null, null, false, false);
				if (flag)
				{
					if (this.IsTattered == 0 | Widgets.RadioButtonLabeled(astable.GetRectangle(0, 5, -1, -1, false), "RWAutoSell.FilterAppIsTattered".Translate(), this.IsTattered == 1, false))
					{
						this.IsTattered = 1;
					}
					if (Widgets.RadioButtonLabeled(astable.GetRectangle(0, 6, -1, -1, false), "RWAutoSell.FilterAppIsFrayed".Translate(), this.IsTattered == 2, false))
					{
						this.IsTattered = 2;
						return;
					}
				}
				else
				{
					this.IsTattered = 0;
				}
			}
		}

		// Token: 0x060001C4 RID: 452 RVA: 0x00011410 File Offset: 0x0000F610
		public void RenderStarted(List<string> Data, Map map)
		{
			this.IsTainted = 0;
			this.IsTattered = 0;
			foreach (string text in Data)
			{
				string a = text.Substring(0, 4);
				if (a == "tnt.")
				{
					int.TryParse(text.Substring(4), out this.IsTainted);
				}
				else if (a == "frd.")
				{
					int.TryParse(text.Substring(4), out this.IsTattered);
				}
			}
			this.AllowRender = true;
		}

		// Token: 0x060001C5 RID: 453 RVA: 0x000114B8 File Offset: 0x0000F6B8
		public void RenderStopped(List<string> Data, Map map)
		{
			this.AllowRender = false;
			Data.Clear();
			Data.Add("tnt." + this.IsTainted.ToString());
			Data.Add("frd." + this.IsTattered.ToString());
		}

		// Token: 0x04000100 RID: 256
		private bool AllowRender;

		// Token: 0x04000101 RID: 257
		private int IsTainted;

		// Token: 0x04000102 RID: 258
		private int IsTattered;
	}
}
