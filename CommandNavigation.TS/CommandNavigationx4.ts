import { ICommandCtrlx4, CommandState } from "./Interfaces";

export namespace CommandNavigationx4 {

  export type OnCommandPushedHandle = (CurrentStack: CommandNavigation, PushedItem: ICommandCtrlx4) => void;
  export type OnCommandPoppedHandle = (CurrentStack: CommandNavigation, PoppedItem: ICommandCtrlx4) => void;
  export type OnCommandToppedHandle = (CurrentStack: CommandNavigation, ToppedItem: ICommandCtrlx4) => void;
  export type OnCommandOveredHandle = (CurrentStack: CommandNavigation, OveredItem: ICommandCtrlx4) => void;

  export class CommandNavigation extends Array {
    public readonly OnPopped: OnCommandPoppedHandle[];
    public readonly OnPushed: OnCommandPushedHandle[];
    public readonly OnTopped: OnCommandToppedHandle[];
    public readonly OnOvered: OnCommandOveredHandle[];

    constructor() {
      super();
      this.OnPopped = [];
      this.OnPushed = [];
      this.OnTopped = [];
      this.OnOvered = [];
    }

    public CurrentOrder(): number { return super.length === 0 ? Number.MAX_SAFE_INTEGER : this.Peek().Order; }
    public NextOrder(): number { return super.length === 0 ? 0 : this.Peek().Order + 10; }

    public Peek(): ICommandCtrlx4 { return this[this.length - 1]; }

    public Push(Item: ICommandCtrlx4) {
      if (super.length === 0) {
        Item.CommandState = CommandState.Topped;
        Item.OnPush();
        super.push(Item);
        if (this.OnPushed !== null && this.OnPushed.length > 0) this.OnPushed.forEach(E => E(this, Item));
      }
      else if (Item.Order <= this.Peek().Order) {
        const Popped = super.pop();
        Item.CommandState = CommandState.Popped;
        Popped.OnPop();
        if (this.OnPopped !== null && this.OnPopped.length > 0) this.OnPopped.forEach(E => E(this, Popped));
        this.Push(Item);
      }
      else {
        const Topped = this.Peek();
        if (Topped.CommandState === CommandState.Topped) {
          Topped.CommandState = CommandState.Overed;
          Topped.OnOver();
          if (this.OnOvered !== null && this.OnOvered.length > 0) this.OnOvered.forEach(E => E(this, Topped));
        }
        Item.CommandState = CommandState.Topped;
        Item.OnPush();
        super.push(Item);
        if (this.OnPushed !== null && this.OnPushed.length > 0) this.OnPushed.forEach(E => E(this, Item));
      }
    }
    public Pop(): ICommandCtrlx4 {
      if (super.length > 0) {
        this.Peek().OnPop();
        const Popped = super.pop();
        Popped.CommandState = CommandState.Popped;
        if (this.OnPopped !== null && this.OnPopped.length > 0) this.OnPopped.forEach(E => E(this, Popped));
        if (super.length > 0) {
          const Overed = this.Peek();
          Overed.CommandState = CommandState.Topped;
          Overed.OnTop();
          if (this.OnTopped !== null && this.OnTopped.length > 0) this.OnTopped.forEach(E => E(this, Overed));
        }
        return Popped;
      }
      throw Error("Empty Stack");
    }

    public Clear() {
      while (super.length > 0) {
        const Popped = super.pop();
        Popped.CommandState = CommandState.Popped;
        Popped.OnPop();
        if (this.OnPopped !== null && this.OnPopped.length > 0) this.OnPopped.forEach(E => E(this, Popped));
      }
    }
    public Discard() { super.splice(0, super.length); }
  }
}