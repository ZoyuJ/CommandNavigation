namespace CommandNavigation {
  using System;
  using System.Collections.Generic;
  using System.Text;

  public class CommandStack<T> : Stack<T> {

    /// <summary>
    /// 
    /// </summary>
    /// <param name="OnPushed"></param>
    /// <param name="OnPopped"></param>
    /// <param name="Detecting">left:Top item in stack;right:item need push;return:True->pop the top one</param>
    public CommandStack(Action<T> OnPushed, Action<T> OnPopped, Func<T, T, bool> Detecting) : base() {
      _OnPushed = OnPushed ?? (E => { });
      _OnPopped = OnPopped ?? (E => { });
      _Detecting = Detecting ?? ((L, R) => false);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="OnPushed"></param>
    /// <param name="OnPopped"></param>
    /// <param name="Detecting">left:Top item in stack;right:item need push;return:True->pop the top one</param>
    /// <param name="Capacity"></param>
    public CommandStack(Action<T> OnPushed, Action<T> OnPopped, Func<T, T, bool> Detecting, int Capacity) : base(Capacity) {
      _OnPushed = OnPushed ?? (E => { });
      _OnPopped = OnPopped ?? (E => { });
      _Detecting = Detecting ?? ((L, R) => false);
    }

    protected readonly Action<T> _OnPushed;
    protected readonly Action<T> _OnPopped;
    protected readonly Func<T, T, bool> _Detecting;

    public new void Push(T Item) {
      if (Count == 0) {
        base.Push(Item);
        _OnPushed(Item);
      }
      else if (_Detecting(Peek(), Item)) {
        _OnPopped(base.Pop());
        Push(Item);
      }
      else {
        base.Push(Item);
        _OnPushed(Item);
      }
    }

    public new T Pop() {
      var Item = base.Pop();
      _OnPopped(Item);
      return Item;
    }

    public new void Clear() {
      while (Count > 0) Pop();
    }

    public void Discard() {
      base.Clear();
    }
  }
}
