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
	// Token: 0x02000054 RID: 84
	[NullableContext(1)]
	[Nullable(0)]
	public class FilterValue : IBaseFilter, IWikiHelp
	{
		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x0600030C RID: 780 RVA: 0x0001C9B2 File Offset: 0x0001ABB2
		public string[] Tags { get; } = new string[]
		{
			"RWAutoSell.TagAnimals",
			"RWAutoSell.TagPeople",
			"RWAutoSell.TagItems",
			"RWAutoSell.TagFood"
		};

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x0600030D RID: 781 RVA: 0x0001C9BA File Offset: 0x0001ABBA
		public Texture2D MiniGraphic
		{
			get
			{
				return ASTextures.FPrice;
			}
		}

		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x0600030E RID: 782 RVA: 0x0001C9C1 File Offset: 0x0001ABC1
		public string HelpPage
		{
			get
			{
				return "https://github.com/SupesSolutions/RWAutoSeller/wiki/Value-Filter";
			}
		}

		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x0600030F RID: 783 RVA: 0x0001C9C8 File Offset: 0x0001ABC8
		public string Label
		{
			get
			{
				return "RWAutoSell.FilterBuyPrice".Translate();
			}
		}

		// Token: 0x06000310 RID: 784 RVA: 0x0001C9DC File Offset: 0x0001ABDC
		public bool CheckItem(Transferable t, List<string> Data)
		{
			Tradeable tradeable = t as Tradeable;
			if (tradeable != null)
			{
				PriceType priceType = tradeable.PriceTypeFor(TradeAction.PlayerBuys);
				string str = "pt.";
				byte b = (byte)priceType;
				if (Data.Contains(str + b.ToString()))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000311 RID: 785 RVA: 0x0001CA19 File Offset: 0x0001AC19
		public List<string> DataFromTradeable(Transferable t)
		{
			return new List<string>();
		}

		// Token: 0x06000312 RID: 786 RVA: 0x0001CA20 File Offset: 0x0001AC20
		public List<string> DataOptions(List<string> Data, Map map)
		{
			return Data;
		}

		// Token: 0x06000313 RID: 787 RVA: 0x0001CA23 File Offset: 0x0001AC23
		public void DrawWidget(Rect inrect, List<string> Data)
		{
			if (this.AllowRender)
			{
				this.PriceBox.DrawListBox(inrect.ContractedBy(2f));
			}
		}

		// Token: 0x06000314 RID: 788 RVA: 0x0001CA44 File Offset: 0x0001AC44
		private Texture2D CreateFromColour(Color d)
		{
			Texture2D texture2D = new Texture2D(1, 1);
			texture2D.SetPixels((from el in texture2D.GetPixels()
			select d).ToArray<Color>());
			return texture2D;
		}

		// Token: 0x06000315 RID: 789 RVA: 0x0001CA88 File Offset: 0x0001AC88
		public void RenderStarted(List<string> Data, Map map)
		{
			this.PriceBox = new ASMultiBox<PriceType>(30f, 4f, true, true);
			foreach (string text in Enum.GetNames(typeof(PriceType)))
			{
				string text2 = "PriceType" + text;
				if (text2.CanTranslate())
				{
					text2 = text2.Translate();
				}
				else
				{
					text2 = text;
				}
				bool selected = false;
				if (Data.Contains("pt." + ((byte)Enum.Parse(typeof(PriceType), text, true)).ToString()))
				{
					selected = true;
				}
				Texture2D icon = null;
				PriceType item = (PriceType)Enum.Parse(typeof(PriceType), text, true);
				switch (item)
				{
				case PriceType.VeryCheap:
					icon = this.CreateFromColour(new Color(0f, 1f, 0f));
					break;
				case PriceType.Cheap:
					icon = this.CreateFromColour(new Color(0.5f, 1f, 0.5f));
					break;
				case PriceType.Normal:
					icon = this.CreateFromColour(Color.white);
					break;
				case PriceType.Expensive:
					icon = this.CreateFromColour(new Color(1f, 0.5f, 0.5f));
					break;
				case PriceType.Exorbitant:
					icon = this.CreateFromColour(new Color(1f, 0f, 0f));
					break;
				}
				this.PriceBox.Add(new SelectedItem<PriceType>
				{
					Item = item,
					label = text2.CapitalizeFirst(),
					Selected = selected,
					Icon = icon
				});
			}
			this.AllowRender = true;
		}

		// Token: 0x06000316 RID: 790 RVA: 0x0001CC2C File Offset: 0x0001AE2C
		public void RenderStopped(List<string> Data, Map map)
		{
			this.AllowRender = false;
			if (this.PriceBox != null)
			{
				Data.Clear();
				foreach (SelectedItem<PriceType> selectedItem in this.PriceBox)
				{
					if (selectedItem.Selected)
					{
						Data.Add("pt." + ((byte)selectedItem.Item).ToString());
					}
				}
			}
		}

		// Token: 0x04000195 RID: 405
		private ASMultiBox<PriceType> PriceBox;

		// Token: 0x04000196 RID: 406
		private bool AllowRender;
	}
}
