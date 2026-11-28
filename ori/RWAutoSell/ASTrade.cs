using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using RimWorld;
using RimWorld.Planet;
using RWASFilterLib;
using RWAutoSell.Filters;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RWAutoSell
{
	// Token: 0x0200000F RID: 15
	[NullableContext(1)]
	[Nullable(0)]
	public static class ASTrade
	{
		// Token: 0x0600005B RID: 91 RVA: 0x00004A7C File Offset: 0x00002C7C
		public static void GetCurrencies()
		{
			if (ASTrade.CurrencyDefs.Count == 0)
			{
				List<string> list = new List<string>();
				foreach (ModMetaData modMetaData in ModsConfig.ActiveModsInLoadOrder)
				{
					foreach (DirectoryInfo directoryInfo in modMetaData.RootDir.GetDirectories())
					{
						if (directoryInfo.Name.ToLower() == "about")
						{
							foreach (FileInfo fileInfo in directoryInfo.GetFiles())
							{
								if (fileInfo.Name.ToLower() == "rwautosell.xml")
								{
									int num = 0;
									try
									{
										XElement xelement = XDocument.Load(fileInfo.FullName, LoadOptions.SetLineInfo).Element("AutoSeller");
										num = ((IXmlLineInfo)xelement).LineNumber;
										foreach (XElement xelement2 in xelement.Elements("Currency"))
										{
											list.Add(xelement2.Value);
										}
									}
									catch
									{
										Log.Message("Error reading Currency File: line " + num.ToString());
									}
								}
							}
						}
					}
				}
				ASTrade.CurrencyDefs = list;
			}
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00004C28 File Offset: 0x00002E28
		internal static bool TradeAllShips(Pawn pawn)
		{
			if (pawn.Map.passingShipManager.passingShips == null || pawn.Map.passingShipManager.passingShips.Count == 0)
			{
				return false;
			}
			ASMapComp singleton = ASMapComp.GetSingleton(pawn.Map);
			if (singleton.Rules.Count == 0)
			{
				return false;
			}
			bool flag = false;
			foreach (PassingShip passingShip in pawn.Map.passingShipManager.passingShips)
			{
				ITrader ship = (ITrader)passingShip;
				flag |= ASTrade.CreateTrade(ship, pawn, true);
			}
			if (flag)
			{
				if (ASMod.GetSettings().Kaching)
				{
					SoundDefOf.ExecuteTrade.PlayOneShotOnCamera(null);
				}
				singleton.HasNewShip = false;
			}
			return flag;
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00004CF8 File Offset: 0x00002EF8
		internal static bool SetCurrencies(ITrader Trader)
		{
			bool devAILog = ASMod.GetSettings().devAILog;
			Pawn playerNegotiator = TradeSession.playerNegotiator;
			if (TradeSession.deal.CurrencyTradeable != null)
			{
				bool flag;
				ASTrade.Forcetrade(out flag);
			}
			else if (devAILog)
			{
				Log.Message("Autoseller: Get Currency Failed, aborting");
			}
			return true;
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00004D3C File Offset: 0x00002F3C
		private static void Forcetrade(out bool actuallyTraded)
		{
			actuallyTraded = false;
			float num = 0f;
			foreach (Tradeable tradeable in TradeSession.deal.AllTradeables)
			{
				if (tradeable.ActionToDo != TradeAction.None)
				{
					actuallyTraded = true;
				}
				if (tradeable.ActionToDo == TradeAction.PlayerSells)
				{
					num += tradeable.CurTotalCurrencyCostForDestination;
				}
				tradeable.ResolveTrade();
			}
			TradeSession.deal.Reset();
			if (TradeSession.trader.Faction != null)
			{
				TradeSession.trader.Faction.Notify_PlayerTraded(num, TradeSession.playerNegotiator);
			}
			Pawn pawn = TradeSession.trader as Pawn;
			if (pawn != null)
			{
				TaleRecorder.RecordTale(TaleDefOf.TradedWith, new object[]
				{
					TradeSession.playerNegotiator,
					pawn
				});
			}
			TradeSession.playerNegotiator.mindState.inspirationHandler.EndInspiration(InspirationDefOf.Inspired_Trade);
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00004E28 File Offset: 0x00003028
		private static int TransactorToDir(Transactor Trans)
		{
			return (int)(Trans * (Transactor)2 - Transactor.Trader);
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00004E2F File Offset: 0x0000302F
		private static int TransactorToAltDir(Transactor Trans)
		{
			return ASTrade.TransactorToDir(Trans) * -1;
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00004E3C File Offset: 0x0000303C
		private static bool DoesXHaveEnoughMoney()
		{
			bool result = false;
			if (TradeSession.deal.CurrencyTradeable.CountToTransfer >= TradeSession.deal.CurrencyTradeable.GetMinimumToTransfer() && TradeSession.deal.CurrencyTradeable.CountToTransfer <= TradeSession.deal.CurrencyTradeable.GetMaximumToTransfer())
			{
				result = true;
			}
			return result;
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00004E90 File Offset: 0x00003090
		internal static bool CreateTrade(ITrader Ship, Pawn pawn, bool ShowReport = false)
		{
			bool flag = ShowReport;
			if (pawn.MapHeld != null)
			{
				pawn.MapHeld.GetComponent<ASMapComp>();
				flag = (ASMod.GetSettings().devlettershow || ShowReport);
			}
			bool devAILog = ASMod.GetSettings().devAILog;
			if (devAILog)
			{
				Log.Message("Autoseller: AI - Setting up trade (" + Ship.TraderName + ")");
			}
			TradeSession.SetupWith(Ship, pawn, false);
			List<Tradeable> cachedTradeables = (from tr in TradeSession.deal.AllTradeables
			where !tr.IsCurrency && !tr.IsFavor && tr.TraderWillTrade
			select tr).ToList<Tradeable>();
			List<Defcountpair> list = new List<Defcountpair>();
			List<Defcountpair> list2 = new List<Defcountpair>();
			if (devAILog)
			{
				Log.Message("Autoseller: AI - Appling Rules");
			}
			int currencychanged;
			ASTrade.SetTradables(cachedTradeables, pawn.Map, list2, list, out currencychanged, true, false);
			bool result;
			if (ASTrade.DoesXHaveEnoughMoney())
			{
				if (devAILog)
				{
					Log.Message("Autoseller: AI - Currency Check, Attempting Trade Excute");
				}
				result = ASTrade.SetCurrencies(Ship);
			}
			else
			{
				if (devAILog)
				{
					Log.Message(string.Concat(new string[]
					{
						"Autoseller: AI - Currency Check Failed",
						TradeSession.deal.CurrencyTradeable.GetMinimumToTransfer().ToString(),
						"/",
						TradeSession.deal.CurrencyTradeable.GetMaximumToTransfer().ToString(),
						"(",
						TradeSession.deal.CurrencyTradeable.CountToTransfer.ToString(),
						"), Trade Rejected"
					}));
				}
				result = false;
				SoundDefOf.ClickReject.PlayOneShotOnCamera(null);
			}
			bool flag2 = false;
			if (list.Count != 0 || list2.Count != 0)
			{
				flag2 = true;
			}
			if (pawn.MapHeld != null && pawn.MapHeld.GetComponent<ASMapComp>().MSettings.NotifyZeroTrades)
			{
				flag2 = true;
			}
			if (flag2 && flag)
			{
				if (ASMod.GetSettings().Kaching)
				{
					SoundDefOf.ExecuteTrade.PlayOneShotOnCamera(null);
				}
				if (pawn.MapHeld.GetComponent<ASMapComp>().MSettings.NotifyTrades)
				{
					ASTrade.TradeReport(pawn, Ship, list2, list, currencychanged, pawn.Position, true);
				}
			}
			return result;
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00005088 File Offset: 0x00003288
		private static int SellCountChange(Tradeable Tradeitem, int newvalue, int oldvalue)
		{
			Tradeitem.AdjustTo(newvalue);
			TradeSession.deal.UpdateCurrencyCount();
			if (ASTrade.DoesXHaveEnoughMoney())
			{
				return newvalue;
			}
			Tradeitem.AdjustTo(oldvalue);
			TradeSession.deal.UpdateCurrencyCount();
			if (ASTrade.currentcomp.MSettings.ASPartTrade)
			{
				if (oldvalue != 0 && newvalue > 0 != oldvalue > 0)
				{
					Tradeitem.AdjustTo(0);
					TradeSession.deal.UpdateCurrencyCount();
					if (!ASTrade.DoesXHaveEnoughMoney())
					{
						Tradeitem.AdjustTo(oldvalue);
						TradeSession.deal.UpdateCurrencyCount();
						return oldvalue;
					}
				}
				int num = 0;
				float num2 = 0f;
				if (newvalue > 0)
				{
					float priceFor = Tradeitem.GetPriceFor(TradeAction.PlayerBuys);
					if (priceFor == 0f)
					{
						Tradeitem.AdjustTo(oldvalue);
						TradeSession.deal.UpdateCurrencyCount();
						return oldvalue;
					}
					num2 = (float)(TradeSession.deal.CurrencyTradeable.CountHeldBy(Transactor.Colony) + TradeSession.deal.CurrencyTradeable.CountToTransfer);
					if (num2 > priceFor)
					{
						num = (int)Math.Truncate((double)(num2 / priceFor));
					}
				}
				else if (newvalue < 0)
				{
					float priceFor = Tradeitem.GetPriceFor(TradeAction.PlayerSells);
					if (priceFor == 0f)
					{
						Tradeitem.AdjustTo(oldvalue);
						TradeSession.deal.UpdateCurrencyCount();
						return oldvalue;
					}
					num2 = (float)(TradeSession.deal.CurrencyTradeable.CountHeldBy(Transactor.Trader) - TradeSession.deal.CurrencyTradeable.CountToTransfer);
					if (num2 > priceFor)
					{
						num = (int)Math.Truncate((double)(num2 / -(double)priceFor));
					}
				}
				if (num != 0)
				{
					if (Tradeitem.CanAdjustTo(num).Accepted)
					{
						Tradeitem.AdjustTo(num);
						TradeSession.deal.UpdateCurrencyCount();
						return num;
					}
					Log.Error(string.Concat(new string[]
					{
						"Error with partial trade: wanted value(",
						newvalue.ToString(),
						") / old value(",
						oldvalue.ToString(),
						") / attempted partial value (",
						num.ToString(),
						") / currency value (",
						num2.ToString(),
						")"
					}));
				}
				Tradeitem.AdjustTo(oldvalue);
				TradeSession.deal.UpdateCurrencyCount();
			}
			return oldvalue;
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00005264 File Offset: 0x00003464
		private static int SetCount(ASRule Rule, Tradeable item)
		{
			if (item.CountHeldBy(Transactor.Colony) > Rule.SellCount && Rule.SellCount >= Rule.BuyPoint)
			{
				return -(item.CountHeldBy(Transactor.Colony) - Rule.SellCount);
			}
			if (item.CountHeldBy(Transactor.Colony) >= Rule.BuyPoint || Rule.RestockLevel == 0)
			{
				return item.CountToTransfer;
			}
			int num = -(item.CountHeldBy(Transactor.Colony) - Rule.RestockLevel);
			if (num > item.CountHeldBy(Transactor.Trader))
			{
				return item.CountHeldBy(Transactor.Trader);
			}
			return num;
		}

		// Token: 0x06000065 RID: 101 RVA: 0x000052E0 File Offset: 0x000034E0
		private static void SetTradeStack(List<int> MStackTr, ASRule Rule, ref List<Tradeable> cachedTradeables, List<int> MStackCt, ref List<TransferableOneWay> ColonyTradeables)
		{
			if (ASTrade.currentcomp.MSettings.ASStackBeh && MStackTr.Count<int>() >= 1)
			{
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				List<Tradeable> list = new List<Tradeable>();
				for (int i = 0; i < MStackTr.Count<int>(); i++)
				{
					Tradeable tradeable = cachedTradeables[MStackTr[i]];
					list.Add(cachedTradeables[MStackTr[i]]);
					num += tradeable.CountHeldBy(Transactor.Colony);
					num2 += tradeable.CountHeldBy(Transactor.Trader);
				}
				bool buymode = false;
				if (ASTrade.currentcomp.MSettings.IncludeUnavalible)
				{
					for (int j = 0; j < MStackCt.Count; j++)
					{
						TransferableOneWay transferableOneWay = ColonyTradeables[MStackCt[j]];
						num3 += transferableOneWay.MaxCount;
					}
					if (ASTrade.IsCaravan)
					{
						num3 += num;
					}
				}
				if (!ASTrade.GetStackByAvalibility(Rule, ref num3, ref num, ref num2, ref buymode))
				{
					return;
				}
				ASTrade.SetByMode(list, buymode, ref num);
			}
		}

		// Token: 0x06000066 RID: 102 RVA: 0x000053D8 File Offset: 0x000035D8
		private static void SetByMode(List<Tradeable> MList, bool buymode, ref int StackCount)
		{
			for (int i = 0; i < MList.Count; i++)
			{
				Tradeable tradeable = MList[i];
				int countToTransfer = tradeable.CountToTransfer;
				if (buymode)
				{
					if (tradeable.CountHeldBy(Transactor.Trader) != 0)
					{
						if (StackCount >= tradeable.CountHeldBy(Transactor.Trader))
						{
							int newvalue = tradeable.CountHeldBy(Transactor.Trader);
							StackCount -= ASTrade.SellCountChange(tradeable, newvalue, countToTransfer);
						}
						else if (StackCount > 0)
						{
							StackCount -= ASTrade.SellCountChange(tradeable, StackCount, countToTransfer);
						}
					}
				}
				else if (tradeable.CountHeldBy(Transactor.Colony) != 0)
				{
					if (StackCount >= tradeable.CountHeldBy(Transactor.Colony))
					{
						int newvalue2 = -tradeable.CountHeldBy(Transactor.Colony);
						StackCount += ASTrade.SellCountChange(tradeable, newvalue2, countToTransfer);
					}
					else if (StackCount > 0)
					{
						StackCount += ASTrade.SellCountChange(tradeable, -StackCount, countToTransfer);
					}
				}
			}
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00005490 File Offset: 0x00003690
		private static bool GetStackByAvalibility(ASRule Rule, ref int UnStackCount, ref int StackCount, ref int TradeCount, ref bool buymode)
		{
			bool result = true;
			if (ASTrade.currentcomp.MSettings.IncludeUnavalible)
			{
				if (StackCount > UnStackCount)
				{
					UnStackCount = StackCount;
				}
				if (UnStackCount > Rule.SellCount && Rule.SellCount >= Rule.BuyPoint)
				{
					UnStackCount -= Rule.SellCount;
					StackCount = UnStackCount;
				}
				else if (StackCount < Rule.BuyPoint)
				{
					buymode = true;
					int num = ((Rule.RestockLevel >= Rule.BuyPoint) ? Rule.RestockLevel : Rule.BuyPoint) - UnStackCount;
					StackCount = ((num < TradeCount) ? num : TradeCount);
				}
				else
				{
					result = false;
				}
			}
			else if (StackCount > Rule.SellCount && Rule.SellCount >= Rule.BuyPoint)
			{
				StackCount -= Rule.SellCount;
			}
			else if (StackCount < Rule.BuyPoint)
			{
				buymode = true;
				int num2 = ((Rule.RestockLevel >= Rule.BuyPoint) ? Rule.RestockLevel : Rule.BuyPoint) - StackCount;
				StackCount = ((num2 < TradeCount) ? num2 : TradeCount);
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000068 RID: 104 RVA: 0x0000558C File Offset: 0x0000378C
		private static void SetTradeStackSeperate(List<int> MStackTr, ASRule Rule, ref List<Tradeable> cachedTradeables, List<int> MStackCt, ref List<TransferableOneWay> ColonyTradeables)
		{
			if (ASTrade.currentcomp.MSettings.ASStackBeh && MStackTr.Count >= 1)
			{
				HashSet<ThingDef> hashSet = new HashSet<ThingDef>();
				foreach (int index in MStackTr)
				{
					hashSet.Add(cachedTradeables[index].ThingDef);
				}
				foreach (ThingDef thingDef in hashSet)
				{
					int num = 0;
					int num2 = 0;
					int num3 = 0;
					List<Tradeable> list = new List<Tradeable>();
					for (int i = 0; i < MStackTr.Count; i++)
					{
						Tradeable tradeable = cachedTradeables[MStackTr[i]];
						if (tradeable.ThingDef == thingDef)
						{
							list.Add(cachedTradeables[MStackTr[i]]);
							num += tradeable.CountHeldBy(Transactor.Colony);
							num2 += tradeable.CountHeldBy(Transactor.Trader);
						}
					}
					bool buymode = false;
					if (ASTrade.currentcomp.MSettings.IncludeUnavalible)
					{
						for (int j = 0; j < MStackCt.Count; j++)
						{
							TransferableOneWay transferableOneWay = ColonyTradeables[MStackCt[j]];
							if (transferableOneWay.ThingDef == thingDef)
							{
								num3 += transferableOneWay.MaxCount;
							}
						}
						if (ASTrade.IsCaravan)
						{
							num3 += num;
						}
					}
					if (ASTrade.GetStackByAvalibility(Rule, ref num3, ref num, ref num2, ref buymode))
					{
						ASTrade.SetByMode(list, buymode, ref num);
					}
				}
			}
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00005750 File Offset: 0x00003950
		private static void DoColony(List<TransferableOneWay> cachedTradeables, ASRule Rule)
		{
			ASTrade.UnavaliblePairs.Add(new Pair<ASRule, List<TransferableOneWay>>(Rule, cachedTradeables));
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00005764 File Offset: 0x00003964
		private static void DoRuleMulti(RuleArgs<Tradeable, ASRule> RuleArgs)
		{
			int num = 0;
			if (ASLibTransferUtility.AnyPassthroughOnRuleChain<ASRule>(RuleArgs.RulesOnChain.ToArray(), 1))
			{
				int num2;
				ASLibTransferUtility.TransferableListtoCount<Tradeable>(RuleArgs.Matches[1], out num, out num2);
			}
			ASRule rule;
			if (num > 1)
			{
				rule = RuleArgs.Rule.MultipliedCopy(num);
			}
			else
			{
				rule = RuleArgs.Rule;
			}
			ASTrade.DoRule(RuleArgs.Matches[0], rule);
		}

		// Token: 0x0600006B RID: 107 RVA: 0x000057C0 File Offset: 0x000039C0
		private static void DoRule(List<Tradeable> cachedTradeables, ASRule Rule)
		{
			List<int> list = new List<int>();
			List<int> list2 = new List<int>();
			List<TransferableOneWay> source = ASTrade.ColonyTradeables;
			for (int i = 0; i < cachedTradeables.Count; i++)
			{
				if (cachedTradeables[i].HasAnyThing && cachedTradeables[i].TraderWillTrade && !cachedTradeables[i].IsCurrency && !cachedTradeables[i].Label.NullOrEmpty())
				{
					int countToTransfer = cachedTradeables[i].CountToTransfer;
					if (!ASTrade.currentcomp.MSettings.ASStackBeh || (Rule.SellCount == 0 && Rule.BuyPoint == 0))
					{
						ASTrade.SellCountChange(cachedTradeables[i], ASTrade.SetCount(Rule, cachedTradeables[i]), countToTransfer);
					}
					else
					{
						list.Add(i);
					}
				}
			}
			if (ASTrade.currentcomp.MSettings.IncludeUnavalible)
			{
				Pair<ASRule, List<TransferableOneWay>> pair = ASTrade.UnavaliblePairs.FirstOrDefault((Pair<ASRule, List<TransferableOneWay>> x) => x.First == Rule);
				if (pair.Second != null)
				{
					source = pair.Second;
					for (int j = 0; j < source.Count<TransferableOneWay>(); j++)
					{
						if (ASTrade.currentcomp.MSettings.ASStackBeh && (Rule.SellCount != 0 || Rule.BuyPoint != 0))
						{
							list2.Add(j);
						}
					}
				}
			}
			if (Rule.TreatDefAsSeperate)
			{
				ASTrade.SetTradeStackSeperate(list, Rule, ref cachedTradeables, list2, ref source);
			}
			else
			{
				ASTrade.SetTradeStack(list, Rule, ref cachedTradeables, list2, ref source);
			}
			TradeSession.deal.UpdateCurrencyCount();
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00005974 File Offset: 0x00003B74
		public static void SetTradables(List<Tradeable> cachedTradeables, Map map, List<Defcountpair> Bought, List<Defcountpair> Sold, out int Currency, bool IgnoreAdditionalCurrencys = true, bool iscaravan = false)
		{
			ASTrade.IsCaravan = iscaravan;
			Currency = 0;
			if (TradeSession.TradeCurrency != TradeCurrency.Silver || cachedTradeables.Count == 0 || TradeSession.giftMode)
			{
				return;
			}
			List<Tradeable> list = new List<Tradeable>();
			foreach (Tradeable tradeable in cachedTradeables)
			{
				if (!tradeable.IsCurrency && !tradeable.IsFavor && tradeable.HasAnyThing)
				{
					list.Add(tradeable);
				}
			}
			ASTrade.currentcomp = ASMapComp.GetSingleton(map);
			ASTrade.ColonyTradeables = ASLibTransferUtility.MapTradables(map, false, TransferAsOneMode.Normal, null, false, false, false);
			if (ASTrade.currentcomp.MSettings.IncEquip || ASTrade.currentcomp.MSettings.IncInv)
			{
				HashSet<Pawn> hashSet = new HashSet<Pawn>();
				hashSet.AddRange(map.mapPawns.FreeColonists.ToList<Pawn>());
				if (ASTrade.IsCaravan)
				{
					Caravan caravan = TradeSession.playerNegotiator.GetCaravan();
					if (caravan != null)
					{
						foreach (Pawn item in caravan.pawns)
						{
							if (!hashSet.Add(item))
							{
								break;
							}
						}
					}
				}
				if (ASTrade.currentcomp.MSettings.IncEquip)
				{
					foreach (Pawn pawn in hashSet)
					{
						if (pawn.apparel != null && pawn.apparel.WornApparelCount > 0)
						{
							foreach (Thing thing in pawn.apparel.WornApparel)
							{
								TransferableOneWay transferableOneWay = TransferableUtility.TransferableMatching<TransferableOneWay>(thing, ASTrade.ColonyTradeables, TransferAsOneMode.Normal);
								if (transferableOneWay == null)
								{
									transferableOneWay = new TransferableOneWay();
									ASTrade.ColonyTradeables.Add(transferableOneWay);
								}
								transferableOneWay.things.Add(thing);
							}
						}
						if (pawn.equipment != null && pawn.equipment.HasAnything() && pawn.equipment.Primary != null)
						{
							TransferableOneWay transferableOneWay2 = TransferableUtility.TransferableMatching<TransferableOneWay>(pawn.equipment.Primary, ASTrade.ColonyTradeables, TransferAsOneMode.Normal);
							if (transferableOneWay2 == null)
							{
								transferableOneWay2 = new TransferableOneWay();
								ASTrade.ColonyTradeables.Add(transferableOneWay2);
							}
							transferableOneWay2.things.Add(pawn.equipment.Primary);
						}
					}
				}
				if (ASTrade.currentcomp.MSettings.IncInv)
				{
					foreach (Pawn pawn2 in hashSet)
					{
						if (pawn2.inventory != null)
						{
							foreach (Thing thing2 in pawn2.inventory.innerContainer)
							{
								TransferableOneWay transferableOneWay3 = TransferableUtility.TransferableMatching<TransferableOneWay>(thing2, ASTrade.ColonyTradeables, TransferAsOneMode.Normal);
								if (transferableOneWay3 == null)
								{
									transferableOneWay3 = new TransferableOneWay();
									ASTrade.ColonyTradeables.Add(transferableOneWay3);
								}
								transferableOneWay3.things.Add(thing2);
							}
						}
					}
				}
			}
			ASTrade.UnavaliblePairs = new List<Pair<ASRule, List<TransferableOneWay>>>();
			List<TransferableOneWay> colonyTradeables = ASTrade.ColonyTradeables;
			BaseRuledMapComp<ASRule> rulesIn = ASTrade.currentcomp;
			int ruleRoot = 0;
			Action<TransferableOneWay, ASRule> actionOnTrans = null;
			Action<List<TransferableOneWay>, ASRule> actionOnRule;
			if ((actionOnRule = ASTrade.<>O.<0>__DoColony) == null)
			{
				actionOnRule = (ASTrade.<>O.<0>__DoColony = new Action<List<TransferableOneWay>, ASRule>(ASTrade.DoColony));
			}
			ASLibTransferUtility.GetMatchedTradables<TransferableOneWay, ASRule>(colonyTradeables, rulesIn, ruleRoot, actionOnTrans, actionOnRule);
			int num = ASTrade.currentcomp.MSettings.Repitition;
			List<IRule> list2 = new List<IRule>();
			if (ASTrade.IsCaravan && ASTrade.currentcomp.MSettings.CustomShopping)
			{
				FilterContainer filterContainer = new FilterContainer(typeof(FilterCat));
				foreach (ThingCategoryDef thingCategoryDef in DefDatabase<ThingCategoryDef>.AllDefs)
				{
					if (thingCategoryDef.Parents.Any((ThingCategoryDef x) => x == ThingCategoryDefOf.Foods) || thingCategoryDef == ThingCategoryDefOf.Foods)
					{
						filterContainer.FilterData.Add("cat." + thingCategoryDef.defName);
					}
				}
				FilterContainer filterContainer2 = new FilterContainer(typeof(FilterPawnKind));
				filterContainer2.FilterData.Add("any.hmd");
				filterContainer2.FilterData.Add("any.anm");
				filterContainer2.FilterData.Add("any.mch");
				ASRule asrule = new ASRule();
				FilterGroup filterGroup = new FilterGroup();
				filterGroup.Filters.Add(filterContainer);
				filterGroup.IsNegated = true;
				FilterGroup filterGroup2 = new FilterGroup();
				filterGroup2.Filters.Add(filterContainer2);
				filterGroup2.IsNegated = true;
				asrule.Nodes[0].RootNode.Groups.Add(filterGroup);
				asrule.Nodes[0].RootNode.Groups.Add(filterGroup2);
				list2.Add(asrule);
			}
			List<Tradeable> list3 = new List<Tradeable>();
			foreach (TransferableOneWay transferableOneWay4 in ASTrade.ColonyTradeables)
			{
				Tradeable tradeable2 = new Tradeable();
				foreach (Thing t in transferableOneWay4.things)
				{
					tradeable2.AddThing(t, Transactor.Colony);
				}
				if (tradeable2.HasAnyThing)
				{
					list3.Add(tradeable2);
				}
			}
			foreach (Thing thing3 in ASLibTransferUtility.GetColonyPawns(map, null))
			{
				Pawn t2 = (Pawn)thing3;
				Tradeable_Pawn tradeable_Pawn = new Tradeable_Pawn();
				tradeable_Pawn.AddThing(t2, Transactor.Colony);
				if (tradeable_Pawn.HasAnyThing)
				{
					list3.Add(tradeable_Pawn);
				}
			}
			do
			{
				num--;
				if (ASTrade.IsCaravan && ASTrade.currentcomp.MSettings.CustomShopping)
				{
					List<ASRule> list4 = new List<ASRule>();
					List<Tradeable>[] listIn = new List<Tradeable>[]
					{
						list,
						list3
					};
					List<ASRule> preRules = list4;
					IEnumerable<IRule> rulesIn2 = list2;
					Action<Tradeable, ASRule, int>[] actionOnTrans2 = null;
					Action<RuleArgs<Tradeable, ASRule>> actionOnRule2;
					if ((actionOnRule2 = ASTrade.<>O.<1>__DoRuleMulti) == null)
					{
						actionOnRule2 = (ASTrade.<>O.<1>__DoRuleMulti = new Action<RuleArgs<Tradeable, ASRule>>(ASTrade.DoRuleMulti));
					}
					ASLibTransferUtility.GetMultiNodeMatchedTradables<Tradeable, ASRule>(listIn, preRules, rulesIn2, actionOnTrans2, actionOnRule2);
				}
				else
				{
					List<Tradeable>[] listIn2 = new List<Tradeable>[]
					{
						list,
						list3
					};
					BaseRuledMapComp<ASRule> rulesIn3 = ASTrade.currentcomp;
					Action<Tradeable, ASRule, int>[] actionOnTrans3 = null;
					Action<RuleArgs<Tradeable, ASRule>> actionOnRule3;
					if ((actionOnRule3 = ASTrade.<>O.<1>__DoRuleMulti) == null)
					{
						actionOnRule3 = (ASTrade.<>O.<1>__DoRuleMulti = new Action<RuleArgs<Tradeable, ASRule>>(ASTrade.DoRuleMulti));
					}
					ASLibTransferUtility.GetMultiNodeMatchedTradables<Tradeable, ASRule>(listIn2, rulesIn3, actionOnTrans3, actionOnRule3);
				}
			}
			while (num > 0);
			try
			{
				foreach (Tradeable tradeable3 in list)
				{
					if (tradeable3.CountToTransfer > 0)
					{
						Defcountpair.ModifyExisting(tradeable3, Bought);
					}
					else if (tradeable3.CountToTransfer < 0)
					{
						Defcountpair.ModifyExisting(tradeable3, Sold);
					}
				}
				Currency = TradeSession.deal.CurrencyTradeable.CountToTransfer;
			}
			catch
			{
			}
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00006164 File Offset: 0x00004364
		private static void DeZeroFi(List<Tradeable> CTrads)
		{
			float num = 0f;
			float num2 = num - Mathf.Round(num);
			for (int i = 0; i < CTrads.Count; i++)
			{
				Tradeable tradeable = CTrads[i];
				if (!tradeable.IsCurrency)
				{
					num += tradeable.CurTotalCurrencyCostForSource;
				}
			}
			foreach (Tradeable tradeable2 in CTrads)
			{
				if (!tradeable2.IsCurrency && !tradeable2.IsFavor && tradeable2.CountToTransfer < 0)
				{
					float priceFor = tradeable2.GetPriceFor(TradeAction.PlayerSells);
					if (priceFor == 0f)
					{
						tradeable2.AdjustTo(0);
					}
					else if (num2 > 0f && priceFor < num2)
					{
						int num3 = (int)(priceFor / num2);
						if (-num3 > tradeable2.CountToTransfer)
						{
							num3 = -tradeable2.CountToTransfer;
						}
						tradeable2.AdjustTo(tradeable2.ClampAmount(tradeable2.CountToTransfer + num3));
						num2 -= priceFor * (float)num3;
					}
				}
			}
			TradeSession.deal.UpdateCurrencyCount();
		}

		// Token: 0x0600006E RID: 110 RVA: 0x0000627C File Offset: 0x0000447C
		private static void ReportAlign(string header, ref StringBuilder builder, List<Defcountpair> Appendfrom)
		{
			float x = Text.CalcSize("\t").x;
			float x2 = Text.CalcSize(header).x;
			builder.Append(header + "\n");
			foreach (Defcountpair defcountpair in Appendfrom)
			{
				string text = string.Format("{0,-20} {1,-8}", defcountpair.Def.LabelCap, defcountpair.Count).Trim();
				float x3 = Text.CalcSize(text).x;
				string value = string.Empty;
				if (x3 < x2)
				{
					string value2 = string.Format("{0," + ((int)((x3 - x2) / x)).ToString() + "}", "\t");
					value = text.Insert(text.Length - defcountpair.Count.ToString().Length - 1, value2) + "\n";
				}
				builder.Append(value);
			}
		}

		// Token: 0x0600006F RID: 111 RVA: 0x000063A4 File Offset: 0x000045A4
		public static void TradeReport(Pawn Negotiator, ITrader ship, List<Defcountpair> BoughtList, List<Defcountpair> SoldList, int currencychanged, IntVec3 location, bool showzero = false)
		{
			if (BoughtList.Count<Defcountpair>() == 0 && SoldList.Count<Defcountpair>() == 0 && !showzero)
			{
				return;
			}
			string value = string.Empty;
			if (currencychanged >= 0)
			{
				value = "RWAutoSell.BargainReportGained".Translate();
			}
			else
			{
				value = "RWAutoSell.BargainReportLost".Translate();
			}
			int num = Math.Abs(currencychanged);
			StringBuilder stringBuilder = new StringBuilder();
			if (BoughtList.Count<Defcountpair>() > 0)
			{
				ASTrade.ReportAlign(string.Format("{0,-20} {1,-8}", "RWAutoSell.BargainReportBought".Translate(), "RWAutoSell.Quantity".Translate()).Trim(), ref stringBuilder, BoughtList);
			}
			StringBuilder stringBuilder2 = new StringBuilder();
			if (SoldList.Count<Defcountpair>() > 0)
			{
				ASTrade.ReportAlign(string.Format("{0,-20} {1,-8}", "RWAutoSell.BargainReportSold".Translate(), "RWAutoSell.Quantity".Translate()).Trim(), ref stringBuilder2, SoldList);
			}
			GlobalTargetInfo target = new GlobalTargetInfo(location, Negotiator.Map, false);
			string value2 = "";
			if (ship != null)
			{
				value2 = ship.TraderName;
			}
			string str = "RWAutoSell.BargainReport".Translate(Negotiator.Name.ToStringShort, value, string.Format("{0:N0}", num.ToString()), stringBuilder2.ToString(), stringBuilder.ToString());
			Find.LetterStack.ReceiveLetter("RWAutoSell.BargainReportTitle".Translate(value2), str, LetterDefOf.PositiveEvent, target, null, null, null, null, 0, true);
		}

		// Token: 0x0400004B RID: 75
		public static List<string> CurrencyDefs = new List<string>();

		// Token: 0x0400004C RID: 76
		private static ASMapComp currentcomp = null;

		// Token: 0x0400004D RID: 77
		private static List<TransferableOneWay> ColonyTradeables;

		// Token: 0x0400004E RID: 78
		[Nullable(new byte[]
		{
			1,
			0,
			1,
			1,
			1
		})]
		private static List<Pair<ASRule, List<TransferableOneWay>>> UnavaliblePairs;

		// Token: 0x0400004F RID: 79
		private static bool IsCaravan = false;

		// Token: 0x02000061 RID: 97
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x040001B4 RID: 436
			[Nullable(new byte[]
			{
				0,
				1,
				1,
				1
			})]
			public static Action<List<TransferableOneWay>, ASRule> <0>__DoColony;

			// Token: 0x040001B5 RID: 437
			[Nullable(new byte[]
			{
				0,
				1,
				1,
				1
			})]
			public static Action<RuleArgs<Tradeable, ASRule>> <1>__DoRuleMulti;
		}
	}
}
