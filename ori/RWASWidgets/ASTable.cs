using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace RWASWidgets
{
	// Token: 0x02000017 RID: 23
	[NullableContext(1)]
	[Nullable(0)]
	public class ASTable
	{
		// Token: 0x17000037 RID: 55
		public Rect this[int column, int row]
		{
			get
			{
				return this.GetRectangle(column, row, -1, -1, false);
			}
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x00006ABC File Offset: 0x00004CBC
		public void DebugDrawCells(bool ignoremargins = false, bool tablearea = true)
		{
			for (int i = 0; i < this.MaxColumns; i++)
			{
				for (int j = 0; j < this.MaxRows; j++)
				{
					Widgets.DrawHighlight(this.GetRectangle(i, j, -1, -1, ignoremargins));
				}
			}
			Widgets.DrawBox(this.GetRectangle(0, 0, this.MaxColumns - 1, this.MaxRows - 1, true), 1, null);
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x00006B1C File Offset: 0x00004D1C
		public ASTable(Rect InRect, int columns, int rows, float SideMargin = 0f, float BottomMargin = 0f, bool NormaliseDecimals = false)
		{
			this.Tablearea = InRect;
			this.cellsize = new Vector2(InRect.width / (float)columns, InRect.height / (float)rows);
			this.Initalize(columns, rows, SideMargin, BottomMargin, NormaliseDecimals);
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x00006B70 File Offset: 0x00004D70
		public ASTable(Vector2 StartPoint, Vector2 CellSize, int columns, int rows, float SideMargin = 0f, float BottomMargin = 0f, bool NormaliseDecimals = false)
		{
			this.Tablearea = new Rect(StartPoint.x, StartPoint.y, CellSize.x * (float)columns, CellSize.y * (float)rows);
			this.cellsize = CellSize;
			this.Initalize(columns, rows, SideMargin, BottomMargin, NormaliseDecimals);
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x00006BD0 File Offset: 0x00004DD0
		private void Initalize(int columns, int rows, float SideMargin = 0f, float BottomMargin = 0f, bool NormaliseDecimals = false)
		{
			this.NormaliseDecimals_ = NormaliseDecimals;
			this.MaxColumns = columns;
			this.MaxRows = rows;
			if (SideMargin > 0f && SideMargin < this.cellsize.x && SideMargin < this.cellsize.y)
			{
				this.padding = new Vector2(SideMargin, BottomMargin);
			}
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x00006C28 File Offset: 0x00004E28
		public void DrawCellBorders(int start, int end, bool vertical, params int[] columnrow)
		{
			if (columnrow != null)
			{
				for (int i = 0; i < columnrow.Count<int>(); i++)
				{
					if (vertical)
					{
						Widgets.DrawLineVertical(this.Tablearea.x + (float)columnrow[i] * this.cellsize.x, this.Tablearea.y + (float)start * this.cellsize.y, this.cellsize.y * (float)(end - start + 1));
					}
					else
					{
						Widgets.DrawLineHorizontal(this.Tablearea.x + (float)start * this.cellsize.x, this.Tablearea.y + (float)columnrow[i] * this.cellsize.y, this.cellsize.x * (float)(end - start + 1));
					}
				}
			}
		}

		// Token: 0x060000EA RID: 234 RVA: 0x00006CF5 File Offset: 0x00004EF5
		public void DrawCellBorders(bool vertical, params int[] columnrow)
		{
			if (vertical)
			{
				this.DrawCellBorders(0, this.MaxRows - 1, vertical, columnrow);
				return;
			}
			this.DrawCellBorders(0, this.MaxColumns - 1, vertical, columnrow);
		}

		// Token: 0x060000EB RID: 235 RVA: 0x00006D20 File Offset: 0x00004F20
		public Rect GetRectangle(int column, int row, int mergecolumn = -1, int mergerow = -1, bool ignoremargins = false)
		{
			if (column > this.MaxColumns)
			{
				column = this.MaxColumns;
			}
			if (row > this.MaxRows)
			{
				row = this.MaxRows;
			}
			if (mergecolumn > this.MaxColumns)
			{
				mergecolumn = this.MaxColumns;
			}
			if (mergerow > this.MaxRows)
			{
				mergerow = this.MaxRows;
			}
			if (column < 0)
			{
				column = 0;
			}
			if (row < 0)
			{
				row = 0;
			}
			Rect result = default(Rect);
			if (mergecolumn > -1)
			{
				if (mergecolumn < column)
				{
					int num = column;
					column = mergecolumn;
					mergecolumn = num;
				}
			}
			else
			{
				mergecolumn = column;
			}
			if (mergerow > -1)
			{
				if (mergerow < row)
				{
					int num2 = row;
					row = mergerow;
					mergerow = num2;
				}
			}
			else
			{
				mergerow = row;
			}
			result.width = (float)(mergecolumn - column + 1) * this.cellsize.x;
			result.height = (float)(mergerow - row + 1) * this.cellsize.y;
			result.x = this.Tablearea.x + (float)column * this.cellsize.x;
			result.y = this.Tablearea.y + (float)row * this.cellsize.y;
			if (!ignoremargins)
			{
				result.width -= this.padding.x;
				result.height -= this.padding.y;
				result.x += this.padding.x / 2f;
				result.y += this.padding.y / 2f;
			}
			result.width = Mathf.Floor(result.width);
			result.height = Mathf.Floor(result.height);
			return result;
		}

		// Token: 0x060000EC RID: 236 RVA: 0x00006EC1 File Offset: 0x000050C1
		public void SplitRectangle(int column, int row, float Split, out Rect half1, out Rect half2, bool vertical = true)
		{
			this.SplitRectangle(this.GetRectangle(column, row, -1, -1, false), Split, out half1, out half2, vertical);
		}

		// Token: 0x060000ED RID: 237 RVA: 0x00006EDC File Offset: 0x000050DC
		public void SplitRectangle(Rect rect, float Split, out Rect half1, out Rect half2, bool vertical = true)
		{
			if (vertical)
			{
				if (Split > rect.width)
				{
					Split = rect.width;
				}
				half1 = new Rect(rect.x, rect.y, Mathf.Floor(Split - this.padding.x / 2f), rect.height);
				half2 = new Rect(Mathf.Floor(rect.x + Split + this.padding.x / 2f), rect.y, Mathf.Floor(rect.width - Split - this.padding.x / 2f), rect.height);
				return;
			}
			if (Split > rect.height)
			{
				Split = rect.height;
			}
			half1 = new Rect(rect.x, rect.y, rect.width, Mathf.Floor(Split - this.padding.y / 2f));
			half2 = new Rect(rect.x, Mathf.Floor(rect.y + Split + this.padding.y / 2f), rect.width, Mathf.Floor(rect.height - Split - this.padding.y / 2f));
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x060000EE RID: 238 RVA: 0x0000703B File Offset: 0x0000523B
		public float YMax
		{
			get
			{
				return this.Tablearea.yMax;
			}
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x060000EF RID: 239 RVA: 0x00007048 File Offset: 0x00005248
		public float XMax
		{
			get
			{
				return this.Tablearea.xMax;
			}
		}

		// Token: 0x04000068 RID: 104
		private Rect Tablearea;

		// Token: 0x04000069 RID: 105
		private Vector2 cellsize;

		// Token: 0x0400006A RID: 106
		private int MaxColumns;

		// Token: 0x0400006B RID: 107
		private int MaxRows;

		// Token: 0x0400006C RID: 108
		private bool NormaliseDecimals_;

		// Token: 0x0400006D RID: 109
		private Vector2 padding = Vector2.zero;
	}
}
