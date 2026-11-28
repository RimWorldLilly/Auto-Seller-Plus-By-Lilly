using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using RimWorld;
using RWASWidgets;
using UnityEngine;
using Verse;

namespace RWAutoSell.Filters
{
	// Token: 0x0200002F RID: 47
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class DoerBoxWorker
	{
		// Token: 0x1700003A RID: 58
		// (get) Token: 0x0600015F RID: 351 RVA: 0x0000EAFC File Offset: 0x0000CCFC
		protected static List<DoerBoxWorker> DoerWorkers
		{
			get
			{
				if (DoerBoxWorker.DoersWorkers_ == null)
				{
					DoerBoxWorker.DoersWorkers_ = new List<DoerBoxWorker>();
					string str = "";
					foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
					{
						try
						{
							foreach (Type type in from tp in assembly.GetExportedTypes()
							where !tp.IsAbstract && !tp.IsInterface && typeof(DoerBoxWorker).IsAssignableFrom(tp)
							select tp)
							{
								str = type.FullName;
								DoerBoxWorker item = (DoerBoxWorker)Activator.CreateInstance(type);
								DoerBoxWorker.DoersWorkers_.Add(item);
							}
						}
						catch
						{
							Log.Message("activate failed on " + str);
						}
					}
				}
				return DoerBoxWorker.DoersWorkers_;
			}
		}

		// Token: 0x06000160 RID: 352 RVA: 0x0000EBEC File Offset: 0x0000CDEC
		protected SelectedItem<string> Createboxitem(Def def, string[] list, string label = "")
		{
			string text = label;
			if (text == "")
			{
				text = def.LabelCap;
			}
			bool selected = false;
			if (list != null)
			{
				selected = list.Contains(def.defName);
			}
			return new SelectedItem<string>
			{
				Item = def.defName,
				label = text,
				surpresslabelupdate = true,
				Selected = selected,
				tooltip = def.description
			};
		}

		// Token: 0x06000161 RID: 353 RVA: 0x0000EC60 File Offset: 0x0000CE60
		protected static DoerBoxWorker DoerFromType(string s)
		{
			foreach (DoerBoxWorker doerBoxWorker in DoerBoxWorker.DoersWorkers_)
			{
				if (doerBoxWorker.Doerclass == s)
				{
					return doerBoxWorker;
				}
			}
			return null;
		}

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x06000162 RID: 354 RVA: 0x0000ECC0 File Offset: 0x0000CEC0
		public virtual string Doerclass
		{
			get
			{
				return string.Empty;
			}
		}

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x06000163 RID: 355 RVA: 0x0000ECC7 File Offset: 0x0000CEC7
		public virtual string DoerLabel
		{
			get
			{
				return string.Empty;
			}
		}

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x06000164 RID: 356 RVA: 0x0000ECCE File Offset: 0x0000CECE
		public virtual int Expandedsize
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x06000165 RID: 357 RVA: 0x0000ECD1 File Offset: 0x0000CED1
		// (set) Token: 0x06000166 RID: 358 RVA: 0x0000ECD9 File Offset: 0x0000CED9
		public bool Checked
		{
			get
			{
				return this.checked_;
			}
			set
			{
				this.checked_ = value;
			}
		}

		// Token: 0x06000167 RID: 359 RVA: 0x0000ECE2 File Offset: 0x0000CEE2
		public bool HasBox()
		{
			return this.Checked && this.Box != null;
		}

		// Token: 0x06000168 RID: 360 RVA: 0x0000ECF8 File Offset: 0x0000CEF8
		public static SelectedItem<DoerBoxWorker> Create(Type doer, List<string> Data, List<SelectedItem<DoerBoxWorker>> Exisiting)
		{
			if (Exisiting.Any((SelectedItem<DoerBoxWorker> x) => x.Item.Doerclass == doer.FullName))
			{
				return Exisiting.First((SelectedItem<DoerBoxWorker> x) => x.Item.Doerclass == doer.FullName);
			}
			DoerBoxWorker doerBoxWorker;
			if (DoerBoxWorker.DoerWorkers.Any((DoerBoxWorker x) => x.Doerclass == doer.FullName))
			{
				doerBoxWorker = DoerBoxWorker.DoerWorkers.First((DoerBoxWorker x) => x.Doerclass == doer.FullName);
			}
			else
			{
				DoerBoxWorker_Unknown doerBoxWorker_Unknown = new DoerBoxWorker_Unknown();
				doerBoxWorker_Unknown.SetUnknownClass(doer);
				doerBoxWorker = doerBoxWorker_Unknown;
			}
			SelectedItem<DoerBoxWorker> selectedItem = new SelectedItem<DoerBoxWorker>();
			selectedItem.Item = doerBoxWorker;
			selectedItem.label = doerBoxWorker.ToString();
			selectedItem.surpresslabelupdate = true;
			doerBoxWorker.InitSettings(Data);
			return selectedItem;
		}

		// Token: 0x06000169 RID: 361 RVA: 0x0000EDA4 File Offset: 0x0000CFA4
		protected bool CmpSet<[Nullable(2)] T>(string Cmp, List<T> list, Func<T, bool> func)
		{
			if (!(Cmp == "any"))
			{
				return list.All(func);
			}
			return list.Any(func);
		}

		// Token: 0x0600016A RID: 362 RVA: 0x0000EDC2 File Offset: 0x0000CFC2
		public DoerBoxWorker()
		{
		}

		// Token: 0x0600016B RID: 363 RVA: 0x0000EDE0 File Offset: 0x0000CFE0
		public void InitSettings(List<string> Data)
		{
			this.Settings.Clear();
			if (this.Box != null)
			{
				this.Box.Clear();
			}
			string[] array = Data.AllStartsWith(this.Doerclass + "#");
			if (array != null)
			{
				this.Checked = array.Contains("ckd");
			}
			else
			{
				this.Checked = false;
			}
			this.DoerSettingsWorker(array);
		}

		// Token: 0x0600016C RID: 364 RVA: 0x0000EE4C File Offset: 0x0000D04C
		public static bool CheckWorkers(Book book, List<string> Data)
		{
			string[] array = Data.AllUniqueStartStrings('#');
			for (int i = 0; i < array.Length; i++)
			{
				string dd = array[i];
				string[] array2 = Data.AllSubString(dd + "#");
				DoerBoxWorker doerBoxWorker = DoerBoxWorker.DoerFromType(dd);
				if (array2.Contains("ckd"))
				{
					if (doerBoxWorker == null)
					{
						return false;
					}
					if (book.BookComp.Doers.Count((BookOutcomeDoer x) => x.GetType().FullName == dd) == 0)
					{
						return false;
					}
					if (!doerBoxWorker.CheckDoerWorker(book, array2))
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x0600016D RID: 365 RVA: 0x0000EEEC File Offset: 0x0000D0EC
		public virtual void DoerSettingsWorker(string[] rvtlist)
		{
		}

		// Token: 0x0600016E RID: 366 RVA: 0x0000EEEE File Offset: 0x0000D0EE
		public virtual bool CheckDoerWorker(Book b, string[] Data)
		{
			return false;
		}

		// Token: 0x0600016F RID: 367 RVA: 0x0000EEF4 File Offset: 0x0000D0F4
		private T[] GetValues<[Nullable(0)] T, [Nullable(0)] E>(E doer) where T : Def where E : BookOutcomeDoer
		{
			List<T> list = new List<T>();
			try
			{
				foreach (T item in ((Dictionary<T, float>)typeof(E).GetField("values", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(doer)).Keys)
				{
					list.Add(item);
				}
			}
			catch
			{
				Log.Message("could not grab doer value: " + doer.GetType().Name);
			}
			return list.ToArray();
		}

		// Token: 0x06000170 RID: 368 RVA: 0x0000EFA8 File Offset: 0x0000D1A8
		protected List<T> CheckValueList<[Nullable(0)] T, [Nullable(0)] E>(Book book) where T : Def where E : BookOutcomeDoer
		{
			HashSet<T> hashSet = new HashSet<T>();
			foreach (E doer in from el in book.BookComp.Doers
			where el.GetType().FullName == typeof(E).FullName
			select (E)((object)el))
			{
				foreach (T item in this.GetValues<T, E>(doer))
				{
					hashSet.Add(item);
				}
			}
			return hashSet.ToList<T>();
		}

		// Token: 0x06000171 RID: 369 RVA: 0x0000F074 File Offset: 0x0000D274
		public int Draw(Rect rect, int maxheight)
		{
			int num = 30;
			ASRectMonitor asrectMonitor = new ASRectMonitor(new Rect(rect.x, rect.y, rect.width, (float)maxheight));
			string text = this.DoerLabel;
			if (text != this.Doerclass && text != string.Empty && text.CanTranslate())
			{
				text = this.DoerLabel.Translate();
			}
			ASWidgets.ASCheckbox(asrectMonitor.NextFromTop(30f), text, ref this.checked_);
			if (this.Checked)
			{
				asrectMonitor.NextFromLeft(30f);
				int num2 = 0;
				string[] us = this.Settingsdefault.AllUniqueStartStrings('.');
				if (us != null && us.Length != 0)
				{
					IEnumerable<SettingSettings> settingsNames = DoerBoxWorker.SettingsNames;
					Func<SettingSettings, bool> predicate;
					Func<SettingSettings, bool> <>9__0;
					if ((predicate = <>9__0) == null)
					{
						predicate = (<>9__0 = ((SettingSettings el) => us.Any((string x) => x + "." == el.DatVal)));
					}
					foreach (SettingSettings settingSettings in settingsNames.Where(predicate))
					{
						num2 += settingSettings.DrawRow(ref asrectMonitor, this.Settings, this);
					}
				}
				num += num2;
				if (this.Box != null)
				{
					this.Box.DrawListBox(asrectMonitor.Remaining());
					num += (int)asrectMonitor.Remaining().height;
				}
			}
			return num;
		}

		// Token: 0x06000172 RID: 370 RVA: 0x0000F1F4 File Offset: 0x0000D3F4
		public List<string> RenderStop()
		{
			List<string> list = new List<string>();
			if (this.Checked)
			{
				string str = this.Doerclass + "#";
				list.Add(str + "ckd");
				if (this.Settings != null && this.Settings.Count<string>() > 0)
				{
					foreach (string text in this.Settings)
					{
						if (!this.Settingsdefault.Contains(text))
						{
							list.Add(str + text);
						}
					}
				}
				if (this.HasBox() && this.Box != null)
				{
					foreach (SelectedItem<string> selectedItem in this.Box)
					{
						if (selectedItem.Selected)
						{
							list.Add(str + "box." + selectedItem.Item);
						}
					}
				}
			}
			return list;
		}

		// Token: 0x06000173 RID: 371 RVA: 0x0000F314 File Offset: 0x0000D514
		protected bool CheckBox<[Nullable(0)] T>(List<T> boxlist, bool multi, string[] Data) where T : Def
		{
			string[] array = Data.AllSubString("box.");
			if (array != null && array.Count<string>() > 0)
			{
				int num = 0;
				foreach (T t in boxlist)
				{
					if (array.Contains(t.defName))
					{
						num++;
					}
				}
				if (num == 0)
				{
					return false;
				}
				if (!multi)
				{
					return true;
				}
				if (boxlist.Count<T>() == num)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000174 RID: 372 RVA: 0x0000F3A8 File Offset: 0x0000D5A8
		protected void PopBox<[Nullable(0)] T>(string[] rvtlist, Func<T, bool> enProcessor = null, Func<T, string> strProcessor = null) where T : Def
		{
			if (enProcessor == null)
			{
				enProcessor = ((T x) => true);
			}
			if (strProcessor == null)
			{
				strProcessor = ((T x) => x.LabelCap);
			}
			IEnumerable<T> allDefs = DefDatabase<T>.AllDefs;
			Func<T, bool> <>9__2;
			Func<T, bool> predicate;
			if ((predicate = <>9__2) == null)
			{
				predicate = (<>9__2 = ((T el) => enProcessor(el)));
			}
			foreach (T t in allDefs.Where(predicate))
			{
				SelectedItem<string> selectedItem = this.Createboxitem(t, rvtlist, strProcessor(t));
				selectedItem.Selected = (rvtlist.Contains("box." + selectedItem.Item) > false);
				this.Box.Add(selectedItem);
			}
		}

		// Token: 0x06000175 RID: 373 RVA: 0x0000F4B8 File Offset: 0x0000D6B8
		public override string ToString()
		{
			return this.DoerLabel;
		}

		// Token: 0x040000EB RID: 235
		private static List<DoerBoxWorker> DoersWorkers_ = null;

		// Token: 0x040000EC RID: 236
		private bool checked_;

		// Token: 0x040000ED RID: 237
		protected List<string> Settingsdefault = new List<string>();

		// Token: 0x040000EE RID: 238
		protected List<string> Settings = new List<string>();

		// Token: 0x040000EF RID: 239
		protected static List<SettingSettings> SettingsNames = new List<SettingSettings>
		{
			new SettingSettings("Joy", "joy.", SettingSettings.SettingType.Range, "float", "singlepercent", "2.5", "0"),
			new SettingSettings("Mental", "mbk.", SettingSettings.SettingType.Range, "float", "singlepercent", "0.2", "0"),
			new SettingSettings("Comparison", "cmp.", SettingSettings.SettingType.Radio, "string", "any:RWAutoSell.Any,all:RWAutoSell.All", "", ""),
			new SettingSettings("RWAutoSell.FilterLearningKnown", "unl.", SettingSettings.SettingType.Check, "bool", "", "", ""),
			new SettingSettings("RWAutoSell.FilterLearningTP", "tpr.", SettingSettings.SettingType.Check, "bool", "", "", ""),
			new SettingSettings("RWAutoSell.FilterLearningUndisc", "und.", SettingSettings.SettingType.Check, "bool", "", "", ""),
			new SettingSettings("RWAutoSell.FilterLearningCompleted", "rnc.", SettingSettings.SettingType.Check, "bool", "", "", "")
		};

		// Token: 0x040000F0 RID: 240
		protected ASMultiBox<string> Box;
	}
}
