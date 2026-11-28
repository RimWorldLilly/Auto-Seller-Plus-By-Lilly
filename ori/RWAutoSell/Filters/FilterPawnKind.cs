using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RimWorld;
using RWASFilterLib;
using RWASWidgets;
using UnityEngine;
using Verse;

namespace RWAutoSell.Filters
{
	// Token: 0x02000046 RID: 70
	[NullableContext(1)]
	[Nullable(0)]
	public class FilterPawnKind : IBaseFilter, IWikiHelp, IRuleSorter
	{
		// Token: 0x1700008D RID: 141
		// (get) Token: 0x06000266 RID: 614 RVA: 0x00016692 File Offset: 0x00014892
		public Texture2D MiniGraphic
		{
			get
			{
				return ASTextures.FPKind;
			}
		}

		// Token: 0x06000267 RID: 615 RVA: 0x0001669C File Offset: 0x0001489C
		public int Compare<[Nullable(0)] T>(T First, T Second) where T : Transferable
		{
			Pawn pawn = First.AnyThing as Pawn;
			if (pawn != null)
			{
				Pawn pawn2 = Second.AnyThing as Pawn;
				if (pawn2 == null)
				{
					return -1;
				}
				if (pawn.RaceProps.Humanlike && !pawn2.RaceProps.Humanlike)
				{
					return -1;
				}
				if (pawn.RaceProps.Animal && pawn2.RaceProps.IsMechanoid)
				{
					return -1;
				}
				if (pawn.RaceProps.IsMechanoid && !pawn2.RaceProps.IsMechanoid)
				{
					return 1;
				}
				if (pawn.RaceProps.Humanlike && pawn2.RaceProps.Humanlike)
				{
					return pawn.Name.ToStringFull.CompareTo(pawn2.Name.ToStringFull);
				}
				return pawn.kindDef.label.CompareTo(pawn2.kindDef.label);
			}
			else
			{
				if (Second.AnyThing is Pawn)
				{
					return 1;
				}
				return 0;
			}
		}

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x06000268 RID: 616 RVA: 0x00016794 File Offset: 0x00014994
		public string HelpPage
		{
			get
			{
				return "https://github.com/SupesSolutions/RWAutoSeller/wiki/Race-Filter";
			}
		}

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x06000269 RID: 617 RVA: 0x0001679B File Offset: 0x0001499B
		public string Label
		{
			get
			{
				return "RWAutoSell.FilterRace".Translate();
			}
		}

		// Token: 0x0600026A RID: 618 RVA: 0x000167AC File Offset: 0x000149AC
		public bool CheckItem(Transferable t, List<string> Data)
		{
			Pawn pawn = t.AnyThing as Pawn;
			if (pawn != null)
			{
				string b = "pkd." + pawn.kindDef.defName;
				string b2 = "";
				if (pawn.kindDef.race != null)
				{
					b2 = "rac." + pawn.kindDef.race.defName;
				}
				foreach (string text in Data)
				{
					string a = text.Substring(0, 4);
					if (a == "pkd.")
					{
						if (text == b)
						{
							return true;
						}
					}
					else if (a == "rac.")
					{
						if (text == b2)
						{
							return true;
						}
					}
					else if (a == "any.")
					{
						string a2 = text.Substring(4);
						if (!(a2 == "hmd"))
						{
							if (!(a2 == "anm"))
							{
								if (a2 == "mch")
								{
									if (pawn.RaceProps.IsMechanoid)
									{
										return true;
									}
								}
							}
							else if (pawn.RaceProps.Animal)
							{
								return true;
							}
						}
						else if (pawn.RaceProps.Humanlike)
						{
							return true;
						}
					}
				}
				return false;
			}
			return false;
		}

		// Token: 0x0600026B RID: 619 RVA: 0x00016920 File Offset: 0x00014B20
		public List<string> DataFromTradeable(Transferable t)
		{
			List<string> list = new List<string>();
			Pawn pawn = t.AnyThing as Pawn;
			if (pawn != null)
			{
				list.Add("pkd." + pawn.kindDef.defName);
			}
			return list;
		}

		// Token: 0x0600026C RID: 620 RVA: 0x0001695E File Offset: 0x00014B5E
		public List<string> DataOptions(List<string> Data, Map map)
		{
			return Data;
		}

