using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using RimWorld;
using RWASFilterLib;
using RWASWidgets;
using UnityEngine;
using Verse;

namespace RWAutoSell
{
	// Token: 0x0200001D RID: 29
	[NullableContext(1)]
	[Nullable(0)]
	internal class ASDialog_Import : Window
	{
		// Token: 0x060000D6 RID: 214 RVA: 0x00009695 File Offset: 0x00007895
		public ASDialog_Import(IRuleComp Comp) : base(null)
		{
			this.Comp = Comp;
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060000D7 RID: 215 RVA: 0x000096AC File Offset: 0x000078AC
		public override Vector2 InitialSize
		{
			get
			{
				return new Vector2(700f, 500f);
			}
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x000096C0 File Offset: 0x000078C0
		public override void PostOpen()
		{
			this.box = new ASChangeBox<string>(25f, 4f, false, false, true, true);
			this.boxmap = new ASListBox<IList>(25f, 4f, false, false);
			this.boxRules = new ASMultiBox<IRule>(25f, 4f, true, false);
			this.box.Clear();
			this.boxmap.Clear();
			this.boxRules.Clear();
			this.box.Add("RWAutoSell.ImCurrent".Translate());
			this.box.AddRange(ASImport.ListSaves());
			this.box.OnClick += this.OnBoxClickEvent;
			this.box.OnRightClick += this.Box_OnRightClick;
			this.box.OnPostRightClick += this.Box_OnPostRightClick;
			this.boxmap.OnClick += this.OnClickImport;
			base.PostOpen();
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x000097C4 File Offset: 0x000079C4
		private void Box_OnPostRightClick(IGUIList<string> sender, GUIListEventArgs e)
		{
			if (this.itemind != -1)
			{
				if (e.index == -1)
				{
					if (this.itemind == 0 && this.delitem.Item == null)
					{
						this.box.Insert(0, this.delitem);
					}
					else
					{
						FileInfo fileInfo = new FileInfo(this.delitem.Item);
						if (fileInfo.Directory.Name == "ASExports")
						{
							fileInfo.Delete();
						}
						else
						{
							this.box.Insert(this.itemind, this.delitem);
						}
					}
				}
				else
				{
					sender.RemoveAt(e.index);
					string text = this.DoExport();
					if (text != string.Empty)
					{
						try
						{
							FileInfo fileInfo2 = new FileInfo(text);
							SelectedItem<string> item = new SelectedItem<string>
							{
								Item = fileInfo2.FullName,
								label = fileInfo2.Name + " " + VersionControl.CurrentVersionStringWithRev
							};
							sender.Insert(1, item);
						}
						catch (Exception ex)
						{
							Log.Warning("Could Not Add or file error: " + ex.ToString());
						}
					}
				}
				this.itemind = -1;
				this.delitem = null;
			}
		}

		// Token: 0x060000DA RID: 218 RVA: 0x000098F4 File Offset: 0x00007AF4
		private void Box_OnRightClick(IGUIList<string> sender, GUIListEventArgs e)
		{
			if (e.index != -1)
			{
				this.delitem = sender[e.index];
				this.itemind = e.index;
			}
		}

		// Token: 0x060000DB RID: 219 RVA: 0x00009920 File Offset: 0x00007B20
		private void OnBoxClickEvent(IGUIList<string> sender, GUIListEventArgs e)
		{
			this.boxRules.Clear();
			this.boxmap.Clear();
			if (e.index == 0)
			{
				using (List<Map>.Enumerator enumerator = Find.Maps.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Map map = enumerator.Current;
						if (map.ParentFaction == Faction.OfPlayer && map.Parent.HasMap)
						{
							Type type = this.Comp.GetType();
							IRuleComp ruleComp = map.GetComponent(type) as IRuleComp;
							IList list = new List<IRule>();
							foreach (IRule rule in ruleComp.EnumerateRules())
							{
								list.Add(rule.DeepCopy());
							}
							if (list.Count > 0)
							{
								string str = "RWAutoSell.Unnamed".Translate();
								if (map.Parent.HasName)
								{
									str = map.Parent.LabelCap;
								}
								this.boxmap.Add(new SelectedItem<IList>
								{
									surpresslabelupdate = true,
									Item = list,
									label = str + " " + list.Count.ToString()
								});
							}
						}
					}
					return;
				}
			}
			ASImport.ProcessSave(sender[e.index].Item, this.Comp);
			this.boxmap.AddRange(ASImport.CacheMaps.Select(delegate(Pair<string, IList> el)
			{
				SelectedItem<IList> selectedItem = new SelectedItem<IList>();
				selectedItem.surpresslabelupdate = true;
				Pair<string, IList> pair = el;
				selectedItem.Item = pair.Second;
				pair = el;
				string first = pair.First;
				string str2 = " ";
				pair = el;
				selectedItem.label = first + str2 + pair.Second.Count.ToString();
				return selectedItem;
			}));
		}

		// Token: 0x060000DC RID: 220 RVA: 0x00009AF4 File Offset: 0x00007CF4
		private void OnClickImport(IGUIList<IList> sender, GUIListEventArgs e)
		{
			this.boxRules.Clear();
			foreach (object obj in sender.GetSelected)
			{
				IRule item = (IRule)obj;
				SelectedItem<IRule> item2 = new SelectedItem<IRule>
				{
					Item = item,
					Selected = true
				};
				this.boxRules.Add(item2);
			}
		}

		// Token: 0x060000DD RID: 221 RVA: 0x00009B74 File Offset: 0x00007D74
		private string DoExport()
		{
			string text = Path.Combine(GenFilePaths.ConfigFolderPath, "..", "ASExports");
			string text4;
			try
			{
				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
				}
				string text2 = "Export_" + Find.FactionManager.OfPlayer.Name;
				string text3 = string.Empty;
				foreach (char c in text2)
				{
					if (!Path.GetInvalidFileNameChars().Contains(c))
					{
						ReadOnlySpan<char> str = text3;
						char c2 = c;
						text3 = str + new ReadOnlySpan<char>(ref c2);
					}
				}
				text2 = text3;
				if (text2.Length > 64)
				{
					text2 = text2.Substring(0, 64);
				}
				int num = 0;
				text2 += "0.xml";
				text2 = Path.Combine(text, text2);
				Log.Message(text2);
				if (File.Exists(text2))
				{
					do
					{
						text2 = text2.Substring(0, text2.Length - (4 + num.ToString().Length));
						num++;
						text2 = text2 + num.ToString() + ".xml";
					}
					while (File.Exists(text2));
				}
				ASImport.ExportRules(text2);
				text4 = text2;
			}
			catch
			{
				Log.Warning("file create error");
				text4 = string.Empty;
			}
			return text4;
		}

		// Token: 0x060000DE RID: 222 RVA: 0x00009CBC File Offset: 0x00007EBC
		public override void DoWindowContents(Rect inRect)
		{
			ASTable astable = new ASTable(new Rect(inRect.x, inRect.y + 10f, inRect.width, inRect.height - 60f), 3, 1, 4f, 0f, true);
			this.box.DrawListBox(astable.GetRectangle(0, 0, -1, -1, false));
			this.boxmap.DrawListBox(astable.GetRectangle(1, 0, -1, -1, false));
			this.boxRules.DrawListBox(astable.GetRectangle(2, 0, -1, -1, false));
			if (Widgets.ButtonText(new Rect(inRect.width / 2f - 165f, inRect.yMax - 40f, 160f, 40f), "AcceptButton".Translate(), true, true, true, null))
			{
				this.OnAcceptKeyPressed();
				this.Close(true);
			}
			if (Widgets.ButtonText(new Rect(inRect.width / 2f + 5f, inRect.yMax - 40f, 160f, 40f), "CancelButton".Translate(), true, true, true, null))
			{
				this.Close(true);
			}
		}

		// Token: 0x060000DF RID: 223 RVA: 0x00009E04 File Offset: 0x00008004
		public override void OnAcceptKeyPressed()
		{
			foreach (IRule rule in this.boxRules.GetSelectedItems)
			{
				this.Comp.Add(rule);
			}
			MainTabWindow_AutoSell mainTabWindow_AutoSell = Find.WindowStack.WindowOfType<MainTabWindow_AutoSell>();
			if (mainTabWindow_AutoSell != null)
			{
				mainTabWindow_AutoSell.ChangeList(null, null);
			}
			base.OnAcceptKeyPressed();
		}

		// Token: 0x04000093 RID: 147
		private ASChangeBox<string> box;

		// Token: 0x04000094 RID: 148
		private ASListBox<IList> boxmap;

		// Token: 0x04000095 RID: 149
		private ASMultiBox<IRule> boxRules;

		// Token: 0x04000096 RID: 150
		private IRuleComp Comp;

		// Token: 0x04000097 RID: 151
		private SelectedItem<string> delitem;

		// Token: 0x04000098 RID: 152
		private int itemind = -1;
	}
}
