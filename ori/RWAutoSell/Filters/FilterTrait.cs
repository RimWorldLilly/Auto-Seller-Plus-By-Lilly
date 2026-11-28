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
	// Token: 0x02000053 RID: 83
	[NullableContext(1)]
	[Nullable(0)]
	public class FilterTrait : IBaseFilter, IWikiHelp
	{
		// Token: 0x170000BC RID: 188
		// (get) Token: 0x06000301 RID: 769 RVA: 0x0001C5D6 File Offset: 0x0001A7D6
		public Texture2D MiniGraphic
		{
			get
			{
				return ASTextures.FTrait;
			}
		}

		// Token: 0x170000BD RID: 189
		// (get) Token: 0x06000302 RID: 770 RVA: 0x0001C5DD File Offset: 0x0001A7DD
		public string HelpPage
		{
			get
			{
				return "https://github.com/SupesSolutions/RWAutoSeller/wiki/Trait-Filter";
			}
		}

		// Token: 0x170000BE RID: 190
		// (get) Token: 0x06000303 RID: 771 RVA: 0x0001C5E4 File Offset: 0x0001A7E4
		public string Label
		{
			get
			{
				return "RWAutoSell.FilterTrait".Translate();
			}
		}

		// Token: 0x06000304 RID: 772 RVA: 0x0001C5F8 File Offset: 0x0001A7F8
		public bool CheckItem(Transferable t, List<string> Data)
		{
			Pawn pawn = t.AnyThing as Pawn;
			if (pawn != null)
			{
				foreach (string a in Data)
				{
					Pawn_StoryTracker story = pawn.story;
					bool flag;
					if (story == null)
					{
						flag = (null != null);
					}
					else
					{
						TraitSet traits = story.traits;
						flag = (((traits != null) ? traits.allTraits : null) != null);
					}
					if (flag)
					{
						foreach (Trait trait in pawn.story.traits.allTraits)
						{
							string b = "dgr." + trait.Degree.ToString() + ".trt." + trait.def.defName;
							if (a == b)
							{
								return true;
							}
						}
					}
				}
				return false;
			}
			return false;
		}

		// Token: 0x06000305 RID: 773 RVA: 0x0001C700 File Offset: 0x0001A900
		public List<string> DataFromTradeable(Transferable t)
		{
			List<string> list = new List<string>();
			Pawn pawn = t.AnyThing as Pawn;
			if (pawn != null && pawn.RaceProps.Humanlike)
			{
				foreach (Trait trait in pawn.story.traits.allTraits)
				{
					list.Add("dgr." + trait.Degree.ToString() + ".trt." + trait.def.defName);
				}
			}
			return list;
		}

		// Token: 0x06000306 RID: 774 RVA: 0x0001C7A8 File Offset: 0x0001A9A8
		public List<string> DataOptions(List<string> Data, Map map)
		{
			return Data;
		}

		// Token: 0x06000307 RID: 775 RVA: 0x0001C7AB File Offset: 0x0001A9AB
		public void DrawWidget(Rect inrect, List<string> Data)
		{
			if (this.AllowRender)
			{
				this.TraitBox.DrawListBox(inrect);
			}
		}

		// Token: 0x06000308 RID: 776 RVA: 0x0001C7C4 File Offset: 0x0001A9C4
		public void RenderStarted(List<string> Data, Map map)
		{
			this.TraitBox = new ASMultiBox<string>(25f, 2f, true, false);
			foreach (TraitDef traitDef in DefDatabase<TraitDef>.AllDefs)
			{
				if (traitDef.degreeDatas.Count != 0)
				{
					using (List<TraitDegreeData>.Enumerator enumerator2 = traitDef.degreeDatas.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							TraitDegreeData traitDegreeData = enumerator2.Current;
							string item = "dgr." + traitDegreeData.degree.ToString() + ".trt." + traitDef.defName;
							SelectedItem<string> item2 = new SelectedItem<string>
							{
								Item = item,
								label = traitDegreeData.label.CapitalizeFirst(),
								Selected = Data.Contains(item)
							};
							this.TraitBox.Add(item2);
						}
						continue;
					}
				}
				string item3 = "dgr." + traitDef.degreeDatas[0].ToString() + "trt." + traitDef.defName;
				SelectedItem<string> item4 = new SelectedItem<string>
				{
					Item = item3,
					label = traitDef.degreeDatas[0].label.CapitalizeFirst(),
					Selected = Data.Contains(item3)
				};
				this.TraitBox.Add(item4);
			}
			this.AllowRender = true;
		}

		// Token: 0x06000309 RID: 777 RVA: 0x0001C95C File Offset: 0x0001AB5C
		public void RenderStopped(List<string> Data, Map map)
		{
			this.AllowRender = false;
			Data.Clear();
			Data.AddRange(this.TraitBox.GetSelectedItems);
			this.TraitBox.Clear();
			this.TraitBox = null;
		}

		// Token: 0x170000BF RID: 191
		// (get) Token: 0x0600030A RID: 778 RVA: 0x0001C98E File Offset: 0x0001AB8E
		public string[] Tags { get; } = new string[]
		{
			"RWAutoSell.TagPeople"
		};

		// Token: 0x04000192 RID: 402
		private bool AllowRender;

		// Token: 0x04000193 RID: 403
		private ASMultiBox<string> TraitBox;
	}
}
