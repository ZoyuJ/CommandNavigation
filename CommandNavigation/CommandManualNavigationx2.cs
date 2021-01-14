namespace CommandNavigation.Command1Navigation2 {
  using System;
  using System.Collections.Generic;
  using System.Text;

  public class CommandManualNavigation<T> : Stack<T> where T : class, ICommandManualCtrl {
    public CommandManualNavigation() : base() { }
    public CommandManualNavigation(int Capacity) : base(Capacity) { }

    public event OnCommandManualPoppedHandle<T> OnPopped;
    public event OnCommandManualPushedHandle<T> OnPushed;

    public new void Clear() {
      while (Count > 0) {
        this.Pop();
      }
    }
    public void Discard() { base.Clear(); }

    public new void Push(T Item) {
      base.Push(Item);
      Item.OnPush();
      OnPushed?.Invoke(this, Item);
    }
    public new T Pop() {
      var Item = base.Pop();
      Item.OnPop();
      OnPopped?.Invoke(this, Item);
      return Item;
    }

  }
  public delegate void OnCommandManualPushedHandle<T>(CommandManualNavigation<T> CurrentStack, T PushedItem) where T : class, ICommandManualCtrl;
  public delegate void OnCommandManualPoppedHandle<T>(CommandManualNavigation<T> CurrentStack, T PoppedItem) where T : class, ICommandManualCtrl;
}
