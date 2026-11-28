using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RimWorld;
using RWASFilterLib;
using RWASWidgets;
using RWAutoSell.Mergers;
using UnityEngine;
using Verse;

namespace RWAutoSell.Filters
{
	// Token: 0x0200003A RID: 58
	[NullableContext(1)]
	[Nullable(0)]
	public class FilterArea : IBaseFilter, IRequiredMerger
	{
		// Token: 0x1700005F RID: 95
		// (get) Token: 0x060001C7 RID: 455 RVA: 0x00011510 File Offset: 0x0000F710
		public string Label
		{
			get
			{
				return "RWAutoSell.FilterArea".Translate();
			}
		}

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x060001C8 RID: 456 RVA: 0x00011521 File Offset: 0x0000F721
		public string[] Tags { get; } = new string[]
		{
			"RWAutoSell.TagItems",
			"RWAutoSell.TagFood",
			"RWAutoSell.TagAnimals",
			"RWAutoSell.TagPeople"
		};

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x060001C9 RID: 457 RVA: 0x00011529 File Offset: 0x0000F729
		public Texture2D MiniGraphic
		{
			get
			{
				return ASTextures.FArea;
			}
		}

		// Token: 0x060001CA RID: 458 RVA: 0x00011530 File Offset: 0x0000F730
		public void DrawWidget(Rect inrect, List<string> Data)
		{
			if (this.expanded)
			{
				this.transition = Mathf.Lerp(this.transition, inrect.width * 0.7f, Time.deltaTime * 2.5f);
			}
			else
			{
				this.transition = Mathf.Lerp(this.transition, 100f, Time.deltaTime * 2.5f);
			}
			float height = inrect.height - (this.transition + 10f);
			ASRectMonitor asrectMonitor = new ASRectMonitor(inrect);
			ASRectMonitor d = asrectMonitor.NextFromTop(height);
			int num = 0;
			int num2 = Data.FindIndex((string x) => x.Substring(0, 4) == "opt.");
			if (num2 == -1)
			{
				num = 1;
				num2 = Data.Count<string>();
				Data.Add("opt.1");
			}
			else if (!int.TryParse(Data[num2].Substring(4), out num))
			{
				Data[num2] = "opt.1";
				num = 1;
			}
			if (Widgets.RadioButtonLabeled(d.NextFromTop(24f), "RWAutoSell.FilterAreaRestricted".Translate(), num == 1, false))
			{
				Data[num2] = "opt.1";
			}
			if (Widgets.RadioButtonLabeled(d.NextFromTop(24f), "RWAutoSell.FilterAreaIn".Translate(), num == 2, false))
			{
				Data[num2] = "opt.2";
			}
			this.box.DrawListBox(d);
			asrectMonitor.NextFromTop(10f);
			float height2 = Mathf.Min(this.transition, inrect.width);
			Rect rect = asrectMonitor.NextFromTop(height2);
			if (Widgets.ButtonInvisible(rect, false))
			{
				this.expanded = !this.expanded;
			}
			Widgets.DrawTextureFitted(rect, this.material, 1f, 1f);
		}

		// Token: 0x060001CB RID: 459 RVA: 0x000116F8 File Offset: 0x0000F8F8
		private void Clicked(IGUIList<string> sender, GUIListEventArgs e)
		{
			int num = Mathf.NextPowerOfTwo(this.tempmap.info.Size.x);
			int num2 = Mathf.NextPowerOfTwo(this.tempmap.info.Size.z);
			int num3 = num * num2;
			for (int i = 0; i < num3; i++)
			{
				this.col[i] = this.colbase[i];
			}
			using (List<string>.Enumerator enumerator = sender.GetSelectedItems.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string s = enumerator.Current;
					Area area = this.tempmap.areaManager.AllAreas.FirstOrDefault((Area x) => "are." + x.ID.ToString() == s);
					if (area != null)
					{
						foreach (IntVec3 intVec in area.ActiveCells)
						{
							int num4 = intVec.z * num2 + intVec.x;
							this.col[num4] = area.Color;
						}
					}
				}
			}
			this.material.SetPixels32(this.col);
			this.material.Apply();
		}

