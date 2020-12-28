namespace CommandNavigation.Command1Navigation2 {
  using System;
  using System.Collections.Generic;
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

    public int CurrentOrder { get => Count == 0 ? int.MinValue : Peek().Order; }
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
        base.Push(Item);
        Item.OnPush();
        OnPushed?.Invoke(this, Item);
      }
      else if (Item.Order <= Peek().Order) {
        Pop();
        this.Push(Item);
      }
      else {
        base.Push(Item);
        Item.OnPush();
        OnPushed?.Invoke(this, Item);
      }
    }
    public new T Pop() {
      if (Count > 0) {
        var Popped = base.Pop();
        Popped.OnPop();
        OnPopped?.Invoke(this, Popped);
        return Popped;
      }
      throw new IndexOutOfRangeException("Empty Stack");
    }


  }

  public delegate void OnCommandPushedHandle<T>(CommandNavigation<T> CurrentStack, T PushedItem) where T : class, ICommandCtrl;
  public delegate void OnCommandPoppedHandle<T>(CommandNavigation<T> CurrentStack, T PoppedItem) where T : class, ICommandCtrl;

}
