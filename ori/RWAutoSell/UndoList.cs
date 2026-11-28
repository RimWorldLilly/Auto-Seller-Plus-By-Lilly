using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RWASFilterLib;

namespace RWAutoSell
{
	// Token: 0x02000027 RID: 39
	[NullableContext(1)]
	[Nullable(0)]
	public class UndoList : IEnumerable<IRule>, IEnumerable
	{
		// Token: 0x0600013C RID: 316 RVA: 0x0000E176 File Offset: 0x0000C376
		public IEnumerator<IRule> GetEnumerator()
		{
			UndoList.<GetEnumerator>d__4 <GetEnumerator>d__ = new UndoList.<GetEnumerator>d__4(0);
			<GetEnumerator>d__.<>4__this = this;
			return <GetEnumerator>d__;
		}

		// Token: 0x0600013D RID: 317 RVA: 0x0000E185 File Offset: 0x0000C385
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x0600013E RID: 318 RVA: 0x0000E18D File Offset: 0x0000C38D
		public void Push(IRule Item, FilterGroup SelectedGroup, FilterContainer Filter, int node, int ptab)
		{
			this.Changes.Push(new UndoList.UndoItem(Item, SelectedGroup, Filter, node, ptab));
			this.UnDone.Clear();
		}

		// Token: 0x0600013F RID: 319 RVA: 0x0000E1B4 File Offset: 0x0000C3B4
		public IRule Undo(IRule Current, ref FilterGroup SelectedGroup, ref FilterContainer Filter, ref int node, ref int ptab)
		{
			if (this.CanUndo())
			{
				UndoList.UndoItem undoItem = this.Changes.Pop();
				IRule rule = undoItem.rule;
				this.UnDone.Push(new UndoList.UndoItem(Current, SelectedGroup, Filter, node, ptab));
				node = undoItem.Node;
				ptab = undoItem.Property;
				SelectedGroup = ((undoItem.Group == -1L) ? null : rule.Nodes[undoItem.Node].RootNode.GetGroupWithID(undoItem.Group));
				Filter = ((undoItem.Filter == -1L) ? null : rule.Nodes[undoItem.Node].RootNode.GetFilterWithID(undoItem.Filter));
				return rule;
			}
			return Current;
		}

		// Token: 0x06000140 RID: 320 RVA: 0x0000E26F File Offset: 0x0000C46F
		public bool CanUndo()
		{
			return this.Changes.Count > 0;
		}

		// Token: 0x06000141 RID: 321 RVA: 0x0000E27F File Offset: 0x0000C47F
		public int Count()
		{
			return this.Changes.Count;
		}

		// Token: 0x06000142 RID: 322 RVA: 0x0000E28C File Offset: 0x0000C48C
		public bool CanRedo()
		{
			return this.UnDone.Count > 0;
		}

		// Token: 0x06000143 RID: 323 RVA: 0x0000E29C File Offset: 0x0000C49C
		public IRule Redo(IRule Current, ref FilterGroup SelectedGroup, ref FilterContainer Filter, ref int node, ref int ptab)
		{
			if (this.CanRedo())
			{
				this.Changes.Push(new UndoList.UndoItem(Current, SelectedGroup, Filter, node, ptab));
				UndoList.UndoItem undoItem = this.UnDone.Pop();
				SelectedGroup = undoItem.rule.Nodes[undoItem.Node].RootNode.GetGroupWithID(undoItem.Group);
				Filter = undoItem.rule.Nodes[undoItem.Node].RootNode.GetFilterWithID(undoItem.Filter);
				node = undoItem.Node;
				ptab = undoItem.Property;
				return undoItem.rule;
			}
			return Current;
		}

		// Token: 0x040000E2 RID: 226
		private Stack<UndoList.UndoItem> Changes = new Stack<UndoList.UndoItem>();

		// Token: 0x040000E3 RID: 227
		private Stack<UndoList.UndoItem> UnDone = new Stack<UndoList.UndoItem>();

		// Token: 0x02000076 RID: 118
		[Nullable(0)]
		private class UndoItem
		{
			// Token: 0x0600039E RID: 926 RVA: 0x0001ECC0 File Offset: 0x0001CEC0
			public UndoItem(IRule Rule, FilterGroup SelectedGroup, FilterContainer SelectedFilter, int SelectedNode, int ptab)
			{
				this.rule = Rule.DeepCopy();
				this.Group = ((SelectedGroup == null) ? -1L : SelectedGroup.ID);
				this.Filter = ((SelectedFilter == null) ? -1L : SelectedFilter.ID);
				this.Node = SelectedNode;
				this.Property = ptab;
			}

			// Token: 0x040001E3 RID: 483
			public IRule rule;

			// Token: 0x040001E4 RID: 484
			public long Group;

			// Token: 0x040001E5 RID: 485
			public long Filter;

			// Token: 0x040001E6 RID: 486
			public int Node;

			// Token: 0x040001E7 RID: 487
			public int Property;
		}

		// Token: 0x02000077 RID: 119
		// (Invoke) Token: 0x060003A0 RID: 928
		[NullableContext(0)]
		public delegate void UndoRedo(IRule Current, ref FilterGroup SelectedGroup, ref FilterContainer Filter, ref int node, ref int ptab);
	}
}