		// Token: 0x0600026D RID: 621 RVA: 0x00016964 File Offset: 0x00014B64
		public void DrawWidget(Rect inrect, List<string> Data)
		{
			ASRectMonitor asrectMonitor = new ASRectMonitor(inrect);
			if (this.AllowRender)
			{
				this.PawnBox.DrawListBox(asrectMonitor.Remaining());
			}
			if (this.dopost)
			{
				this.DoPost();
			}
		}

		// Token: 0x0600026E RID: 622 RVA: 0x000169A4 File Offset: 0x00014BA4
		private void InitBox(List<string> Data)
		{
			this.PawnBox = new ASMultiBox<string>(25f, 2f, false, true)
			{
				new SelectedItem<string>
				{
					Item = "any.hmd",
					label = "RWAutoSell.FilterRaceHumi".Translate(),
					Selected = Data.Contains("any.hmd")
				},
				new SelectedItem<string>
				{
					Item = "any.anm",
					label = "RWAutoSell.FilterRaceAnm".Translate(),
					Selected = Data.Contains("any.anm")
				},
				new SelectedItem<string>
				{
					Item = "any.mch",
					label = "RWAutoSell.FilterRaceMech".Translate(),
					Selected = Data.Contains("any.mch")
				}
			};
			this.PawnBox.OnClick += this.PostChange;
		}

		// Token: 0x0600026F RID: 623 RVA: 0x00016A93 File Offset: 0x00014C93
		private void PostChange(IGUIList<string> sender, GUIListEventArgs e)
		{
			this.dopost = true;
		}

		// Token: 0x06000270 RID: 624 RVA: 0x00016A9C File Offset: 0x00014C9C
		private void DoPost()
		{
			if (this.PawnBox[0].Selected || this.PawnBox[1].Selected || this.PawnBox[2].Selected)
			{
				for (int i = this.PawnBox.Count - 1; i >= 2; i--)
				{
					SelectedItem<string> item = this.PawnBox[i];
					if (this.hmd.Contains(item))
					{
						this.PawnBox.RemoveAt(i);
					}
					else if (this.anm.Contains(item))
					{
						this.PawnBox.RemoveAt(i);
					}
					else if (this.mch.Contains(item))
					{
						this.PawnBox.RemoveAt(i);
					}
				}
			}
			List<SelectedItem<string>> list = new List<SelectedItem<string>>();
			list.Add(this.PawnBox[0]);
			list.Add(this.PawnBox[1]);
			list.Add(this.PawnBox[2]);
			if (!this.PawnBox[0].Selected)
			{
				for (int j = 0; j < this.hmd.Count; j++)
				{
					list.Add(this.hmd[j]);
				}
			}
			if (!this.PawnBox[1].Selected)
			{
				for (int k = 0; k < this.anm.Count; k++)
				{
					list.Add(this.anm[k]);
				}
			}
			if (!this.PawnBox[2].Selected)
			{
				for (int l = 0; l < this.mch.Count; l++)
				{
					list.Add(this.mch[l]);
				}
			}
			this.PawnBox.Clear();
			foreach (SelectedItem<string> item2 in list)
			{
				this.PawnBox.Add(item2);
			}
			this.dopost = false;
		}

