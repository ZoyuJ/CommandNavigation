namespace CommandNavigation.CommandnNavigation4 {
  using System;
  using System.Collections.Generic;

  public partial class CommandNavigation<TCommand> : Stack<CommandChain<TCommand>> where TCommand : class, ICommandCtrlx4 {
    /// <summary>
    /// 有节点被压栈
    /// </summary>
    public event OnCommandPushedHandle<TCommand> OnPushed;
    internal void InvokeOnCommandPushedHandle(CommandChain<TCommand> Chain, TCommand Item) => OnPushed?.Invoke(this, Chain, Item);
    /// <summary>
    /// 有节点被出栈
    /// </summary>
    public event OnCommandPoppedHandle<TCommand> OnPopped;
    internal void InvokeOnCommandPoppedHandle(CommandChain<TCommand> Chain, TCommand Item) => OnPopped?.Invoke(this, Chain, Item);
    /// <summary>
    /// 有节点回到栈顶
    /// </summary>
    public event OnCommandToppedHandle<TCommand> OnTopped;
    internal void InvokeOnCommandToppedHandle(CommandChain<TCommand> Chain, TCommand Item) => OnTopped?.Invoke(this, Chain, Item);
    /// <summary>
    /// 有节点压到栈底
    /// </summary>
    public event OnCommandOveredHandle<TCommand> OnOvered;
    internal void InvokeOnCommandOveredHandle(CommandChain<TCommand> Chain, TCommand Item) => OnOvered?.Invoke(this, Chain, Item);

    /*
        Push 1                                Push 2-1                            Push 2-2
        Push 2                                Push 3                            
                                              Push 3-1                          
    
         ┃         ┃                          ┃         ┃                         ┃           ┃
         ┃         ┃  ← CommandChain          ┃         ┃  ← CommandChain         ┃           ┃  ← CommandChain
         ┃         ┃  ← CommandChain          ┃ 3  3-1  ┃  ← CommandChain         ┃           ┃  ← CommandChain
         ┃ 2       ┃  ← CommandChain    =》   ┃ 2  2-1  ┃  ← CommandChain     =》 ┃ 2 2-1 2-2 ┃  ← CommandChain
         ┃ 1       ┃  ← CommandChain          ┃ 1       ┃  ← CommandChain         ┃ 1         ┃  ← CommandChain
         ┗━━━━━━━━━┛                          ┗━━━━━━━━━┛                         ┗━━━━━━━━━━━┛
              ↑                                     
      CommandNavigation                         
     */

    public CommandNavigation() : base() { }
    public CommandNavigation(int Capacity) : base(Capacity) { }

    /// <summary>
    /// 当前Order
    /// </summary>
    public int CurrentOrder { get => Count == 0 ? int.MinValue : Peek().Order; }
    /// <summary>
    /// 下一级Order
    /// </summary>
    public int NextOrder { get => Count == 0 || Peek().Count <= 0 ? 0 : Peek().Order + 10; }

    /// <summary>
    /// 清空
    /// </summary>
    public new void Clear() {
      while (Count > 0) {
        var Chain = this.Pop();
        Chain.Clear();
      }
    }
    /// <summary>
    /// 放弃
    /// </summary>
    public void Discard() {
      base.Clear();
    }

    /// <summary>
    /// 压栈
    /// </summary>
    /// <param name="Chain"></param>
    public void Push(IEnumerable<TCommand> Items) {
      Push(new CommandChain<TCommand>(this, Items));
    }
    /// <summary>
    /// 压栈
    /// </summary>
    /// <param name="Chain"></param>
    public new void Push(CommandChain<TCommand> Chain) {
      if (Count == 0) {
        base.Push(Chain);
        Chain.OnPush(this);
      }
      else if (Chain.Order <= Peek().Order) {
        base.Pop().Clear();
        Push(Chain);
      }
      else {
        Peek().OnOver();
        base.Push(Chain);
        Chain.OnPush(this);
      }
    }

    /// <summary>
    /// 压栈节点
    /// </summary>
    /// <param name="Item"></param>
    public void Push(TCommand Item) {
      if (Count == 0) {
        var Chain = new CommandChain<TCommand>(this, Item.Order);
        Chain.OnPush();
        Chain.Add(Item);
        base.Push(Chain);
      }
      else if (Item.Order == Peek().Order) {
        Peek().Add(Item);
      }
      else if (Item.Order < Peek().Order) {
        base.Pop().Clear();
        Push(Item);
      }
      else {
        Peek().OnOver();
        var Chain = new CommandChain<TCommand>(this, Item.Order);
        base.Push(Chain);
        Chain.OnPush();
        Chain.Add(Item);
      }
    }

    /// <summary>
    /// 出栈
    /// </summary>
    /// <returns></returns>
    public new CommandChain<TCommand> Pop() {
      if (Count > 0) {
        var Poppeds = base.Pop().OnPop();
        if (Count > 0) {
          Peek().OnTop();
        }
        return Poppeds;
      }
      throw new IndexOutOfRangeException("Empty Stack");
    }
    /// <summary>
    /// 出栈栈顶一个节点
    /// </summary>
    /// <param name="Match"></param>
    public TCommand Pop(Predicate<TCommand> Match) {
      if (Count > 0) {
        if (Peek().Remove(Match, out var Item)) {
          if (Peek().Count == 0) {
            base.Pop();
            if (Count > 0) {
              Peek().OnTop();
            }
          }
          return Item;
        }
        return null;
      }
      throw new IndexOutOfRangeException("Empty Stack");
    }

    /// <summary>
    /// 
    /// if matched, gonna invoke onpush then onover immdiately
    /// </summary>
    /// <param name="Match"></param>
    public bool PushInside(Predicate<CommandChain<TCommand>> Match, TCommand Item) {
      foreach (var Level in this) {
        if (Level.Count > 0 && Match(Level)) {
          Item.Order = Level.Order;
          Level.Add(Item);
          return true;
        }
      }
      return false;
    }
    /// <summary>
    /// 出栈栈内节点
    /// </summary>
    /// <param name="Match"></param>
    public TCommand PopInside(Predicate<TCommand> Match) {
      foreach (var Level in this) {
        if (Level.Remove(Match, out var Item)) {
          return Item;
        }
      }
      return null;
    }



  }
  public delegate void OnCommandPushedHandle<TCommand>(Stack<CommandChain<TCommand>> CurrentNavigation, CommandChain<TCommand> CurrentChain, TCommand PushedItem) where TCommand : class, ICommandCtrlx4;
  public delegate void OnCommandPoppedHandle<TCommand>(Stack<CommandChain<TCommand>> CurrentNavigation, CommandChain<TCommand> CurrentChain, TCommand PoppedItem) where TCommand : class, ICommandCtrlx4;
  public delegate void OnCommandToppedHandle<TCommand>(Stack<CommandChain<TCommand>> CurrentNavigation, CommandChain<TCommand> CurrentChain, TCommand ToppedItem) where TCommand : class, ICommandCtrlx4;
  public delegate void OnCommandOveredHandle<TCommand>(Stack<CommandChain<TCommand>> CurrentNavigation, CommandChain<TCommand> CurrentChain, TCommand OveredItem) where TCommand : class, ICommandCtrlx4;

}
