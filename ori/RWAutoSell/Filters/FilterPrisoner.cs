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
	// Token: 0x02000049 RID: 73
	[NullableContext(1)]
	[Nullable(0)]
	public class FilterPrisoner : IBaseFilter
	{
		// Token: 0x17000095 RID: 149
		// (get) Token: 0x06000285 RID: 645 RVA: 0x00017DFF File Offset: 0x00015FFF
		public Texture2D MiniGraphic
		{
			get
			{
				return ASTextures.FPrisoner;
			}
		}

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x06000286 RID: 646 RVA: 0x00017E06 File Offset: 0x00016006
		public string[] Tags { get; } = new string[]
		{
			"RWAutoSell.TagPeople"
		};

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x06000287 RID: 647 RVA: 0x00017E0E File Offset: 0x0001600E
		public string Label
		{
			get
			{
				return "Prisoner".Translate();
			}
		}

		// Token: 0x06000288 RID: 648 RVA: 0x00017E20 File Offset: 0x00016020
		public bool CheckItem(Transferable t, List<string> Data)
		{
			Thing anyThing = t.AnyThing;
			Pawn p = anyThing as Pawn;
			if (p != null && p.RaceProps.Humanlike)
			{
				if (p.IsSlaveOfColony)
				{
					if (!Data.Any((string x) => x == "sim." + p.guest.slaveInteractionMode.defName))
					{
						if (!Data.Any((string x) => x == "sim.null"))
						{
							goto IL_83;
						}
					}
					return true;
				}
				IL_83:
				if (p.IsPrisonerOfColony)
				{
					if (!Data.Any((string x) => x == "pim." + p.guest.ExclusiveInteractionMode.defName))
					{
						if (!Data.Any((string x) => x == "pim.null"))
						{
							return false;
						}
					}
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000289 RID: 649 RVA: 0x00017EFB File Offset: 0x000160FB
		public List<string> DataFromTradeable(Transferable t)
		{
			return new List<string>();
		}

		// Token: 0x0600028A RID: 650 RVA: 0x00017F02 File Offset: 0x00016102
		public List<string> DataOptions(List<string> Data, Map map)
		{
			return Data;
		}

		// Token: 0x0600028B RID: 651 RVA: 0x00017F08 File Offset: 0x00016108
		public void RenderStarted(List<string> Data, Map map)
		{
			this.sld = DefDatabase<SlaveInteractionModeDef>.AllDefs.ToList<SlaveInteractionModeDef>();
			this.pld = DefDatabase<PrisonerInteractionModeDef>.AllDefs.ToList<PrisonerInteractionModeDef>();
			this.box = new ASMultiBox<Def>(30f, 5f, true, false);
			if (Data.Count == 0)
			{
				Data.Add("typ.prs");
			}
			if (Data.Any((string x) => x == "typ.slv"))
			{
				this.typ = true;
				using (List<SlaveInteractionModeDef>.Enumerator enumerator = this.sld.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Def d = enumerator.Current;
						SelectedItem<Def> item = new SelectedItem<Def>
						{
							Item = d,
							Selected = Data.Any((string x) => x == "sim." + d.defName)
						};
						this.box.Add(item);
					}
					return;
				}
			}
			this.typ = false;
			using (List<PrisonerInteractionModeDef>.Enumerator enumerator2 = this.pld.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					Def d = enumerator2.Current;
					SelectedItem<Def> item2 = new SelectedItem<Def>
					{
						Item = d,
						Selected = Data.Any((string x) => x == "pim." + d.defName)
					};
					this.box.Add(item2);
				}
			}
		}

		// Token: 0x0600028C RID: 652 RVA: 0x0001809C File Offset: 0x0001629C
		public void RenderStopped(List<string> Data, Map map)
		{
			Data.Clear();
			int num = 0;
			if (this.typ)
			{
				Data.Add("typ.slv");
				foreach (Def def in this.box.GetSelectedItems)
				{
					Data.Add("sim." + def.defName);
					num++;
				}
				if (num == 0)
				{
					Data.Add("sim.null");
				}
			}
			else
			{
				Data.Add("typ.prs");
				foreach (Def def2 in this.box.GetSelectedItems)
				{
					Data.Add("pim." + def2.defName);
					num++;
				}
				if (num == 0)
				{
					Data.Add("pim.null");
				}
			}
			this.box = null;
		}

		// Token: 0x0600028D RID: 653 RVA: 0x000181AC File Offset: 0x000163AC
		public void DrawWidget(Rect inrect, List<string> Data)
		{
			ASRectMonitor asrectMonitor = new ASRectMonitor(inrect);
			if (Widgets.RadioButtonLabeled(asrectMonitor.NextFromTop(30f), "Prisoner".Translate(), !this.typ, false))
			{
				this.typ = false;
				this.box.Clear();
				using (List<PrisonerInteractionModeDef>.Enumerator enumerator = this.pld.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Def d = enumerator.Current;
						this.box.Add(new SelectedItem<Def>
						{
							Item = d,
							Selected = Data.Any((string x) => x == "pim." + d.defName)
						});
					}
				}
			}
			if (Widgets.RadioButtonLabeled(asrectMonitor.NextFromTop(30f), "Slave".Translate(), this.typ, false))
			{
				this.typ = true;
				this.box.Clear();
				using (List<SlaveInteractionModeDef>.Enumerator enumerator2 = this.sld.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Def d = enumerator2.Current;
						this.box.Add(new SelectedItem<Def>
						{
							Item = d,
							Selected = Data.Any((string x) => x == "sim." + d.defName)
						});
					}
				}
			}
			this.box.DrawListBox(asrectMonitor.Remaining());
		}

		// Token: 0x04000157 RID: 343
		private List<SlaveInteractionModeDef> sld;

		// Token: 0x04000158 RID: 344
		private List<PrisonerInteractionModeDef> pld;

		// Token: 0x04000159 RID: 345
		private ASMultiBox<Def> box;

		// Token: 0x0400015A RID: 346
		private bool typ;
	}
}
