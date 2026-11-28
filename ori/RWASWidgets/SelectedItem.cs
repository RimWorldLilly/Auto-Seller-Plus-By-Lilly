using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace RWASWidgets
{
	// Token: 0x02000007 RID: 7
	[NullableContext(1)]
	[Nullable(0)]
	public class SelectedItem<[Nullable(2)] T>
	{
		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600002D RID: 45 RVA: 0x00002BA0 File Offset: 0x00000DA0
		// (set) Token: 0x0600002E RID: 46 RVA: 0x00002BA8 File Offset: 0x00000DA8
		public virtual bool Selected
		{
			get
			{
				return this.selected_;
			}
			set
			{
				this.selected_ = value;
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600002F RID: 47 RVA: 0x00002BB1 File Offset: 0x00000DB1
		// (set) Token: 0x06000030 RID: 48 RVA: 0x00002BBC File Offset: 0x00000DBC
		public T Item
		{
			get
			{
				return this.item_;
			}
			set
			{
				this.item_ = value;
				if (value != null && !this.surpresslabelupdate)
				{
					Entity entity = value as Entity;
					Def def = value as Def;
					if (entity != null)
					{
						this.label = entity.LabelCap;
					}
					else if (def != null)
					{
						string str = def.LabelCap;
						if (str.NullOrEmpty())
						{
							this.label.ToString();
						}
						else
						{
							this.label = str;
						}
					}
					else
					{
						this.label = value.ToString();
					}
					try
					{
						if (def != null)
						{
							BuildableDef buildableDef = value as BuildableDef;
							PawnKindDef pawnKindDef = value as PawnKindDef;
							if (buildableDef != null)
							{
								this.color = buildableDef.uiIconColor;
								this.Icon = buildableDef.uiIcon;
							}
							else if (pawnKindDef != null)
							{
								PawnKindLifeStage pawnKindLifeStage = pawnKindDef.lifeStages.Last<PawnKindLifeStage>();
								if (pawnKindLifeStage != null)
								{
									GraphicData bodyGraphicData = pawnKindLifeStage.bodyGraphicData;
									Material material = bodyGraphicData.Graphic.MatAt(Rot4.East, null);
									this.Icon = (Texture2D)material.mainTexture;
									this.color = bodyGraphicData.color;
								}
								else
								{
									this.color = Color.white;
									this.Icon = BaseContent.BadTex;
								}
							}
							else
							{
								this.color = Color.white;
								this.Icon = BaseContent.BadTex;
							}
						}
						else
						{
							this.color = Color.white;
							this.Icon = BaseContent.BadTex;
						}
						return;
					}
					catch
					{
						this.Icon = BaseContent.BadTex;
						return;
					}
				}
				this.label = string.Empty;
				this.Icon = BaseContent.BadTex;
				this.color = Color.white;
			}
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00002D6C File Offset: 0x00000F6C
		public static implicit operator T(SelectedItem<T> x)
		{
			return x.Item;
		}

		// Token: 0x0400000C RID: 12
		public Vector2 pos = Vector2.zero;

		// Token: 0x0400000D RID: 13
		private bool selected_;

		// Token: 0x0400000E RID: 14
		public bool surpresslabelupdate;

		// Token: 0x0400000F RID: 15
		public Color color = Color.white;

		// Token: 0x04000010 RID: 16
		private T item_;

		// Token: 0x04000011 RID: 17
		public string label = string.Empty;

		// Token: 0x04000012 RID: 18
		public string tooltip = string.Empty;

		// Token: 0x04000013 RID: 19
		public Texture2D Icon = BaseContent.BadTex;
	}
}