		// Token: 0x060001CC RID: 460 RVA: 0x00011860 File Offset: 0x0000FA60
		public void RenderStarted(List<string> Data, Map map)
		{
			this.expanded = false;
			this.transition = 100f;
			if (this.solid == null)
			{
				this.solid = SolidColorMaterials.NewSolidColorTexture(Color.white);
			}
			if (this.material != null)
			{
				Object.DestroyImmediate(this.material, false);
			}
			this.material = new Texture2D(Mathf.NextPowerOfTwo(map.info.Size.x), Mathf.NextPowerOfTwo(map.info.Size.z), TextureFormat.ARGB32, false);
			int num = Mathf.NextPowerOfTwo(map.info.Size.x) * Mathf.NextPowerOfTwo(map.info.Size.z);
			int num2 = Mathf.NextPowerOfTwo(map.info.Size.x);
			this.col = new Color32[num];
			this.colbase = new Color32[num];
			for (int i = 0; i < num; i++)
			{
				this.col[i] = Color.black;
				this.colbase[i] = Color.black;
			}
			foreach (Building building in map.edificeGrid.InnerArray)
			{
				if (building != null && !building.Fogged())
				{
					bool flag = false;
					IntVec3[] adjacentCellsAround = GenAdj.AdjacentCellsAround;
					for (int k = 0; k < adjacentCellsAround.Length; k++)
					{
						if (adjacentCellsAround[k].GetEdifice(map) == null)
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						this.colbase[building.Position.z * num2 + building.Position.x] = Color.white;
					}
				}
			}
			this.tempmap = map;
			this.box = new ASMultiBox<string>(25f, 4f, true, true);
			bool selected = false;
			if (Data.Any((string x) => x == "spc.nul"))
			{
				selected = true;
			}
			string text = "Unrestricted".Translate() + " / " + "None".Translate();
			SelectedItem<string> item = new SelectedItem<string>
			{
				Item = "spc.nul",
				Selected = selected,
				surpresslabelupdate = true,
				color = Color.white,
				Icon = this.solid,
				tooltip = text,
				label = text
			};
			this.box.Add(item);
			using (IEnumerator<Area> enumerator = (from el in map.areaManager.AllAreas
			orderby el.GetType().Name
			orderby el.GetType() != typeof(Area_Home)
			select el).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Area z = enumerator.Current;
					bool selected2 = false;
					if (Data.Any((string x) => x == "are." + z.ID.ToString()))
					{
						selected2 = true;
					}
					SelectedItem<string> item2 = new SelectedItem<string>
					{
						Item = "are." + z.ID.ToString(),
						Selected = selected2,
						surpresslabelupdate = true,
						color = z.Color,
						Icon = this.solid,
						tooltip = z.Label,
						label = z.Label
					};
					this.box.Add(item2);
				}
			}
			this.box.OnClick += this.Clicked;
			this.Clicked(this.box, null);
		}

		// Token: 0x060001CD RID: 461 RVA: 0x00011C50 File Offset: 0x0000FE50
		public void RenderStopped(List<string> Data, Map map)
		{
			string item = Data.FirstOrFallback((string x) => x.Substring(0, 4) == "opt.", "opt.1");
			Data.Clear();
			Data.Add(item);
			foreach (string item2 in this.box.GetSelectedItems)
			{
				Data.Add(item2);
			}
			this.box.OnClick -= this.Clicked;
			this.box = null;
			this.tempmap = null;
			Object.DestroyImmediate(this.material, false);
			this.material = null;
			this.col = null;
			this.colbase = null;
		}

		// Token: 0x060001CE RID: 462 RVA: 0x00011D28 File Offset: 0x0000FF28
		public List<string> DataOptions(List<string> Data, Map map)
		{
			return Data;
		}

		// Token: 0x060001CF RID: 463 RVA: 0x00011D2C File Offset: 0x0000FF2C
		public bool CheckItem(Transferable t, List<string> Data)
		{
			if (t.HasAnyThing)
			{
				Map mapHeld = t.AnyThing.MapHeld;
				if (mapHeld != null)
				{
					Thing anyThing = t.AnyThing;
					Pawn p = anyThing as Pawn;
					if (p != null)
					{
						if (Data.Any((string x) => x == "opt.1") && p.Faction != null && p.Faction.IsPlayer)
						{
							if (p.playerSettings.AreaRestrictionInPawnCurrentMap == null)
							{
								if (Data.Any((string x) => x == "spc.nul"))
								{
									return true;
								}
								return false;
							}
							else
							{
								if (Data.Any((string x) => x == "are." + p.playerSettings.AreaRestrictionInPawnCurrentMap.ID.ToString()))
								{
									return true;
								}
								return false;
							}
						}
					}
					if (Data.Any((string x) => x == "opt.2"))
					{
						IntVec3 positionHeld = t.AnyThing.PositionHeld;
						if (positionHeld.InBounds(mapHeld))
						{
							using (List<Area>.Enumerator enumerator = mapHeld.areaManager.AllAreas.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									Area a = enumerator.Current;
									if (Data.Any((string x) => x == "are." + a.ID.ToString()) && a[positionHeld])
									{
										return true;
									}
								}
							}
							return false;
						}
					}
				}
			}
			return false;
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x00011ED8 File Offset: 0x000100D8
		public List<string> DataFromTradeable(Transferable t)
		{
			return new List<string>();
		}

		// Token: 0x060001D1 RID: 465 RVA: 0x00011EDF File Offset: 0x000100DF
		public string RequiredEnabled()
		{
			return typeof(AreaMerger).FullName;
		}

		// Token: 0x04000103 RID: 259
		private bool expanded;

		// Token: 0x04000104 RID: 260
		private float transition = 100f;

		// Token: 0x04000105 RID: 261
		private Texture2D material;

		// Token: 0x04000106 RID: 262
		private Texture2D solid;

		// Token: 0x04000107 RID: 263
		private Color32[] col;

		// Token: 0x04000108 RID: 264
		private Color32[] colbase;

		// Token: 0x04000109 RID: 265
		private Map tempmap;

		// Token: 0x0400010B RID: 267
		private ASMultiBox<string> box;
	}
}
