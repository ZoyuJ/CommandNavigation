namespace CommandNavigation.Command1Navigation4 {
  using System;
  using System.Collections.Generic;
  using System.Text;

  using CommandNavigation;
  public class CommandManualNavigation<T> : Stack<T> where T : class, ICommandManualCtrlx4 {
    public CommandManualNavigation() : base() { }
    public CommandManualNavigation(int Capacity) : base(Capacity) { }

    public event OnCommandManualOveredHandle<T> OnCommandOvered;
    public event OnCommandManualPoppedHandle<T> OnCommandPopped;
    public event OnCommandManualPushedHandle<T> OnCommandPushed;
    public event OnCommandManualToppedHandle<T> OnCommnadTopped;

    public new void Clear() {
      while (Count > 0) {
        base.Pop().OnPop();
      }
    }
    public new void Push(T Item) {
      if (Count > 0) {
        var TopItem = base.Peek();
        TopItem.OnOver();
        OnCommandOvered?.Invoke(this, TopItem);
      }
      base.Push(Item);
      Item.OnPush();
      OnCommandPushed?.Invoke(this, Item);
    }
    public new T Pop() {
      var Item = base.Pop();
      Item.OnPop();
      OnCommandPopped?.Invoke(this, Item);
      if (Count > 0) {
        var Item2 = base.Peek();
        Item2.OnTop();
        OnCommnadTopped?.Invoke(this, Item2);
      }
      return Item;
    }
    public void Discard() { base.Clear(); }


  }

  public delegate void OnCommandManualPushedHandle<T>(CommandManualNavigation<T> CurrentStack, T PushedItem) where T : class, ICommandManualCtrlx4;
  public delegate void OnCommandManualPoppedHandle<T>(CommandManualNavigation<T> CurrentStack, T PoppedItem) where T : class, ICommandManualCtrlx4;
  public delegate void OnCommandManualToppedHandle<T>(CommandManualNavigation<T> CurrentStack, T ToppedItem) where T : class, ICommandManualCtrlx4;
  public delegate void OnCommandManualOveredHandle<T>(CommandManualNavigation<T> CurrentStack, T OveredItem) where T : class, ICommandManualCtrlx4;

}
