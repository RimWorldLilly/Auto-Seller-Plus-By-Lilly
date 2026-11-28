using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml;
using Ionic.Zlib;
using RimWorld;
using RimWorld.Planet;
using RWASFilterLib;
using RWASWidgets;
using Verse;

namespace RWAutoSell
{
	// Token: 0x02000006 RID: 6
	[NullableContext(1)]
	[Nullable(0)]
	internal static class ASImport
	{
		// Token: 0x06000023 RID: 35 RVA: 0x00002814 File Offset: 0x00000A14
		private static bool ReadVersion(XmlReader reader, List<SelectedItem<string>> valid, string FullName, string Name)
		{
			bool result = true;
			try
			{
				if (reader.ReadToFollowing("gameVersion") && !reader.IsEmptyElement)
				{
					string text = reader.ReadInnerXml();
					if (text.Substring(0, 2) == "1." || text.Substring(0, 4) == "0.19")
					{
						SelectedItem<string> selectedItem = new SelectedItem<string>();
						selectedItem.Item = FullName;
						selectedItem.label = Name + " " + text.Substring(0, text.FirstIndexOf((char x) => char.IsWhiteSpace(x)));
						SelectedItem<string> item = selectedItem;
						valid.Add(item);
					}
				}
			}
			catch
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000024 RID: 36 RVA: 0x000028D8 File Offset: 0x00000AD8
		public static void ExportRules(string savepath)
		{
			XmlWriterSettings settings = new XmlWriterSettings
			{
				Indent = true
			};
			XmlWriter xmlWriter = XmlWriter.Create(savepath, settings);
			xmlWriter.WriteStartDocument();
			xmlWriter.WriteStartElement("savegame");
			xmlWriter.WriteStartElement("meta");
			xmlWriter.WriteElementString("gameVersion", VersionControl.CurrentVersionStringWithRev);
			xmlWriter.WriteEndElement();
			xmlWriter.WriteStartElement("game");
			xmlWriter.WriteElementString("currentMapIndex", "0".ToString());
			xmlWriter.WriteStartElement("scenario");
			xmlWriter.WriteStartElement("playerFaction");
			xmlWriter.WriteElementString("def", "PlayerFaction");
			xmlWriter.WriteElementString("factionDef", Find.FactionManager.OfPlayer.def.defName);
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();
			xmlWriter.WriteStartElement("tickManager");
			xmlWriter.WriteElementString("ticksGame", Find.TickManager.TicksGame.ToString());
			xmlWriter.WriteElementString("gameStartAbsTick", Find.TickManager.gameStartAbsTick.ToString());
			xmlWriter.WriteElementString("startingYear", Find.TickManager.StartingYear.ToString());
			xmlWriter.WriteEndElement();
			xmlWriter.WriteStartElement("world");
			xmlWriter.WriteStartElement("info");
			xmlWriter.WriteElementString("name", Find.World.info.name);
			xmlWriter.WriteEndElement();
			xmlWriter.WriteStartElement("factionManager");
			xmlWriter.WriteStartElement("allFactions");
			xmlWriter.WriteStartElement("li");
			xmlWriter.WriteElementString("def", Find.FactionManager.OfPlayer.def.defName);
			xmlWriter.WriteElementString("name", Find.FactionManager.OfPlayer.Name);
			xmlWriter.WriteElementString("loadID", Find.FactionManager.OfPlayer.loadID.ToString());
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();
			xmlWriter.WriteStartElement("worldObjects");
			xmlWriter.WriteStartElement("worldObjects");
			foreach (WorldObject worldObject in Current.Game.World.worldObjects.MapParents)
			{
				if (worldObject.Faction != null && worldObject.Faction.loadID == Find.FactionManager.OfPlayer.loadID)
				{
					xmlWriter.WriteStartElement("li");
					xmlWriter.WriteAttributeString("Class", "Settlement");
					xmlWriter.WriteElementString("def", worldObject.def.defName);
					xmlWriter.WriteElementString("faction", "Faction_" + Find.FactionManager.OfPlayer.loadID.ToString());
					xmlWriter.WriteElementString("ID", worldObject.ID.ToString());
					xmlWriter.WriteElementString("nameInt", worldObject.Label);
					xmlWriter.WriteEndElement();
				}
			}
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();
			xmlWriter.WriteStartElement("maps");
			foreach (Map map in Current.Game.Maps)
			{
				if (map.Parent.Faction != null && map.Parent.Faction.loadID == Find.FactionManager.OfPlayer.loadID)
				{
					xmlWriter.WriteStartElement("li");
					xmlWriter.WriteStartElement("mapInfo");
					xmlWriter.WriteElementString("parent", "WorldObject_" + map.Parent.ID.ToString());
					xmlWriter.WriteEndElement();
					xmlWriter.WriteStartElement("components");
					foreach (MapComponent mapComponent in map.components)
					{
						IRuleComp ruleComp = mapComponent as IRuleComp;
						if (ruleComp != null && ruleComp.Count() > 0)
						{
							xmlWriter.WriteStartElement("li");
							xmlWriter.WriteAttributeString("Class", ruleComp.GetType().FullName);
							ASImport.WriteList(xmlWriter, ruleComp.EnumerateRules().ToList<IRule>(), "Rules");
							xmlWriter.WriteEndElement();
						}
					}
					xmlWriter.WriteEndElement();
				}
			}
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndDocument();
			xmlWriter.Close();
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00002DA4 File Offset: 0x00000FA4
		private static void WriteList(XmlWriter writer, IList list, string listname)
		{
			writer.WriteStartElement(listname);
			foreach (object obj in list)
			{
				writer.WriteStartElement("li");
				Type type = obj.GetType();
				if (type.IsValueType || type == typeof(string))
				{
					writer.WriteString(obj.ToString());
				}
				else
				{
					ASImport.WriteComplexClass(writer, obj);
				}
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00002E40 File Offset: 0x00001040
		private static void WriteComplexClass(XmlWriter writer, object o)
		{
			FieldInfo[] fields = o.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (fields != null)
			{
				foreach (FieldInfo fieldInfo in fields)
				{
					object value = fieldInfo.GetValue(o);
					if (fieldInfo.HasAttribute<AutoSellExportableValue>())
					{
						AutoSellExportableValue autoSellExportableValue = fieldInfo.TryGetAttribute<AutoSellExportableValue>();
						ASImport.WriteSimple(writer, value, autoSellExportableValue.SaveName);
					}
					else if (fieldInfo.HasAttribute<AutoSellExportableList>())
					{
						AutoSellExportableList autoSellExportableList = fieldInfo.TryGetAttribute<AutoSellExportableList>();
						ASImport.WriteList(writer, value as IList, autoSellExportableList.SaveName);
					}
					else if (fieldInfo.HasAttribute<AutoSellExportableDeep>())
					{
						AutoSellExportableDeep autoSellExportableDeep = fieldInfo.TryGetAttribute<AutoSellExportableDeep>();
						writer.WriteStartElement(autoSellExportableDeep.SaveName);
						ASImport.WriteComplexClass(writer, value);
						writer.WriteEndElement();
					}
				}
			}
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002EF6 File Offset: 0x000010F6
		private static void WriteSimple(XmlWriter writer, object o, string name)
		{
			writer.WriteElementString(name, o.ToString());
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002F08 File Offset: 0x00001108
		public static List<SelectedItem<string>> ListSaves()
		{
			XmlReaderSettings settings = new XmlReaderSettings
			{
				IgnoreWhitespace = true,
				IgnoreComments = true,
				IgnoreProcessingInstructions = true
			};
			List<SelectedItem<string>> list = new List<SelectedItem<string>>();
			List<FileInfo> list2 = new List<FileInfo>();
			try
			{
				string path = Path.Combine(GenFilePaths.ConfigFolderPath, "..", "ASExports");
				if (Directory.Exists(path))
				{
					foreach (FileInfo fileInfo in new DirectoryInfo(path).GetFiles())
					{
						if (fileInfo.Extension == ".xml")
						{
							list2.Add(fileInfo);
						}
					}
				}
			}
			catch
			{
			}
			foreach (FileInfo item in GenFilePaths.AllSavedGameFiles)
			{
				list2.Add(item);
			}
			foreach (FileInfo fileInfo2 in list2)
			{
				try
				{
					XmlReader xmlReader = XmlReader.Create(fileInfo2.FullName, settings);
					bool flag = ASImport.ReadVersion(xmlReader, list, fileInfo2.FullName, fileInfo2.Name);
					xmlReader.Close();
					if (!flag)
					{
						using (Stream stream = new DeflateStream(new FileStream(fileInfo2.FullName, FileMode.Open, FileAccess.Read, FileShare.Read), CompressionMode.Decompress))
						{
							xmlReader = XmlReader.Create(stream);
							if (!ASImport.ReadVersion(xmlReader, list, fileInfo2.FullName, fileInfo2.Name))
							{
								Log.Message("AS Error: save read" + fileInfo2.FullName);
							}
							xmlReader.Close();
							stream.Close();
						}
					}
				}
				catch
				{
					Log.Message("AS Error: File Error");
				}
			}
			return list;
		}

		// Token: 0x06000029 RID: 41 RVA: 0x000030F0 File Offset: 0x000012F0
		public static void ProcessSave(string savepath, IRuleComp Comp)
		{
			XmlReaderSettings settings = new XmlReaderSettings
			{
				IgnoreWhitespace = true,
				IgnoreComments = true,
				IgnoreProcessingInstructions = true
			};
			XmlReader xmlReader = XmlReader.Create(savepath, settings);
			bool flag = true;
			bool flag2 = false;
			try
			{
				xmlReader.ReadToFollowing("savegame");
			}
			catch
			{
				xmlReader.Close();
				flag = false;
				xmlReader = XmlReader.Create(new DeflateStream(new FileStream(savepath, FileMode.Open, FileAccess.Read, FileShare.Read), CompressionMode.Decompress), settings);
			}
			try
			{
				ASImport.GetPlayerFaction(xmlReader);
				if (ASImport.CacheFactionID != string.Empty)
				{
					ASImport.CheckWorldObjects(xmlReader);
				}
			}
			catch
			{
				flag2 = true;
			}
			xmlReader.Close();
			if (flag2)
			{
				return;
			}
			XmlReader xmlReader2 = flag ? XmlReader.Create(savepath, settings) : XmlReader.Create(new DeflateStream(new FileStream(savepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), CompressionMode.Decompress), settings);
			xmlReader2.ReadToFollowing("meta");
			xmlReader2.ReadToNextSibling("game");
			xmlReader2.ReadToDescendant("currentMapIndex");
			ASImport.CacheMaps.Clear();
			ASImport.ProcessGlobal(xmlReader2, Comp);
			ASImport.ProcessMaps(xmlReader2, Comp);
			xmlReader2.Close();
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00003200 File Offset: 0x00001400
		private static void ProcessGlobal(XmlReader reader, IRuleComp Comp)
		{
			if (Comp.ShareableAllowed)
			{
				IRuleShareComp ruleShareComp = Comp as IRuleShareComp;
				if (ruleShareComp == null)
				{
					return;
				}
				string fullName = ruleShareComp.GetGameCompType().FullName;
				if (reader.ReadToNextSibling("components"))
				{
					XmlReader xmlReader = reader.ReadSubtree();
					if (xmlReader.ReadToDescendant("li"))
					{
						for (;;)
						{
							if (xmlReader.HasAttributes && !xmlReader.IsEmptyElement)
							{
								while (xmlReader.MoveToNextAttribute())
								{
									if (xmlReader.Name == "Class")
									{
										if (xmlReader.Value == fullName)
										{
											goto Block_7;
										}
										xmlReader.MoveToElement();
										break;
									}
								}
							}
							if (!xmlReader.ReadToNextSibling("li"))
							{
								goto IL_190;
							}
						}
						Block_7:
						xmlReader.MoveToElement();
						XmlReader xmlReader2 = xmlReader.ReadSubtree();
						XmlReader xmlReader3 = xmlReader.ReadSubtree();
						if (xmlReader2.ReadToDescendant("GRules") && xmlReader2.ReadInnerXml() == "True" && xmlReader3.ReadToDescendant("GlobalRules") && !xmlReader3.IsEmptyElement)
						{
							XmlReader xmlReader4 = xmlReader3.ReadSubtree();
							IList list = ASImport.SetListSaveOnType(typeof(List<>).MakeGenericType(new Type[]
							{
								Comp.GetCompType()
							}), xmlReader4);
							if (list != null && list.Count > 0)
							{
								ASImport.ValidateCompList((IRule)Activator.CreateInstance(Comp.GetCompType()), list);
							}
							ASImport.CacheMaps.Add(new Pair<string, IList>("RWAutoSell.ImShared".Translate(), list));
							xmlReader4.Close();
						}
						xmlReader2.Close();
						xmlReader3.Close();
					}
					IL_190:
					xmlReader.Close();
					return;
				}
			}
		}

		// Token: 0x0600002B RID: 43 RVA: 0x000033A8 File Offset: 0x000015A8
		private static void PrintLine(XmlReader reader, string s)
		{
			IXmlLineInfo xmlLineInfo = (IXmlLineInfo)reader;
			Log.Message(s + ": " + xmlLineInfo.LineNumber.ToString());
		}

		// Token: 0x0600002C RID: 44 RVA: 0x000033DC File Offset: 0x000015DC
		private static void ValidateCompList(IRule Blank, IList list)
		{
			int num = Blank.Nodes.Count<RuleRoot>();
			foreach (object obj in list)
			{
				IRule rule = (IRule)obj;
				if (rule.Nodes.Count < num)
				{
					for (int i = rule.Nodes.Count; i < num; i++)
					{
						rule.Nodes.Add(Blank.Nodes[i].DeepCopy());
					}
				}
				ASImport.ValidateCompList(Blank, rule.RuleChain().ToList<IRule>());
			}
		}

		// Token: 0x0600002D RID: 45 RVA: 0x0000348C File Offset: 0x0000168C
		private static void ProcessMaps(XmlReader reader, IRuleComp Comp)
		{
			string fullName = Comp.GetType().FullName;
			if (reader.ReadToNextSibling("maps"))
			{
				XmlReader xmlReader = reader.ReadSubtree();
				if (xmlReader.ReadToDescendant("li"))
				{
					do
					{
						XmlReader xmlReader2 = xmlReader.ReadSubtree();
						string empty = string.Empty;
						bool flag = false;
						if (xmlReader2.ReadToDescendant("mapInfo"))
						{
							XmlReader xmlReader3 = xmlReader2.ReadSubtree();
							if (xmlReader3.ReadToDescendant("parent"))
							{
								ASImport.PrintLine(xmlReader3, "parent");
								string key = xmlReader3.ReadInnerXml();
								if (ASImport.CacheWorldObjects.TryGetValue(key, out empty))
								{
									flag = true;
								}
							}
							xmlReader3.Close();
						}
						if (flag && xmlReader2.ReadToNextSibling("components"))
						{
							XmlReader xmlReader4 = xmlReader2.ReadSubtree();
							if (xmlReader4.ReadToDescendant("li"))
							{
								for (;;)
								{
									if (xmlReader4.HasAttributes && !xmlReader4.IsEmptyElement)
									{
										while (xmlReader4.MoveToNextAttribute())
										{
											if (xmlReader4.Name == "Class")
											{
												if (xmlReader4.Value == fullName)
												{
													goto Block_10;
												}
												xmlReader4.MoveToElement();
												break;
											}
										}
									}
									if (!xmlReader4.ReadToNextSibling("li"))
									{
										goto IL_1D7;
									}
								}
								Block_10:
								xmlReader4.MoveToElement();
								XmlReader xmlReader5 = xmlReader4.ReadSubtree();
								if (xmlReader5.ReadToDescendant("Rules") && !xmlReader5.IsEmptyElement)
								{
									XmlReader xmlReader6 = xmlReader5.ReadSubtree();
									IList list = ASImport.SetListSaveOnType(typeof(List<>).MakeGenericType(new Type[]
									{
										Comp.GetCompType()
									}), xmlReader6);
									if (list != null && list.Count > 0)
									{
										ASImport.ValidateCompList((IRule)Activator.CreateInstance(Comp.GetCompType()), list);
									}
									ASImport.CacheMaps.Add(new Pair<string, IList>(empty, list));
									xmlReader6.Close();
								}
								xmlReader5.Close();
								xmlReader4.Close();
								goto IL_1DE;
							}
							IL_1D7:
							xmlReader4.Close();
						}
						IL_1DE:
						xmlReader2.Close();
					}
					while (xmlReader.ReadToNextSibling("li"));
				}
				xmlReader.Close();
			}
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00003694 File Offset: 0x00001894
		private static Dictionary<string, FieldInfo> GetFieldSaveOnType(Type T)
		{
			Dictionary<string, FieldInfo> dictionary = new Dictionary<string, FieldInfo>();
			foreach (FieldInfo fieldInfo in T.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			{
				if (fieldInfo.HasAttribute<AutoSellExportableValue>() || fieldInfo.HasAttribute<AutoSellExportableList>() || fieldInfo.HasAttribute<AutoSellExportableDeep>())
				{
					AutoSellExportable autoSellExportable = fieldInfo.TryGetAttribute<AutoSellExportable>();
					if (autoSellExportable != null)
					{
						dictionary.Add(autoSellExportable.SaveName, fieldInfo);
					}
				}
			}
			return dictionary;
		}

		// Token: 0x0600002F RID: 47 RVA: 0x000036F8 File Offset: 0x000018F8
		private static IList SetListSaveOnType(Type ListT, XmlReader reader)
		{
			IList list = (IList)Activator.CreateInstance(ListT);
			if (reader.ReadToDescendant("li"))
			{
				do
				{
					XmlReader xmlReader = reader.ReadSubtree();
					object obj = ASImport.SetFieldSaveOnType(ListT.GetGenericArguments()[0], xmlReader);
					if (obj != null)
					{
						list.Add(obj);
					}
					xmlReader.Close();
				}
				while (reader.ReadToNextSibling("li"));
			}
			return list;
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00003754 File Offset: 0x00001954
		private static object PrimitiveConvert(Type T, string obj, ref string err)
		{
			TypeCode typeCode = Type.GetTypeCode(T);
			if (typeCode == TypeCode.Boolean)
			{
				return XmlConvert.ToBoolean(obj.ToLower());
			}
			if (typeCode == TypeCode.DateTime)
			{
				return XmlConvert.ToDateTime(obj, XmlDateTimeSerializationMode.Local);
			}
			if (typeCode == TypeCode.String)
			{
				return obj;
			}
			err += "/n is other";
			return TypeDescriptor.GetConverter(T).ConvertFromString(obj);
		}

		// Token: 0x06000031 RID: 49 RVA: 0x000037B0 File Offset: 0x000019B0
		private static object SetFieldSaveOnType(Type obj, XmlReader reader)
		{
			string text = string.Empty;
			int num = 0;
			if (obj.IsValueType || obj == typeof(string))
			{
				while (reader.NodeType != XmlNodeType.Text)
				{
					if (!reader.Read())
					{
						return null;
					}
				}
				return ASImport.PrimitiveConvert(obj, reader.Value, ref text);
			}
			Dictionary<string, FieldInfo> fieldSaveOnType = ASImport.GetFieldSaveOnType(obj);
			object result;
			try
			{
				text += "non value type";
				text = text + "/n" + reader.Name;
				text = text + "/n " + obj.FullName;
				object obj2 = Activator.CreateInstance(obj);
				do
				{
					IL_82:
					text = "/n Loop " + num.ToString();
					if (reader.NodeType == XmlNodeType.Element)
					{
						text = text + "/n entered Element " + reader.Name;
						FieldInfo fieldInfo;
						if (fieldSaveOnType.TryGetValue(reader.Name, out fieldInfo))
						{
							if (fieldInfo.HasAttribute<AutoSellExportableValue>())
							{
								string name = reader.Name;
								text += "/n has value attrib";
								XmlReader xmlReader = reader.ReadSubtree();
								while (xmlReader.NodeType != XmlNodeType.Text)
								{
									if (!xmlReader.Read())
									{
										IL_11B:
										xmlReader.Close();
										text += "success";
										num++;
										fieldSaveOnType.Remove(name);
										goto IL_82;
									}
								}
								fieldInfo.SetValue(obj2, ASImport.PrimitiveConvert(fieldInfo.FieldType, reader.Value, ref text));
								goto IL_11B;
							}
							if (fieldInfo.HasAttribute<AutoSellExportableList>())
							{
								text += "/n has list attrib";
								XmlReader reader2 = reader.ReadSubtree();
								IList value = ASImport.SetListSaveOnType(fieldInfo.FieldType, reader2);
								fieldInfo.SetValue(obj2, value);
							}
							else if (fieldInfo.HasAttribute<AutoSellExportableDeep>())
							{
								text += "/n has deep attrib";
								XmlReader xmlReader2 = reader.ReadSubtree();
								xmlReader2.Read();
								fieldInfo.SetValue(obj2, ASImport.SetFieldSaveOnType(fieldInfo.FieldType, xmlReader2));
							}
						}
					}
				}
				while (reader.Read());
				if (fieldSaveOnType.Count > 0)
				{
					foreach (FieldInfo fieldInfo2 in fieldSaveOnType.Values)
					{
						if (fieldInfo2.HasAttribute<AutoSellExportableValue>())
						{
							AutoSellExportableValue autoSellExportableValue = fieldInfo2.TryGetAttribute<AutoSellExportableValue>();
							if (autoSellExportableValue.DefaultValue != null)
							{
								fieldInfo2.SetValue(obj2, autoSellExportableValue.DefaultValue);
							}
						}
					}
				}
				result = obj2;
			}
			catch
			{
				Log.Message(text);
				result = null;
			}
			return result;
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00003A28 File Offset: 0x00001C28
		private static void CheckWorldObjects(XmlReader reader)
		{
			ASImport.CacheWorldObjects.Clear();
			if (reader.ReadToFollowing("worldObjects") && reader.ReadToFollowing("worldObjects"))
			{
				XmlReader xmlReader = reader.ReadSubtree();
				if (xmlReader.ReadToDescendant("li"))
				{
					do
					{
						XmlReader xmlReader2 = reader.ReadSubtree();
						xmlReader2.ReadToDescendant("def");
						string a = string.Empty;
						string text = string.Empty;
						string text2 = string.Empty;
						for (;;)
						{
							if (xmlReader2.NodeType == XmlNodeType.Element)
							{
								string name = xmlReader2.Name;
								if (!(name == "faction"))
								{
									if (!(name == "ID"))
									{
										if (!(name == "nameInt"))
										{
											xmlReader2.Skip();
										}
										else
										{
											text2 = xmlReader2.ReadInnerXml();
										}
									}
									else
									{
										text = xmlReader2.ReadInnerXml();
									}
								}
								else
								{
									a = xmlReader2.ReadInnerXml();
								}
							}
							else if (!xmlReader2.Read())
							{
								break;
							}
						}
						xmlReader2.Close();
						if (a == "Faction_" + ASImport.CacheFactionID && text != string.Empty)
						{
							if (text2 == string.Empty)
							{
								text2 = "UnknownLower".Translate().CapitalizeFirst();
							}
							ASImport.CacheWorldObjects.Add("WorldObject_" + text, text2);
						}
					}
					while (xmlReader.ReadToFollowing("li"));
				}
				xmlReader.Close();
			}
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00003B84 File Offset: 0x00001D84
		private static void GetPlayerFaction(XmlReader reader)
		{
			string text = string.Empty;
			if (reader.ReadToFollowing("game") && reader.ReadToDescendant("scenario"))
			{
				XmlReader xmlReader = reader.ReadSubtree();
				if (xmlReader.ReadToDescendant("playerFaction") && xmlReader.ReadToDescendant("factionDef"))
				{
					text = reader.ReadInnerXml();
				}
				xmlReader.Close();
			}
			if (text == string.Empty)
			{
				return;
			}
			ASImport.CacheFactionID = string.Empty;
			ASImport.CacheFactionName = string.Empty;
			if (reader.ReadToFollowing("tickManager") && reader.ReadToNextSibling("world") && reader.ReadToDescendant("info") && reader.ReadToNextSibling("factionManager") && reader.ReadToDescendant("allFactions"))
			{
				XmlReader xmlReader2 = reader.ReadSubtree();
				if (xmlReader2.ReadToDescendant("li"))
				{
					string text2;
					string text3;
					for (;;)
					{
						text2 = string.Empty;
						text3 = string.Empty;
						bool flag = false;
						XmlReader xmlReader3 = xmlReader2.ReadSubtree();
						xmlReader2.Read();
						for (;;)
						{
							if (xmlReader3.NodeType == XmlNodeType.Element)
							{
								string name = xmlReader3.Name;
								if (!(name == "def"))
								{
									if (!(name == "loadID"))
									{
										if (!(name == "name"))
										{
											xmlReader3.Skip();
										}
										else
										{
											text3 = xmlReader3.ReadInnerXml();
											if (flag && text3 != string.Empty && text2 != string.Empty)
											{
												break;
											}
										}
									}
									else
									{
										text2 = xmlReader3.ReadInnerXml();
										if (flag && text3 != string.Empty && text2 != string.Empty)
										{
											break;
										}
									}
								}
								else
								{
									flag = (xmlReader3.ReadInnerXml() == text);
									if (!flag)
									{
										break;
									}
									if (text3 != string.Empty && text2 != string.Empty)
									{
										break;
									}
								}
							}
							else if (!xmlReader3.Read())
							{
								break;
							}
						}
						IL_1EB:
						xmlReader3.Close();
						if (flag && text2 != string.Empty && flag)
						{
							break;
						}
						if (!xmlReader2.ReadToNextSibling("li"))
						{
							goto IL_253;
						}
						continue;
						goto IL_1EB;
					}
					ASImport.CacheFactionID = text2;
					if (text3 == string.Empty)
					{
						ASImport.CacheFactionName = "UnknownLower".Translate().CapitalizeFirst();
					}
					else
					{
						ASImport.CacheFactionName = text3;
					}
				}
				IL_253:
				xmlReader2.Close();
			}
		}

		// Token: 0x04000007 RID: 7
		public static string CacheFactionName;

		// Token: 0x04000008 RID: 8
		private static string CacheFactionID;

		// Token: 0x04000009 RID: 9
		private static Dictionary<string, string> CacheWorldObjects = new Dictionary<string, string>();

		// Token: 0x0400000A RID: 10
		[Nullable(new byte[]
		{
			1,
			0,
			1,
			1
		})]
		public static List<Pair<string, IList>> CacheMaps = new List<Pair<string, IList>>();
	}
}
