namespace CommandNavigation.CommandnNavigation4 {
  using System;
  using System.Collections.Generic;
  using System.Diagnostics.CodeAnalysis;
  using System.Linq;
  using System.Text;

  public partial class CommandNavigation<TCommand/*, TChain*/> : Stack<CommandChain<TCommand>> where TCommand : class, ICommandCtrlx4 /*where TChain : CommandChain<TCommand>*/ {

    public event OnCommandPushedHandle<TCommand> OnPushed;
    internal void InvokeOnCommandPushedHandle(CommandChain<TCommand> Chain, TCommand Item) => OnPushed?.Invoke(this, Chain, Item);
    public event OnCommandPoppedHandle<TCommand> OnPopped;
    internal void InvokeOnCommandPoppedHandle(CommandChain<TCommand> Chain, TCommand Item) => OnPopped?.Invoke(this, Chain, Item);
    public event OnCommandToppedHandle<TCommand> OnTopped;
    internal void InvokeOnCommandToppedHandle(CommandChain<TCommand> Chain, TCommand Item) => OnTopped?.Invoke(this, Chain, Item);
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

    public int CurrentOrder { get => Count == 0 ? int.MinValue : Peek().Order; }
    public int NextOrder { get => Count == 0 || Peek().Count <= 0 ? 0 : Peek().Order + 10; }

    public new void Clear() {
      while (Count > 0) {
        var Chain = this.Pop();
        Chain.Clear();
      }
    }
    public void Discard() {
      base.Clear();
    }
    [Obsolete("Not Support")] public new void Push(CommandChain<TCommand> Item) => throw new NotImplementedException("Obsolete");
    public void Push(TCommand Item) {
      if (Count == 0) {
        base.Push(new CommandChain<TCommand>(this, Item));
        Item.CommandState = CommandState.Pushed;
        Item.OnPush();
        OnPushed?.Invoke(this, Peek(), Item);
      }
      else if (Item.Order == Peek().Order) {
        Peek().Add(Item);
        Item.CommandState = CommandState.Pushed;
        Item.OnPush();
        OnPushed?.Invoke(this, Peek(), Item);
      }
      else if (Item.Order > Peek().Order) {
        var Poppeds = this.Pop();
        //Poppeds.
        Push(Item);
      }
      else {
        base.Push(new CommandChain<TCommand>(this, Item));
        Item.CommandState = CommandState.Pushed;
        Item.OnPush();
        OnPushed?.Invoke(this, Peek(), Item);
      }
      //if (Count == 0) {
      //  base.Push(Item);
      //  Item.OnPush();
      //  OnPushed?.Invoke(this, Item);
      //}
      //else if (Item.Order <= Peek().Order) {
      //  this.Pop();
      //  this.Push(Item);
      //}
      //else {
      //  Peek().OnOver();
      //  OnOvered?.Invoke(this, Peek());
      //  base.Push(Item);
      //  Item.OnPush();
      //  OnPushed?.Invoke(this, Item);
      //}
    }
    public new CommandChain<TCommand> Pop() {
      if (Count > 0) {
        var Poppeds = Peek();
        foreach (var Item in Poppeds) {
          Item.CommandState = CommandState.Popped;
          Item.OnTop();
          OnTopped(this, Poppeds, Item);
        }
        if (Count > 0) {
          var Topped = Peek();
          foreach (var Item in Topped) {
            Item.CommandState = CommandState.Topped;
            Item.OnTop();
            OnTopped(this, Topped, Item);
          }
        }
        return Poppeds;
      }
      throw new IndexOutOfRangeException("Empty Stack");
    }
    public void Pop(Predicate<TCommand> Match) {
      if (Count > 0) {
        var Item = /*(IEnumerable<TCommand>)*/Peek().FirstOrDefault(E => Match(E));
        if (Item != null) {
          Item.CommandState = CommandState.Popped;
          Item.OnPop();
          OnPopped?.Invoke(this, Peek(), Item);
          if (Peek().Count == 0) {
            base.Pop();
            if (Count > 0) {
              var Overeds = Peek();
              while (Overeds.Count <= 0) {
                base.Pop();
                Overeds = Peek();
              }
              foreach (var OItem in Overeds) {
                OItem.CommandState = CommandState.Popped;
                OItem.OnTop();
                OnTopped(this, Overeds, OItem);
              }
            }
          }
        }
      }
      throw new IndexOutOfRangeException("Empty Stack");
    }
    /// <summary>
    /// 
    /// if matched, gonna invoke onpush then onover immdiately
    /// </summary>
    /// <param name="Match"></param>
    public void Insert(Predicate<CommandChain<TCommand>> Match, TCommand Item) {
      foreach (var Level in this) {
        if (Level.Count > 0 && Match(Level)) {
          Item.Order = Level.Order;
          Level.Add(Item);
          if (Level != Peek()) {
            Item.CommandState = CommandState.Overed;
            Item.OnOver();
            OnOvered?.Invoke(this, Level, Item);
          }
        }
      }
    }
    /// <summary>
    /// 
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

    //public new bool TryPop([MaybeNullWhen(false)] out CommandChain<TCommand> Popped) {
    //  if (base.TryPop(out Popped)) {
    //    Popped.OnPop();
    //    this.OnPopped?.Invoke(this, Popped);
    //    if (Count > 0) {
    //      Peek().OnTop();
    //      OnTopped?.Invoke(this, Peek());
    //    }
    //    return true;
    //  }
    //  return false;
    //}


  }
  public delegate void OnCommandPushedHandle<T>(Stack<CommandChain<T>> CurrentNavigation, CommandChain<T> CurrentChain, T PushedItem) where T : class, ICommandCtrlx4;
  public delegate void OnCommandPoppedHandle<T>(Stack<CommandChain<T>> CurrentNavigation, CommandChain<T> CurrentChain, T PoppedItem) where T : class, ICommandCtrlx4;
  public delegate void OnCommandToppedHandle<T>(Stack<CommandChain<T>> CurrentNavigation, CommandChain<T> CurrentChain, T ToppedItem) where T : class, ICommandCtrlx4;
  public delegate void OnCommandOveredHandle<T>(Stack<CommandChain<T>> CurrentNavigation, CommandChain<T> CurrentChain, T OveredItem) where T : class, ICommandCtrlx4;
}