		// Token: 0x06000271 RID: 625 RVA: 0x00016CAC File Offset: 0x00014EAC
		private void InitPK(List<string> Data)
		{
			this.hmd.Clear();
			this.anm.Clear();
			this.mch.Clear();
			this.DolistPK(Data, (from el in DefDatabase<PawnKindDef>.AllDefs
			where el.RaceProps.Humanlike
			orderby el.label
			select el).ThenBy(delegate(PawnKindDef el)
			{
				ThingDef race = el.race;
				if (race == null)
				{
					return null;
				}
				return race.defName;
			}), this.hmd, Data.Contains("any.hmd"));
			this.DolistPK(Data, (from el in DefDatabase<PawnKindDef>.AllDefs
			where !el.RaceProps.Humanlike && !el.RaceProps.Insect && !el.RaceProps.IsMechanoid
			orderby el.label
			select el).ThenBy(delegate(PawnKindDef el)
			{
				ThingDef race = el.race;
				if (race == null)
				{
					return null;
				}
				return race.defName;
			}), this.anm, Data.Contains("any.anm"));
			this.DolistPK(Data, (from el in DefDatabase<PawnKindDef>.AllDefs
			where !el.RaceProps.Humanlike && el.RaceProps.Insect && !el.RaceProps.IsMechanoid
			orderby el.label
			select el).ThenBy(delegate(PawnKindDef el)
			{
				ThingDef race = el.race;
				if (race == null)
				{
					return null;
				}
				return race.defName;
			}), this.anm, Data.Contains("any.anm"));
			this.DolistPK(Data, (from el in DefDatabase<PawnKindDef>.AllDefs
			where !el.RaceProps.Humanlike && el.RaceProps.IsMechanoid
			orderby el.label
			select el).ThenBy(delegate(PawnKindDef el)
			{
				ThingDef race = el.race;
				if (race == null)
				{
					return null;
				}
				return race.defName;
			}), this.mch, Data.Contains("any.mch"));
		}

		// Token: 0x06000272 RID: 626 RVA: 0x00016EFE File Offset: 0x000150FE
		public void RenderStarted(List<string> Data, Map map)
		{
			this.hmd = new List<SelectedItem<string>>();
			this.anm = new List<SelectedItem<string>>();
			this.mch = new List<SelectedItem<string>>();
			this.InitBox(Data);
			this.InitPK(Data);
			this.AllowRender = true;
		}

		// Token: 0x06000273 RID: 627 RVA: 0x00016F38 File Offset: 0x00015138
		private void DolistPK(List<string> Data, IEnumerable<PawnKindDef> defs, List<SelectedItem<string>> cache, bool cacheonly = false)
		{
			foreach (PawnKindDef pawnKindDef in defs)
			{
				Color color = Color.white;
				Texture2D icon;
				try
				{
					GraphicData bodyGraphicData = pawnKindDef.lifeStages.Last<PawnKindLifeStage>().bodyGraphicData;
					icon = (Texture2D)bodyGraphicData.Graphic.MatAt(Rot4.East, null).mainTexture;
					color = bodyGraphicData.color;
				}
				catch
				{
					icon = null;
				}
				string templabl = pawnKindDef.LabelCap;
				if (defs.Count((PawnKindDef x) => x.LabelCap == templabl) > 1)
				{
					templabl = pawnKindDef.LabelCap + " (" + pawnKindDef.defName + ")";
				}
				SelectedItem<string> item = new SelectedItem<string>
				{
					Item = "pkd." + pawnKindDef.defName,
					color = color,
					label = templabl,
					Icon = icon,
					Selected = Data.Contains("pkd." + pawnKindDef.defName)
				};
				if (!cacheonly)
				{
					this.PawnBox.Add(item);
				}
				cache.Add(item);
			}
		}

		// Token: 0x06000274 RID: 628 RVA: 0x000170B0 File Offset: 0x000152B0
		public void RenderStopped(List<string> Data, Map map)
		{
			this.AllowRender = false;
			Data.Clear();
			Data.AddRange(this.PawnBox.GetSelectedItems);
			this.PawnBox.Clear();
			this.PawnBox.OnClick -= this.PostChange;
			this.PawnBox = null;
			this.hmd = null;
			this.anm = null;
			this.mch = null;
		}

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x06000275 RID: 629 RVA: 0x00017119 File Offset: 0x00015319
		public string[] Tags { get; } = new string[]
		{
			"RWAutoSell.TagAnimals",
			"RWAutoSell.TagPeople"
		};

		// Token: 0x04000146 RID: 326
		private bool AllowRender;

		// Token: 0x04000147 RID: 327
		private bool dopost;

		// Token: 0x04000148 RID: 328
		private List<SelectedItem<string>> hmd = new List<SelectedItem<string>>();

		// Token: 0x04000149 RID: 329
		private List<SelectedItem<string>> anm = new List<SelectedItem<string>>();

		// Token: 0x0400014A RID: 330
		private List<SelectedItem<string>> mch = new List<SelectedItem<string>>();

		// Token: 0x0400014B RID: 331
		private ASMultiBox<string> PawnBox;
	}
}
