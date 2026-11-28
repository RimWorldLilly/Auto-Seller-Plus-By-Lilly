using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace RWASWidgets
{
	// Token: 0x02000006 RID: 6
	[NullableContext(1)]
	public interface IGUIList<[Nullable(2)] T> : IList<SelectedItem<T>>, ICollection<SelectedItem<T>>, IEnumerable<SelectedItem<T>>, IEnumerable
	{
		// Token: 0x06000021 RID: 33
		void DrawListBox(Rect outrect);

		// Token: 0x06000022 RID: 34
		void ChangeSelected(int i);

		// Token: 0x06000023 RID: 35
		int GetItemIndex(T Item);

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000024 RID: 36
		T GetSelected { get; }

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000025 RID: 37
		List<T> GetSelectedItems { get; }

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000026 RID: 38
		int GetSelectedIndex { get; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000027 RID: 39
		bool CheckBoxes { get; }

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000028 RID: 40
		bool Icons { get; }

		// Token: 0x06000029 RID: 41
		void Add(T Item);

		// Token: 0x0600002A RID: 42
		void AddRange(IEnumerable<SelectedItem<T>> Items);

		// Token: 0x0600002B RID: 43
		void AddRange(IEnumerable<T> Items);

		// Token: 0x0600002C RID: 44
		void AddRange(params T[] Items);
	}
}
