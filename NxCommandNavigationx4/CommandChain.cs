namespace CommandNavigation.CommandnNavigation4 {
  using System;
  using System.Collections.Generic;
  using System.Text;

  public class CommandChain<T> : LinkedList<T> where T : class, ICommandCtrlx4 {
    public int Order { get => First.Value.Order; }
    public CommandNavigation<T> Navigation { get; protected set; }
    protected CommandChain() : base() { }
    public CommandChain(CommandNavigation<T> Navigation, T FirstItem) : base() {
      this.Navigation = Navigation;
      this.Add(FirstItem);
    }
    [Obsolete("", true)] public new LinkedListNode<T> AddAfter(LinkedListNode<T> Node, T Value) => throw new NotImplementedException("Obsolete");
    [Obsolete("", true)] public new LinkedListNode<T> AddBefore(LinkedListNode<T> Node, T Value) => throw new NotImplementedException("Obsolete");
    [Obsolete("", true)] public new void AddAfter(LinkedListNode<T> Node, LinkedListNode<T> NewNode) => throw new NotImplementedException("Obsolete");
    [Obsolete("", true)] public new void AddBefore(LinkedListNode<T> Node, LinkedListNode<T> NewNode) => throw new NotImplementedException("Obsolete");
    [Obsolete("", true)] public new void AddFirst(LinkedListNode<T> Node) => throw new NotImplementedException("Obsolete");
    [Obsolete("", true)] public new void AddLast(LinkedListNode<T> Node) => throw new NotImplementedException("Obsolete");
    [Obsolete("", true)] public new LinkedListNode<T> AddFirst(T Value) => throw new NotImplementedException("Obsolete");
    [Obsolete("", true)] public new LinkedListNode<T> AddLast(T Value) => throw new NotImplementedException("Obsolete");
    [Obsolete("", true)] public new void Remove(LinkedListNode<T> Node) => throw new NotImplementedException("Obsolete");
    [Obsolete("", true)] public new bool Remove(T Value) => throw new NotImplementedException("Obsolete");
    [Obsolete("", true)] public new void RemoveFirst() => throw new NotImplementedException("Obsolete");
    [Obsolete("", true)] public new void RemoveLast() => throw new NotImplementedException("Obsolete");
    public void Add(T Value) {
      base.AddLast(Value);
      Value.CommandState = CommandState.Pushed;
      Value.OnPush();
      Navigation.InvokeOnCommandPushedHandle(this, Value);
    }
    public bool Remove(Predicate<T> Match, out T Value) {
      LinkedListNode<T> C = First;
      while (C != null) {
        if (Match(C.Value)) {
          base.Remove(C);
          C.Value.CommandState = CommandState.Popped;
          C.Value.OnPop();
          Navigation.InvokeOnCommandPoppedHandle(this, C.Value);
          Value = C.Value;
          return true;
        }
        C = C.Next;
      }
      Value = null;
      return false;
    }
    public void Clear() {
      foreach (var item in this) {
        item.CommandState = CommandState.Popped;
        item.OnPop();
        Navigation.InvokeOnCommandPoppedHandle(this, item);
      }
      base.Clear();
    }
    public void Discard() {
      base.Clear();
    }

  }



}
