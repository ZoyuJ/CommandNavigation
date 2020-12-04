namespace CommandNavigation.Command1Navigation2 {
  using System;
  using System.Collections.Generic;
  using System.Diagnostics.CodeAnalysis;
  using System.Text;
  /// <summary>
  /// Push:  -> Item -> Push in stack -> invoke OnPush -> invoke OnPushed event
  /// Pop:   -> invoke OnPop -> Pop form Stack -> invoke OnPopped event -> Item
  /// TryPop:-> Pop form Stack -> invoke OnPop -> invoke OnPopped event -> out Item
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class CommandNavigation<T> : Stack<T> where T : class, ICommandCtrl {
    public event OnCommandPoppedHandle<T> OnPopped;
    public event OnCommandPushedHandle<T> OnPushed;

    public CommandNavigation() : base() { }
    public CommandNavigation(int Capacity) : base(Capacity) { }

    public int CurrentOrder { get => Count == 0 ? 0 : Peek().Order; }
    public int NextOrder { get => Count == 0 ? 0 : Peek().Order + 10; }

    public new void Clear() {
      while (Count > 0) {
        this.Pop();
      }
    }
    public void Discard() {
      base.Clear();
    }
    public new void Push(T Item) {
      if (Count == 0) {
        Item.OnPush();
        base.Push(Item);
        OnPushed?.Invoke(this, Item);
      }
      else if (Item.Order <= Peek().Order) {
        Pop();
        this.Push(Item);
      }
      else {
        Item.OnPush();
        base.Push(Item);
        OnPushed?.Invoke(this, Item);
      }
    }
    public new T Pop() {
      if (Count > 0) {
        Peek().OnPop();
        var Popped = base.Pop();
        OnPopped?.Invoke(this, Popped);
        return Popped;
      }
      throw new IndexOutOfRangeException("Empty Stack");
    }
    public new bool TryPop(out T Popped) {
      if (base.TryPop(out Popped)) {
        Popped.OnPop();
        this.OnPopped?.Invoke(this, Popped);
        return true;
      }
      return false;
    }

  }

  public delegate void OnCommandPushedHandle<T>(CommandNavigation<T> CurrentStack, T PushedItem) where T : class, ICommandCtrl;
  public delegate void OnCommandPoppedHandle<T>(CommandNavigation<T> CurrentStack, T PoppedItem) where T : class, ICommandCtrl;

}
