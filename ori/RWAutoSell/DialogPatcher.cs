using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace RWAutoSell
{
	// Token: 0x02000019 RID: 25
	[NullableContext(1)]
	[Nullable(0)]
	public static class DialogPatcher
	{
		// Token: 0x060000B9 RID: 185 RVA: 0x00007F94 File Offset: 0x00006194
		public static void DrawRow(Rect rect, Tradeable trad, int index)
		{
			if (trad == null || trad.IsFavor || !trad.HasAnyThing || trad.ThingDef == null || trad.ThingDef == ThingDefOf.Silver || !TradeSession.Active || (TradeSession.playerNegotiator != null && TradeSession.playerNegotiator.GetCaravan() != null))
			{
				TradeUI.DrawTradeableRow(rect, trad, index);
				return;
			}
			if (Widgets.ButtonImage(new Rect(rect.x, rect.y + rect.height * 0.05f, rect.height * 0.9f, rect.height * 0.9f), ASTextures.ASyx, true, null))
			{
				Find.WindowStack.Add(new ASDialog_Trade2Rule(trad));
			}
			TradeUI.DrawTradeableRow(new Rect(rect.x + rect.height, rect.y, rect.width - rect.height, rect.height), trad, index);
		}

		// Token: 0x060000BA RID: 186 RVA: 0x00008080 File Offset: 0x00006280
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo right = AccessTools.Method(typeof(TradeUI), "DrawTradeableRow", null, null);
			MethodInfo method = typeof(DialogPatcher).GetMethod("DrawRow");
			List<CodeInstruction> list = new List<CodeInstruction>(instructions);
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].opcode == OpCodes.Call)
				{
					MethodInfo left = list[i].operand as MethodInfo;
					if (left != null && left == right)
					{
						list[i].operand = method;
					}
				}
			}
			return list.AsEnumerable<CodeInstruction>();
		}

		// Token: 0x060000BB RID: 187 RVA: 0x00008124 File Offset: 0x00006324
		public static IEnumerable<CodeInstruction> DWCTranspiler(IEnumerable<CodeInstruction> instructions)
		{
			List<CodeInstruction> list = new List<CodeInstruction>(instructions);
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].opcode == OpCodes.Ldc_R4)
				{
					float? num = list[i].operand as float?;
					if (num != null)
					{
						float? num2 = num;
						float num3 = (float)38;
						if (num2.GetValueOrDefault() == num3 & num2 != null)
						{
							list[i].operand = 78f;
						}
					}
				}
			}
			return list.AsEnumerable<CodeInstruction>();
		}

		// Token: 0x060000BC RID: 188 RVA: 0x000081BC File Offset: 0x000063BC
		public static void PostDoWindowContents(Window __instance, Rect inRect)
		{
			float num = inRect.height + DialogPatcher.offset - 55f;
			if (__instance.GetType().FullName == "DynamicTradeInterface.UserInterface.Window_DynamicTrade")
			{
				num += 10f;
			}
			else
			{
				num -= 40f;
			}
			Color color = GUI.color;
			GUI.color = Color.white;
			Rect rect = new Rect(10f, num, 40f, 40f);
			if (Mouse.IsOver(rect))
			{
				Color color2 = GUI.color;
				GUI.color = GenUI.MouseoverColor;
				Widgets.DrawTextureFitted(rect, ASTextures.ASyx, 1f, 1f);
				GUI.color = color2;
			}
			else
			{
				Widgets.DrawTextureFitted(rect, ASTextures.ASyx, 1f, 1f);
			}
			if (Widgets.ButtonInvisible(rect, true) && TradeSession.Active)
			{
				DialogPatcher.postdo();
			}
			GUI.color = color;
		}

		// Token: 0x060000BD RID: 189 RVA: 0x00008290 File Offset: 0x00006490
		public static void postdo()
		{
			Caravan caravan = TradeSession.playerNegotiator.GetCaravan();
			List<Defcountpair> list = new List<Defcountpair>();
			List<Defcountpair> list2 = new List<Defcountpair>();
			if (caravan == null)
			{
				DialogPatcher.offset = 0f;
				int currencychanged;
				ASTrade.SetTradables(TradeSession.deal.AllTradeables, TradeSession.playerNegotiator.Map, list, list2, out currencychanged, true, false);
				if (Prefs.DevMode && ASMod.GetSettings().devlettershow)
				{
					ASTrade.TradeReport(TradeSession.playerNegotiator, TradeSession.trader, list, list2, currencychanged, TradeSession.playerNegotiator.Position, false);
				}
				return;
			}
			DialogPatcher.offset = 52f;
			List<Thing> list3 = new List<Thing>();
			list3.AddRange(CaravanInventoryUtility.AllInventoryItems(caravan));
			ASShoppingList asshoppingList = list3.FirstOrDefault((Thing x) => x is ASShoppingList) as ASShoppingList;
			if (asshoppingList != null)
			{
				Map origin = asshoppingList.Origin;
				if (origin != null)
				{
					ThingDef namedSilentFail = DefDatabase<ThingDef>.GetNamedSilentFail("ASReportsPrinter");
					ThingDef namedSilentFail2 = DefDatabase<ThingDef>.GetNamedSilentFail("ASReportsLog");
					if ((namedSilentFail != null && origin.listerBuildings.ColonistsHaveBuilding(namedSilentFail)) || (namedSilentFail2 != null && origin.listerBuildings.ColonistsHaveBuilding(namedSilentFail2)))
					{
						int num;
						ASTrade.SetTradables(TradeSession.deal.AllTradeables, origin, list, list2, out num, true, true);
					}
				}
			}
		}

		// Token: 0x060000BE RID: 190 RVA: 0x000083C0 File Offset: 0x000065C0
		public static void PostOpenTrade(Window __instance)
		{
			if (!TradeSession.Active)
			{
				Log.Message("Failed: No Active Session");
				return;
			}
			foreach (WindowFieldContainer windowFieldContainer in ASMod.tpl)
			{
				if (windowFieldContainer.WhiteListed && __instance.GetType() == windowFieldContainer.WindowType)
				{
					DialogPatcher.postdo();
					break;
				}
			}
		}

		// Token: 0x04000085 RID: 133
		private static float offset;
	}
}
