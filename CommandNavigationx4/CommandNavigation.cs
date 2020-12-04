namespace CommandNavigation.Command1Navigation4 {
  using System;
  using System.Collections.Generic;
  using System.Diagnostics.CodeAnalysis;
  using System.Text;

  /// <summary>
  /// Push:  -> Item -> if(has item){ -> invoke Peek.OnOver -> invoke OnOvered event } -> Push in stack -> invoke OnPush -> invoke OnPushed event
  /// Pop:   -> invoke OnPop -> Pop form Stack -> invoke OnPopped event -> if(has item){ -> invoke Peek.OnTop->invoke OnTopped event } -> Item
  /// TryPop:-> Pop form Stack -> invoke OnPop -> invoke OnPopped event -> if(has item){ -> invoke Peek.OnTop->invoke OnTopped event } -> out Item
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class CommandNavigation<T> : Stack<T> where T : class, ICommandCtrlx4 {
    public event OnCommandPoppedHandle<T> OnPopped;
    public event OnCommandPushedHandle<T> OnPushed;
    public event OnCommandToppedHandle<T> OnTopped;
    public event OnCommandOveredHandle<T> OnOvered;

    public CommandNavigation() : base() { }
    public CommandNavigation(int Capacity) : base(Capacity) { }

    public int CurrentOrder { get => Count == 0 ? 0 : Peek().Order; }
    public int NextOrder { get => Count == 0 ? 0 : Peek().Order + 10; }

    public new void Clear() {
      while (Count > 0) {
        var Popped = base.Pop();
        Popped.CommandState = CommandState.Popped;
        Popped.OnPop();
        OnPopped?.Invoke(this, Popped);
      }
    }
    public void Discard() {
      base.Clear();
    }
    public new void Push(T Item) {
      if (Count == 0) {
        base.Push(Item);
        Item.CommandState = CommandState.Topped;
        Item.OnPush();
        OnPushed?.Invoke(this, Item);
      }
      else if (Item.Order <= Peek().Order) {
        var Popped = base.Pop();
        Item.CommandState = CommandState.Popped;
        Popped.OnPop();
        OnPopped?.Invoke(this, Popped);
        this.Push(Item);
      }
      else {
        var Topped = Peek();
        if (Topped.CommandState == CommandState.Topped) {
          Topped.CommandState = CommandState.Overed;
          Topped.OnOver();
          OnOvered?.Invoke(this, Topped);
        }
        base.Push(Item);
        Item.CommandState = CommandState.Topped;
        Item.OnPush();
        OnPushed?.Invoke(this, Item);
      }
    }
    public new T Pop() {
      if (Count > 0) {
        Peek().OnPop();
        var Popped = base.Pop();
        Popped.CommandState = CommandState.Popped;
        OnPopped?.Invoke(this, Popped);
        if (Count > 0) {
          Peek().CommandState = CommandState.Topped;
          Peek().OnTop();
          OnTopped?.Invoke(this, Peek());
        }
        return Popped;
      }
      throw new IndexOutOfRangeException("Empty Stack");
    }
    public new bool TryPop(out T Popped) {
      if (base.TryPop(out Popped)) {
        Popped.CommandState = CommandState.Popped;
        Popped.OnPop();
        this.OnPopped?.Invoke(this, Popped);
        if (Count > 0) {
          Peek().CommandState = CommandState.Topped;
          Peek().OnTop();
          OnTopped?.Invoke(this, Peek());
        }
        return true;
      }
      return false;
    }
  }

  public delegate void OnCommandPushedHandle<T>(CommandNavigation<T> CurrentStack, T PushedItem) where T : class, ICommandCtrlx4;
  public delegate void OnCommandPoppedHandle<T>(CommandNavigation<T> CurrentStack, T PoppedItem) where T : class, ICommandCtrlx4;
  public delegate void OnCommandToppedHandle<T>(CommandNavigation<T> CurrentStack, T ToppedItem) where T : class, ICommandCtrlx4;
  public delegate void OnCommandOveredHandle<T>(CommandNavigation<T> CurrentStack, T OveredItem) where T : class, ICommandCtrlx4;

}
