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
	// Token: 0x02000055 RID: 85
	[NullableContext(1)]
	[Nullable(0)]
	public class FilterWeapon : IBaseFilter
	{
		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x06000318 RID: 792 RVA: 0x0001CCE4 File Offset: 0x0001AEE4
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

		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x06000319 RID: 793 RVA: 0x0001CCF4 File Offset: 0x0001AEF4
		public Texture2D MiniGraphic
		{
			get
			{
				return ASTextures.FWeapon;
			}
		}

		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x0600031A RID: 794 RVA: 0x0001CCFB File Offset: 0x0001AEFB
		public string Label
		{
			get
			{
				return "RWAutoSell.FilterWeapon".Translate();
			}
		}

		// Token: 0x0600031B RID: 795 RVA: 0x0001CD0C File Offset: 0x0001AF0C
		public bool CheckItem(Transferable t, List<string> Data)
		{
			if (t.AnyThing.def.IsWeapon)
			{
				Thing anyThing = t.AnyThing;
				bool flag = CompBiocodable.IsBiocoded(anyThing);
				int num = 0;
				int num2 = 0;
				foreach (string text in Data)
				{
					string a = text.Substring(0, 4);
					if (text == "cat.1")
					{
						if (anyThing.def.thingCategories == null)
						{
							return false;
						}
						bool flag2 = false;
						if (anyThing.def.thingCategories.Any((ThingCategoryDef x) => ThingCategoryDefOf.Weapons.ThisAndChildCategoryDefs.Contains(x)))
						{
							flag2 = true;
						}
						if (!flag2)
						{
							return false;
						}
					}
					if (a == "bio." && int.TryParse(text.Substring(4), out num))
					{
						if (num == 1 && !flag)
						{
							return false;
						}
						if (num == 2 && flag)
						{
							return false;
						}
					}
					if (a == "wep." && int.TryParse(text.Substring(4), out num2))
					{
						if (num2 == 1 && !t.AnyThing.def.IsRangedWeapon)
						{
							return false;
						}
						if (num2 == 2 && !t.AnyThing.def.IsMeleeWeapon)
						{
							return false;
						}
					}
				}
				return true;
			}
			return false;
		}

		// Token: 0x0600031C RID: 796 RVA: 0x0001CE98 File Offset: 0x0001B098
		public List<string> DataFromTradeable(Transferable t)
		{
			List<string> list = new List<string>();
			if (t.ThingDef.IsWeapon)
			{
				if (CompBiocodable.IsBiocoded(t.AnyThing))
				{
					list.Add("bio.1");
				}
				else
				{
					list.Add("bio.2");
				}
				if (t.ThingDef.IsRangedWeapon)
				{
					list.Add("wep.1");
				}
				else if (t.ThingDef.IsMeleeWeapon)
				{
					list.Add("wep.2");
				}
			}
			return list;
		}

		// Token: 0x0600031D RID: 797 RVA: 0x0001CF10 File Offset: 0x0001B110
		public List<string> DataOptions(List<string> Data, Map map)
		{
			return Data;
		}

		// Token: 0x0600031E RID: 798 RVA: 0x0001CF14 File Offset: 0x0001B114
		public void RenderStarted(List<string> Data, Map map)
		{
			this.IsBioCoded = 0;
			this.WeaponType = 0;
			foreach (string text in Data)
			{
				string a = text.Substring(0, 4);
				if (a == "bio.")
				{
					int.TryParse(text.Substring(4), out this.IsBioCoded);
				}
				else if (a == "wep.")
				{
					int.TryParse(text.Substring(4), out this.WeaponType);
				}
				else if (a == "cat.")
				{
					int.TryParse(text.Substring(4), out this.weapcatonly);
				}
			}
			this.AllowRender = true;
		}

		// Token: 0x0600031F RID: 799 RVA: 0x0001CFE0 File Offset: 0x0001B1E0
		public void RenderStopped(List<string> Data, Map map)
		{
			this.AllowRender = false;
			Data.Clear();
			Data.Add("bio." + this.IsBioCoded.ToString());
			Data.Add("wep." + this.WeaponType.ToString());
			Data.Add("cat." + this.weapcatonly.ToString());
		}

		// Token: 0x06000320 RID: 800 RVA: 0x0001D04C File Offset: 0x0001B24C
		public void DrawWidget(Rect inrect, List<string> Data)
		{
			if (this.AllowRender)
			{
				ASRectMonitor asrectMonitor = new ASRectMonitor(inrect.ContractedBy(2f));
				bool flag = this.weapcatonly == 1;
				Widgets.CheckboxLabeled(asrectMonitor.NextFromTop(30f), "RWAutoSell.FilterWepCat".Translate(), ref flag, false, null, null, false, false);
				if (flag)
				{
					this.weapcatonly = 1;
				}
				else
				{
					this.weapcatonly = 0;
				}
				bool flag2 = this.IsBioCoded != 0;
				if (flag2)
				{
					Widgets.DrawHighlight(asrectMonitor.PeekFromTop(90f));
				}
				Widgets.CheckboxLabeled(asrectMonitor.NextFromTop(30f), "RWAutoSell.FilterWepCkBio".Translate(), ref flag2, false, null, null, false, false);
				if (flag2)
				{
					if (this.IsBioCoded == 0 | Widgets.RadioButtonLabeled(asrectMonitor.NextFromTop(30f), "RWAutoSell.FilterWepBio".Translate(), this.IsBioCoded == 1, false))
					{
						this.IsBioCoded = 1;
					}
					if (Widgets.RadioButtonLabeled(asrectMonitor.NextFromTop(30f), "RWAutoSell.FilterWepNotBio".Translate(), this.IsBioCoded == 2, false))
					{
						this.IsBioCoded = 2;
					}
				}
				else
				{
					this.IsBioCoded = 0;
				}
				bool flag3 = this.WeaponType != 0;
				if (flag3)
				{
					Widgets.DrawLightHighlight(asrectMonitor.PeekFromTop(90f));
				}
				Widgets.CheckboxLabeled(asrectMonitor.NextFromTop(30f), "RWAutoSell.FilterWepCkTp".Translate(), ref flag3, false, null, null, false, false);
				if (flag3)
				{
					if (this.WeaponType == 0 | Widgets.RadioButtonLabeled(asrectMonitor.NextFromTop(30f), "RWAutoSell.FilterWepRanged".Translate(), this.WeaponType == 1, false))
					{
						this.WeaponType = 1;
					}
					if (Widgets.RadioButtonLabeled(asrectMonitor.NextFromTop(30f), "RWAutoSell.FilterWepMelee".Translate(), this.WeaponType == 2, false))
					{
						this.WeaponType = 2;
						return;
					}
				}
				else
				{
					this.WeaponType = 0;
				}
			}
		}

		// Token: 0x04000198 RID: 408
		private bool AllowRender;

		// Token: 0x04000199 RID: 409
		private int WeaponType;

		// Token: 0x0400019A RID: 410
		private int IsBioCoded;

		// Token: 0x0400019B RID: 411
		private int weapcatonly = 1;
	}
}
