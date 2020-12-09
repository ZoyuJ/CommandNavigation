namespace CommandNavigation.CommandnNavigation4 {
  using System;
  using System.Collections.Generic;
  using System.Linq;

  public class CommandChain<T> : LinkedList<T> where T : class, ICommandCtrlx4 {
    public int Order { get; protected set; } = 0;
    public CommandState CommandState { get; protected set; } = CommandState.Popped;
    public CommandNavigation<T> Navigation { get; protected set; }
    protected CommandChain() : base() { }

    public CommandChain(CommandNavigation<T> Navigation, IEnumerable<T> Items) : base(Items) {
      this.Navigation = Navigation;
      Order = Items.First().Order;
    }

    public CommandChain(CommandNavigation<T> Navigation) : base() {
      this.Navigation = Navigation;
    }
    public CommandChain(CommandNavigation<T> Navigation, in int Order) : this(Navigation) {
      this.Order = Order;
    }

    #region Obsoleted
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
    #endregion

    public void Add(T Value) {
      Value.OnPush();
      switch (CommandState) {
        case CommandState.Popped:
          Value.CommandState = CommandState.Popped;
          Value.OnPop();
          base.AddLast(Value);
          //Navigation.InvokeOnCommandPushedHandle(this, Value);
          break;
        case CommandState.Overed:
          Value.CommandState = CommandState.Overed;
          Value.OnOver();
          base.AddLast(Value);
          Navigation.InvokeOnCommandOveredHandle(this, Value);
          break;
        case CommandState.Topped:
          Value.CommandState = CommandState.Topped;
          Value.OnPush();
          base.AddLast(Value);
          Navigation.InvokeOnCommandPushedHandle(this, Value);
          break;
      }

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
    public new void Clear() {
      foreach (var item in this) {
        item.CommandState = CommandState.Popped;
        item.OnPop();
        Navigation.InvokeOnCommandPoppedHandle(this, item);
      }
      base.Clear();
    }

    public CommandChain<T> OnPop() {
      if (CommandState != CommandState.Popped) {
        this.Navigation = null;
        CommandState = CommandState.Popped;
        foreach (var item in this) {
          item.CommandState = CommandState.Popped;
          item.OnPop();
          Navigation.InvokeOnCommandPoppedHandle(this, item);
        }
      }
      return this;
    }
    public CommandChain<T> OnTop() {
      if (CommandState != CommandState.Topped) {
        CommandState = CommandState.Topped;
        if (Count > 0 && CommandState == CommandState.Overed) {
          foreach (var item in this) {
            item.CommandState = CommandState.Topped;
            item.OnTop();
            Navigation.InvokeOnCommandToppedHandle(this, item);
          }
        }
      }
      return this;
    }
    public CommandChain<T> OnPush(CommandNavigation<T> Navigation = null) {
      if (CommandState != CommandState.Topped) {
        if (Navigation != null) this.Navigation = Navigation;
        CommandState = CommandState.Topped;
        if (Count > 0 && CommandState == CommandState.Popped) {
          foreach (var item in this) {
            item.CommandState = CommandState.Topped;
            item.OnTop();
            Navigation.InvokeOnCommandToppedHandle(this, item);
          }
        }
      }
      return this;
    }
    public CommandChain<T> OnOver() {
      if (CommandState != CommandState.Overed) {
        CommandState = CommandState.Overed;
        if (Count > 0) {
          foreach (var item in this) {
            item.CommandState = CommandState.Overed;
            item.OnOver();
            Navigation.InvokeOnCommandOveredHandle(this, item);
          }
        }
      }
      return this;
    }
    public void Discard() {
      this.Navigation = null;
      CommandState = CommandState.Popped;
      base.Clear();
    }

  }



}
