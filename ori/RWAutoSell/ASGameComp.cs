using System;
using System.Runtime.CompilerServices;
using System.Threading;
using RWASFilterLib;
using Verse;

namespace RWAutoSell
{
	// Token: 0x02000012 RID: 18
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public class ASGameComp : ASLibBaseGameComp<ASRule>
	{
		// Token: 0x0600008C RID: 140 RVA: 0x00006C13 File Offset: 0x00004E13
		public ASGameComp(Game game)
		{
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600008D RID: 141 RVA: 0x00006C26 File Offset: 0x00004E26
		public static ASGameComp GetSingleton
		{
			get
			{
				return (ASGameComp)Current.Game.GetComponent(typeof(ASGameComp));
			}
		}

		// Token: 0x0600008E RID: 142 RVA: 0x00006C41 File Offset: 0x00004E41
		public override void FinalizeInit()
		{
			base.FinalizeInit();
			ASLibMod.GetSingleton.StartThread(new Action[]
			{
				new Action(ASMod.GetSingleton.BindWindows),
				new Action(this.CurrencyThreadStart)
			});
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00006C7B File Offset: 0x00004E7B
		public void CurrencyThreadStart()
		{
			ThreadStart start;
			if ((start = ASGameComp.<>O.<0>__GetCurrencies) == null)
			{
				start = (ASGameComp.<>O.<0>__GetCurrencies = new ThreadStart(ASTrade.GetCurrencies));
			}
			new Thread(start).Start();
		}

		// Token: 0x06000090 RID: 144 RVA: 0x00006CA4 File Offset: 0x00004EA4
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<bool>(ref this.showadv, "ShowAdvanced", false, false);
			Scribe_Deep.Look<ASMapSettingComp>(ref this.Globalsettings, "GSettings", Array.Empty<object>());
			if (Scribe.mode == LoadSaveMode.PostLoadInit && this.Globalsettings == null)
			{
				this.Globalsettings = new ASMapSettingComp();
			}
		}

		// Token: 0x0400005A RID: 90
		public bool showadv;

		// Token: 0x0400005B RID: 91
		public ASMapSettingComp Globalsettings = new ASMapSettingComp();

		// Token: 0x02000066 RID: 102
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x040001C1 RID: 449
			[Nullable(0)]
			public static ThreadStart <0>__GetCurrencies;
		}
	}
}
