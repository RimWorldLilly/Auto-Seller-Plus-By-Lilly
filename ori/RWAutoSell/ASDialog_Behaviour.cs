using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using RimWorld;
using RWASWidgets;
using UnityEngine;
using Verse;

namespace RWAutoSell
{
	// Token: 0x0200001A RID: 26
	[NullableContext(1)]
	[Nullable(0)]
	internal class ASDialog_Behaviour : Window
	{
		// Token: 0x17000021 RID: 33
		// (get) Token: 0x060000BF RID: 191 RVA: 0x00008440 File Offset: 0x00006640
		public override Vector2 InitialSize
		{
			get
			{
				return new Vector2(700f, 500f);
			}
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x00008454 File Offset: 0x00006654
		public ASDialog_Behaviour(Map map) : base(null)
		{
			this.NegList = new ASMultiBox<string>(20f, 5f, true, false);
			this.map = map;
			this.comp = ASMapComp.GetSingleton(map);
			this.forcePause = true;
			this.absorbInputAroundWindow = true;
			this.doCloseX = true;
			this.doCloseButton = true;
			this.CachePermits();
			foreach (Pawn pawn in map.mapPawns.FreeColonists)
			{
				if (!StatDefOf.TradePriceImprovement.Worker.IsDisabledFor(pawn))
				{
					bool flag = this.comp.Negogiator != null && this.comp.Negogiator.ThingID == pawn.ThingID;
					bool flag2 = pawn.workSettings.WorkIsActive(WorkTypeDefOf.Warden);
					bool flag3 = this.AnyTitle(pawn);
					string text = string.Empty;
					if (flag)
					{
						text = "RWAutoSell.Prefered".Translate();
					}
					if (flag2)
					{
						if (text != "")
						{
							text += "\n\n";
						}
						text += "RWAutoSell.WardenSet".Translate();
					}
					if (flag3)
					{
						if (text != "")
						{
							text += "\n\n";
						}
						text = text + "RWAutoSell.HasPermits".Translate() + "\n" + this.FactionTitles(pawn);
					}
					SelectedItem<string> item = new SelectedItem<string>
					{
						Item = pawn.ThingID,
						label = pawn.Name.ToStringFull + (flag ? " (P)" : (flag2 ? " (W)" : "")) + (flag3 ? " (T)" : ""),
						Selected = (this.comp.MSettings.SelectedNegogiators.Contains(pawn.ThingID) || (this.comp.Negogiator != null && this.comp.Negogiator == pawn)),
						tooltip = text
					};
					this.NegList.Add(item);
				}
			}
			this.cacheperms.Clear();
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x000086C8 File Offset: 0x000068C8
		private string FactionTitles(Pawn p)
		{
			Pawn_RoyaltyTracker royalty = p.royalty;
			if (royalty != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (Faction faction in Find.FactionManager.AllFactionsListForReading)
				{
					StringBuilder stringBuilder2 = new StringBuilder();
					bool flag = false;
					foreach (RoyalTitlePermitDef royalTitlePermitDef in this.cacheperms)
					{
						if (royalty.HasPermit(royalTitlePermitDef, faction))
						{
							flag = true;
							if (stringBuilder2.Length > 0)
							{
								stringBuilder2.AppendLine();
							}
							stringBuilder2.Append("   " + royalTitlePermitDef.LabelCap);
						}
					}
					if (flag)
					{
						stringBuilder.AppendLine(" " + faction.Name);
						stringBuilder.AppendLine(stringBuilder2.ToString());
					}
				}
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x000087EC File Offset: 0x000069EC
		private bool AnyTitle(Pawn p)
		{
			Pawn_RoyaltyTracker royalty = p.royalty;
			if (royalty != null)
			{
				foreach (RoyalTitlePermitDef permit in this.cacheperms)
				{
					foreach (Faction faction in Find.FactionManager.AllFactionsListForReading)
					{
						if (royalty.HasPermit(permit, faction))
						{
							return true;
						}
					}
				}
				return false;
			}
			return false;
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x00008898 File Offset: 0x00006A98
		private void CachePermits()
		{
			foreach (TraderKindDef traderKindDef in DefDatabase<TraderKindDef>.AllDefs)
			{
				if (traderKindDef.permitRequiredForTrading != null)
				{
					this.cacheperms.Add(traderKindDef.permitRequiredForTrading);
				}
			}
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x000088F8 File Offset: 0x00006AF8
		public override void PreClose()
		{
			this.comp.MSettings.SelectedNegogiators.Clear();
			this.comp.MSettings.SelectedNegogiators.AddRange(this.NegList.GetSelectedItems);
			base.PreClose();
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x00008938 File Offset: 0x00006B38
		public override void DoWindowContents(Rect inRect)
		{
			Rect rect = inRect.ContractedBy(5f);
			ASRectMonitor asrectMonitor = new ASRectMonitor(rect.LeftHalf());
			ASRectMonitor asrectMonitor2 = new ASRectMonitor(rect.RightHalf());
			Rect rect2 = asrectMonitor.NextFromTop(30f);
			Rect rect3 = asrectMonitor2.NextFromTop(30f);
			Widgets.Label(rect2, "RWAutoSell.MapBehaviour".Translate(this.map.Parent.LabelCap));
			Widgets.Label(rect3, "RWAutoSell.Negotiator".Translate());
			Widgets.DrawLightHighlight(rect2);
			Widgets.DrawLightHighlight(rect3);
			ASRectMonitor asrectMonitor3 = new ASRectMonitor(asrectMonitor.Remaining().ContractedBy(1f));
			ASRectMonitor asrectMonitor4 = new ASRectMonitor(asrectMonitor2.Remaining().ContractedBy(1f));
			Widgets.CheckboxLabeled(asrectMonitor3.NextFromTop(30f).ContractedBy(2f), "RWAutoSell.Stacktoggle".Translate(), ref this.comp.MSettings.ASStackBeh, false, null, null, false, false);
			Widgets.CheckboxLabeled(asrectMonitor3.NextFromTop(30f).ContractedBy(2f), "RWAutoSell.AIToggle".Translate(), ref this.comp.MSettings.ASAITog, false, null, null, false, false);
			if (this.comp.MSettings.ASAITog)
			{
				Widgets.CheckboxLabeled(asrectMonitor3.NextFromTop(30f).ContractedBy(2f), "RWAutoSell.NotifyZeroTrades".Translate(), ref this.comp.MSettings.NotifyZeroTrades, false, null, null, false, false);
				Widgets.CheckboxLabeled(asrectMonitor3.NextFromTop(30f).ContractedBy(2f), "RWAutoSell.NotifyTrades".Translate(), ref this.comp.MSettings.NotifyTrades, false, null, null, false, false);
			}
			Widgets.CheckboxLabeled(asrectMonitor3.NextFromTop(30f).ContractedBy(2f), "RWAutoSell.Allowpartial".Translate(), ref this.comp.MSettings.ASPartTrade, false, null, null, false, false);
			Widgets.CheckboxLabeled(asrectMonitor3.NextFromTop(30f).ContractedBy(2f), "RWAutoSell.IncludeUnavailable".Translate(), ref this.comp.MSettings.IncludeUnavalible, false, null, null, false, false);
			Widgets.CheckboxLabeled(asrectMonitor3.NextFromTop(30f).ContractedBy(2f), "RWAutoSell.IncEqip".Translate(), ref this.comp.MSettings.IncEquip, false, null, null, false, false);
			Widgets.CheckboxLabeled(asrectMonitor3.NextFromTop(30f).ContractedBy(2f), "RWAutoSell.IncInv".Translate(), ref this.comp.MSettings.IncInv, false, null, null, false, false);
			Widgets.CheckboxLabeled(asrectMonitor3.NextFromTop(30f).ContractedBy(2f), "RWAutoSell.AltShop".Translate(), ref this.comp.MSettings.CustomShopping, false, null, null, false, false);
			Widgets.CheckboxLabeled(asrectMonitor3.NextFromTop(30f).ContractedBy(2f), "RWAutoSell.PostFractionCheck".Translate(), ref this.comp.MSettings.FractionCheck, false, null, null, false, false);
			bool sharedcomp = ASGameComp.GetSingleton.sharedcomp;
			Widgets.CheckboxLabeled(asrectMonitor3.NextFromTop(30f).ContractedBy(2f), "RWAutoSell.SharedGlobally".Translate(), ref sharedcomp, false, null, null, false, false);
			if (sharedcomp != ASGameComp.GetSingleton.sharedcomp)
			{
				if (sharedcomp)
				{
					if (this.comp.rules_ != null && this.comp.rules_.Count > 0)
					{
						ASGameComp.GetSingleton.Globalrules.Clear();
						foreach (ASRule item in this.comp.rules_)
						{
							ASGameComp.GetSingleton.Globalrules.Add(item);
						}
						ASGameComp.GetSingleton.Globalsettings.CopyFrom(this.comp.MSettings);
					}
				}
				else if (ASGameComp.GetSingleton.Globalrules != null && ASGameComp.GetSingleton.Globalrules.Count<ASRule>() > 0)
				{
					this.comp.rules_.Clear();
					foreach (ASRule item2 in ASGameComp.GetSingleton.Globalrules)
					{
						this.comp.rules_.Add(item2);
					}
					this.comp.MSettings.CopyFrom(ASGameComp.GetSingleton.Globalsettings);
				}
				ASGameComp.GetSingleton.sharedcomp = sharedcomp;
			}
			Rect rect4 = asrectMonitor3.NextFromTop(30f).ContractedBy(2f);
			Widgets.Label(rect4.LeftHalf(), "RWAutoSell.Repitition".Translate());
			Widgets.TextFieldNumeric<int>(rect4.RightHalf(), ref this.comp.MSettings.Repitition, ref this.CacheRep, 1f, 5f);
			if (Widgets.ButtonText(asrectMonitor3.NextFromTop(30f), "RWAutoSell.MergeControl".Translate(), true, true, true, null))
			{
				Find.WindowStack.Add(new ASMergerControls());
			}
			this.NegList.DrawListBox(asrectMonitor4.NextFromTop(asrectMonitor3.TotalTopUsed));
			Widgets.DrawBox(asrectMonitor3.GetRectFromTotal(false, false, true, false), 1, null);
			Widgets.DrawBox(asrectMonitor4.GetRectFromTotal(false, false, true, false), 1, null);
		}

		// Token: 0x04000086 RID: 134
		private Map map;

		// Token: 0x04000087 RID: 135
		private ASMapComp comp;

		// Token: 0x04000088 RID: 136
		private ASMultiBox<string> NegList;

		// Token: 0x04000089 RID: 137
		private HashSet<RoyalTitlePermitDef> cacheperms = new HashSet<RoyalTitlePermitDef>();

		// Token: 0x0400008A RID: 138
		private string CacheRep;
	}
}
