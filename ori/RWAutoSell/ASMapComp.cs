using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RimWorld;
using RWASFilterLib;
using Verse;

namespace RWAutoSell
{
	// Token: 0x02000014 RID: 20
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1,
		1
	})]
	public class ASMapComp : ShareableRuledMapComp<ASRule, ASGameComp>
	{
		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000094 RID: 148 RVA: 0x00006F0C File Offset: 0x0000510C
		public ASMapSettingComp MSettings
		{
			get
			{
				ASGameComp getSingleton = ASGameComp.GetSingleton;
				if (getSingleton.sharedcomp)
				{
					return getSingleton.Globalsettings;
				}
				return this.MapSettings;
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000095 RID: 149 RVA: 0x00006F34 File Offset: 0x00005134
		public override int Priority
		{
			get
			{
				return int.MinValue;
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000096 RID: 150 RVA: 0x00006F3B File Offset: 0x0000513B
		public override bool ChainsAllowed
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000097 RID: 151 RVA: 0x00006F3E File Offset: 0x0000513E
		public override bool SortingAllowed
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000098 RID: 152 RVA: 0x00006F41 File Offset: 0x00005141
		public override string TabName
		{
			get
			{
				return ASMapComp.name.Translate();
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000099 RID: 153 RVA: 0x00006F52 File Offset: 0x00005152
		public override string OptionName
		{
			get
			{
				return "RWAutoSell.TradeBehaviour".Translate();
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600009A RID: 154 RVA: 0x00006F63 File Offset: 0x00005163
		public int Tile
		{
			get
			{
				return this.map.Tile;
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600009B RID: 155 RVA: 0x00006F75 File Offset: 0x00005175
		// (set) Token: 0x0600009C RID: 156 RVA: 0x00006F7D File Offset: 0x0000517D
		public bool HasNewShip { get; set; } = true;

		// Token: 0x0600009D RID: 157 RVA: 0x00006F86 File Offset: 0x00005186
		public override Window OptionsWindow()
		{
			return new ASDialog_Behaviour(this.map);
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00006F93 File Offset: 0x00005193
		public static ASMapComp GetSingleton(Map map)
		{
			return map.GetComponent<ASMapComp>();
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00006F9B File Offset: 0x0000519B
		public static ASMapComp GetSingleton()
		{
			return Find.CurrentMap.GetComponent<ASMapComp>();
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00006FA7 File Offset: 0x000051A7
		public override void MapGenerated()
		{
			base.MapGenerated();
			if (this.map.IsPlayerHome)
			{
				ASMod.GetSingleton.mapComps.Add(this);
			}
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00006FCC File Offset: 0x000051CC
		public override void MapRemoved()
		{
			base.MapRemoved();
			if (this.map.IsPlayerHome)
			{
				ASMod.GetSingleton.mapComps.Remove(this);
			}
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x00006FF2 File Offset: 0x000051F2
		public ASMapComp(Map map) : base(map)
		{
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00007020 File Offset: 0x00005220
		private void DelayCheck()
		{
			int num = 0;
			foreach (PassingShip passingShip in this.map.passingShipManager.passingShips)
			{
				num = Math.Max(num, passingShip.ticksUntilDeparture);
			}
			this.ticks = num + 10;
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00007090 File Offset: 0x00005290
		private void BestMapNegogiator()
		{
			this.Negogiator = null;
			float num = -1f;
			foreach (Pawn pawn in this.map.mapPawns.FreeColonists)
			{
				if (!pawn.Dead && !pawn.Downed && !pawn.InMentalState && !StatDefOf.TradePriceImprovement.Worker.IsDisabledFor(pawn) && pawn.workSettings.WorkIsActive(WorkTypeDefOf.Warden))
				{
					float statValue = pawn.GetStatValue(StatDefOf.TradePriceImprovement, true, -1);
					if (this.Negogiator == null || statValue > num)
					{
						this.Negogiator = pawn;
						num = statValue;
					}
				}
			}
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00007154 File Offset: 0x00005354
		public override void MapComponentTick()
		{
			base.MapComponentTick();
			if (this.map.IsPlayerHome)
			{
				int num = -1;
				for (int i = 0; i < this.trader.Count; i++)
				{
					this.trader[i].DoTick();
					if (this.trader[i].CanRemove)
					{
						num = i;
					}
				}
				if (num >= 0)
				{
					this.trader.RemoveAt(num);
				}
				if (this.negogiatortick < 0)
				{
					this.BestMapNegogiator();
					this.negogiatortick = 1800;
				}
				if (this.ticks < 0)
				{
					if (this.map.passingShipManager.passingShips.Count == 0)
					{
						this.HasNewShip = false;
						this.LastShipCount = 0;
						this.ticks = 2400;
					}
					else
					{
						if (this.map.passingShipManager.passingShips.Count > this.LastShipCount)
						{
							this.HasNewShip = true;
						}
						this.LastShipCount = this.map.passingShipManager.passingShips.Count;
						this.ticks = 1200;
					}
				}
				this.ticks--;
				this.negogiatortick--;
			}
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00007280 File Offset: 0x00005480
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<bool>(ref this.MapSettings.ASPartTrade, "AllowPartialStacks", true, false);
			Scribe_Values.Look<bool>(ref this.MapSettings.ASStackBeh, "SmartStackBehavior", true, false);
			Scribe_Values.Look<bool>(ref this.MapSettings.ASAITog, "AllowAI", false, false);
			Scribe_Values.Look<bool>(ref this.MapSettings.IncludeUnavalible, "IncludeUnavalible", false, false);
			Scribe_Values.Look<bool>(ref this.MapSettings.IncEquip, "IncEquip", false, false);
			Scribe_Values.Look<bool>(ref this.MapSettings.IncInv, "IncInventory", false, false);
			Scribe_Values.Look<bool>(ref this.MapSettings.CustomShopping, "CustomShopping", false, false);
			Scribe_Values.Look<bool>(ref this.MapSettings.FractionCheck, "FractionCheck", false, false);
			Scribe_Values.Look<bool>(ref this.MapSettings.NotifyZeroTrades, "NotifyZeroTrades", false, false);
			Scribe_Values.Look<bool>(ref this.MapSettings.NotifyTrades, "NotifyTrades", true, false);
			Scribe_Values.Look<int>(ref this.MapSettings.Repitition, "Repitition", 1, false);
			bool hasNewShip = this.HasNewShip;
			Scribe_Values.Look<bool>(ref hasNewShip, "NewShip", false, false);
			this.HasNewShip = hasNewShip;
			Scribe_Collections.Look<string>(ref this.MapSettings.SelectedNegogiators, "CustomNegogiators", LookMode.Undefined, Array.Empty<object>());
			Scribe_Collections.Look<ASRule>(ref this.rules_, "ASRules", LookMode.Deep, Array.Empty<object>());
			if (Scribe.mode == LoadSaveMode.PostLoadInit && this.MapSettings.SelectedNegogiators == null)
			{
				this.MapSettings.SelectedNegogiators = new List<string>();
			}
		}

		// Token: 0x04000068 RID: 104
		private static string name = "RWAutoSell.TabTrading";

		// Token: 0x04000069 RID: 105
		private ASMapSettingComp MapSettings = new ASMapSettingComp();

		// Token: 0x0400006A RID: 106
		private int negogiatortick;

		// Token: 0x0400006B RID: 107
		private int LastShipCount;

		// Token: 0x0400006C RID: 108
		public static bool RefreshCheck = false;

		// Token: 0x0400006D RID: 109
		public Pawn Negogiator;

		// Token: 0x0400006E RID: 110
		public List<ASTraderCount> trader = new List<ASTraderCount>();

		// Token: 0x04000070 RID: 112
		private int ticks = 120;
	}
}
