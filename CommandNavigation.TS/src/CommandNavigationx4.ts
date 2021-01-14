import { ICommandCtrlx4, CommandState } from "./Interfaces";
import { Event2 } from "./../node_modules/katkits.ts/lib/Event"
export default class CommandNavigation extends Array<ICommandCtrlx4> {
  public readonly OnPopped: Event2<CommandNavigation, ICommandCtrlx4>;
  public readonly OnPushed: Event2<CommandNavigation, ICommandCtrlx4>;
  public readonly OnTopped: Event2<CommandNavigation, ICommandCtrlx4>;
  public readonly OnOvered: Event2<CommandNavigation, ICommandCtrlx4>;

  constructor() {
    super();
    this.OnPopped = new Event2();
    this.OnPushed = new Event2();
    this.OnTopped = new Event2();
    this.OnOvered = new Event2();
  }

  public CurrentOrder(): number { return this.length === 0 ? Number.MAX_SAFE_INTEGER : this.Peek().Order; }
  public NextOrder(): number { return this.length === 0 ? 0 : this.Peek().Order + 10; }

  public Peek(): ICommandCtrlx4 { return this[this.length - 1]; }

  public Push(Item: ICommandCtrlx4) {
    if (this.length === 0) {
      Item.CommandState = CommandState.Topped;
      Item.OnPush();
      super.push(Item);
      this.OnPushed.Invoke(this, Item);
    }
    else if (Item.Order <= this.Peek().Order) {
      const Popped = super.pop();
      Item.CommandState = CommandState.Popped;
      Popped.OnPop();
      this.OnPopped.Invoke(this, Popped);
      this.Push(Item);
    }
    else {
      const Topped = this.Peek();
      if (Topped.CommandState === CommandState.Topped) {
        Topped.CommandState = CommandState.Overed;
        Topped.OnOver();
        this.OnOvered.Invoke(this, Topped);
      }
      Item.CommandState = CommandState.Topped;
      Item.OnPush();
      super.push(Item);
      this.OnPushed.Invoke(this, Item);
    }
  }
  public Pop(): ICommandCtrlx4 {
    if (this.length > 0) {
      this.Peek().OnPop();
      const Popped = super.pop();
      Popped.CommandState = CommandState.Popped;
      this.OnPopped.Invoke(this, Popped);
      if (this.length > 0) {
        const Overed = this.Peek();
        Overed.CommandState = CommandState.Topped;
        Overed.OnTop();
        this.OnTopped.Invoke(this, Overed);
      }
      return Popped;
    }
    throw Error("Empty Stack");
  }

  public Clear() {
    while (this.length > 0) {
      const Popped = super.pop();
      Popped.CommandState = CommandState.Popped;
      Popped.OnPop();
      this.OnPopped.Invoke(this, Popped);
    }
  }
  public Discard() { super.splice(0, this.length); }
}
