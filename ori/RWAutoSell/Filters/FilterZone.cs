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
	// Token: 0x02000057 RID: 87
	[NullableContext(1)]
	[Nullable(0)]
	public class FilterZone : IBaseFilter, IRequiredMerger
	{
		// Token: 0x170000CB RID: 203
		// (get) Token: 0x0600032D RID: 813 RVA: 0x0001D508 File Offset: 0x0001B708
		public string Label
		{
			get
			{
				return "RWAutoSell.FilterZone".Translate();
			}
		}

		// Token: 0x170000CC RID: 204
		// (get) Token: 0x0600032E RID: 814 RVA: 0x0001D519 File Offset: 0x0001B719
		public string[] Tags { get; } = new string[]
		{
			"RWAutoSell.TagItems",
			"RWAutoSell.TagFood"
		};

		// Token: 0x170000CD RID: 205
		// (get) Token: 0x0600032F RID: 815 RVA: 0x0001D521 File Offset: 0x0001B721
		public Texture2D MiniGraphic
		{
			get
			{
				return ASTextures.FZone;
			}
		}

		// Token: 0x06000330 RID: 816 RVA: 0x0001D528 File Offset: 0x0001B728
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
			this.box.DrawListBox(asrectMonitor.NextFromTop(height));
			asrectMonitor.NextFromTop(10f);
			float height2 = Mathf.Min(100f, inrect.width);
			Rect rect = asrectMonitor.NextFromTop(height2);
			if (Widgets.ButtonInvisible(rect, false))
			{
				this.expanded = !this.expanded;
			}
			if (this.material != null)
			{
				Widgets.DrawTextureFitted(rect, this.material, 1f, 1f);
			}
		}

		// Token: 0x06000331 RID: 817 RVA: 0x0001D620 File Offset: 0x0001B820
		private string GrowingZoneName(Zone_Growing gz, string tt)
		{
			string text;
			if (tt == null || tt.Trim() == string.Empty)
			{
				text = "GrowingZone".Translate();
			}
			else
			{
				text = tt;
			}
			ThingDef thingDef = null;
			try
			{
				thingDef = gz.GetPlantDefToGrow();
			}
			finally
			{
				if (thingDef != null)
				{
					text = text + "(" + thingDef.LabelCap + ")";
				}
			}
			return text;
		}

		// Token: 0x06000332 RID: 818 RVA: 0x0001D69C File Offset: 0x0001B89C
		private void Clicked(IGUIList<string> sender, GUIListEventArgs e)
		{
			int num = Mathf.NextPowerOfTwo(this.tempmap.info.Size.x);
			int num2 = Mathf.NextPowerOfTwo(this.tempmap.info.Size.z);
			int num3 = num * num2;
			for (int i = 0; i < num3; i++)
			{
				this.col[i] = this.colbase[i];
			}
			try
			{
				using (List<string>.Enumerator enumerator = sender.GetSelectedItems.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string s = enumerator.Current;
						Zone zone = null;
						try
						{
							zone = this.tempmap.zoneManager.AllZones.FirstOrDefault((Zone x) => "znn." + x.ID.ToString() == s);
						}
						catch
						{
						}
						if (zone != null)
						{
							foreach (IntVec3 intVec in zone.cells)
							{
								int num4 = intVec.z * num + intVec.x;
								if (num4 < this.col.Length && num4 > -1)
								{
									this.col[num4] = zone.color;
								}
							}
						}
					}
				}
			}
			catch
			{
				Log.Warning("RWAutoSell: Filter Zone, Render start, click update failure");
			}
			if (this.material != null)
			{
				this.material.SetPixels32(this.col);
				this.material.Apply();
			}
		}

		// Token: 0x06000333 RID: 819 RVA: 0x0001D858 File Offset: 0x0001BA58
		public void RenderStarted(List<string> Data, Map map)
		{
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
			bool flag = false;
			string a = "";
			foreach (Building building in map.edificeGrid.InnerArray)
			{
				if (building != null && !building.Fogged())
				{
					bool flag2 = false;
					try
					{
						IntVec3[] adjacentCellsAround = GenAdj.AdjacentCellsAround;
						for (int k = 0; k < adjacentCellsAround.Length; k++)
						{
							if (adjacentCellsAround[k].GetEdifice(map) == null)
							{
								flag2 = true;
								break;
							}
						}
						if (flag2)
						{
							int num3 = building.Position.z * num2 + building.Position.x;
							if (num3 < this.colbase.Length && num3 > -1)
							{
								this.colbase[num3] = Color.white;
							}
							else
							{
								flag = true;
								a = "outside range";
							}
						}
					}
					catch
					{
						flag = true;
					}
				}
			}
			if (flag)
			{
				if (a == "outside range")
				{
					string str = "RWAutoSell: Filter Zone, Render start, map preview texture prep failure - outside range ";
					IntVec3 size = map.info.Size;
					string str2 = size.x.ToString();
					string str3 = " ";
					size = map.info.Size;
					Log.Warning(str + str2 + str3 + size.z.ToString());
				}
				else
				{
					Log.Error("RWAutoSell: Filter Zone, Render start, map preview texture prep failure");
				}
			}
			this.tempmap = map;
			this.box = new ASMultiBox<string>(25f, 4f, true, true);
			bool selected = false;
			if (Data.Any((string x) => x == "spc.nul"))
			{
				selected = true;
			}
			SelectedItem<string> item = new SelectedItem<string>
			{
				Item = "spc.nul",
				Selected = selected,
				surpresslabelupdate = true,
				color = Color.white,
				Icon = this.solid,
				tooltip = "None".Translate(),
				label = "None".Translate()
			};
			this.box.Add(item);
			List<Zone> list = new List<Zone>();
			foreach (Zone zone in map.zoneManager.AllZones)
			{
				try
				{
					if (zone.GetType().Name != null)
					{
						string baseLabel = zone.BaseLabel;
						list.Add(zone);
					}
				}
				catch
				{
				}
			}
			try
			{
				list = (from el in list
				orderby el.GetType().Name
				orderby el.label
				select el).ToList<Zone>();
			}
			catch
			{
			}
			using (List<Zone>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Zone z = enumerator.Current;
					bool selected2 = false;
					if (Data.Any((string x) => x == "znn." + z.ID.ToString()))
					{
						selected2 = true;
					}
					string text = z.BaseLabel;
					Zone_Growing zone_Growing = z as Zone_Growing;
					if (zone_Growing != null)
					{
						text = this.GrowingZoneName(zone_Growing, text);
					}
					else if ((text == null || text.Trim() == string.Empty) && z is Zone_Stockpile)
					{
						text = "Stockpile".Translate() + "/" + "DumpingStockpile".Translate();
					}
					SelectedItem<string> item2 = new SelectedItem<string>
					{
						Item = "znn." + z.ID.ToString(),
						Selected = selected2,
						surpresslabelupdate = true,
						color = z.color,
						Icon = this.solid,
						tooltip = text,
						label = z.label
					};
					this.box.Add(item2);
				}
			}
			this.box.OnClick += this.Clicked;
			this.Clicked(this.box, null);
		}

		// Token: 0x06000334 RID: 820 RVA: 0x0001DE08 File Offset: 0x0001C008
		public void RenderStopped(List<string> Data, Map map)
		{
			Data.Clear();
			foreach (string item in this.box.GetSelectedItems)
			{
				Data.Add(item);
			}
			this.box.OnClick -= this.Clicked;
			this.box = null;
			this.tempmap = null;
			Object.DestroyImmediate(this.material, false);
			this.material = null;
			this.col = null;
			this.colbase = null;
		}

		// Token: 0x06000335 RID: 821 RVA: 0x0001DEAC File Offset: 0x0001C0AC
		public List<string> DataOptions(List<string> Data, Map map)
		{
			return Data;
		}

		// Token: 0x06000336 RID: 822 RVA: 0x0001DEB0 File Offset: 0x0001C0B0
		public List<string> DataFromTradeable(Transferable t)
		{
			List<string> list = new List<string>();
			if (t.HasAnyThing)
			{
				Map mapHeld = t.AnyThing.MapHeld;
				if (mapHeld != null && t.AnyThing.PositionHeld.InBounds(mapHeld))
				{
					Zone zone = mapHeld.zoneManager.ZoneAt(t.AnyThing.PositionHeld);
					if (zone != null)
					{
						list.Add("znn." + zone.ID.ToString());
					}
				}
			}
			return list;
		}

		// Token: 0x06000337 RID: 823 RVA: 0x0001DF23 File Offset: 0x0001C123
		public string RequiredEnabled()
		{
			return "ZoneMerger";
		}

		// Token: 0x06000338 RID: 824 RVA: 0x0001DF2C File Offset: 0x0001C12C
		public bool CheckItem(Transferable t, List<string> Data)
		{
			Map mapHeld = t.AnyThing.MapHeld;
			if (mapHeld != null && t.AnyThing.PositionHeld.InBounds(mapHeld))
			{
				Zone z = mapHeld.zoneManager.ZoneAt(t.AnyThing.PositionHeld);
				if (z != null && Data.Any((string x) => x == "znn." + z.ID.ToString()))
				{
					return true;
				}
				if (z == null)
				{
					if (Data.Any((string x) => x == "spc.nul"))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0400019F RID: 415
		private bool expanded;

		// Token: 0x040001A0 RID: 416
		private float transition = 100f;

		// Token: 0x040001A1 RID: 417
		private Texture2D material;

		// Token: 0x040001A2 RID: 418
		private Texture2D solid;

		// Token: 0x040001A3 RID: 419
		private Color32[] col;

		// Token: 0x040001A4 RID: 420
		private Color32[] colbase;

		// Token: 0x040001A5 RID: 421
		private Map tempmap;

		// Token: 0x040001A7 RID: 423
		private ASMultiBox<string> box;
	}
}
