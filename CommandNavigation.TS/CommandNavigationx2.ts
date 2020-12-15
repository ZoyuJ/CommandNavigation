import { ICommandCtrl } from "./Interfaces";
export namespace CommandNavigationx2 {

  export type OnCommandPushedHandle = (CurrentStack: CommandNavigation, PushedItem: ICommandCtrl) => void;
  export type OnCommandPoppedHandle = (CurrentStack: CommandNavigation, PoppedItem: ICommandCtrl) => void;

  export class CommandNavigation extends Array {
    public readonly OnPopped: OnCommandPoppedHandle[];
    public readonly OnPushed: OnCommandPushedHandle[];

    constructor() {
      super();
      this.OnPushed = [];
      this.OnPopped = [];
    }
    public CurrentOrder(): number { return super.length === 0 ? Number.MIN_SAFE_INTEGER : this.Peek().Order; }
    public NextOrder(): number { return super.length === 0 ? 0 : this.Peek().Order + 10; }

    public Peek(): ICommandCtrl { return this[this.length - 1]; }

    public Push(Item: ICommandCtrl): void {
      if (this.length === 0) {
        Item.OnPush();
        super.push(Item);
        if (this.OnPushed !== null && this.OnPushed.length > 0) this.OnPushed.forEach(E => E(this, Item));
      }
      else if (Item.Order <= this.Peek().Order) {
        this.Pop();
        this.Push(Item);
      }
      else {
        Item.OnPush();
        super.push(Item);
        if (this.OnPushed !== null && this.OnPushed.length > 0) this.OnPushed.forEach(E => E(this, Item));
      }
    }

    public Pop(): ICommandCtrl {
      if (super.length > 0) {
        this.Peek().OnPop();
        const Popped = super.pop();
        if (this.OnPopped !== null && this.OnPopped.length > 0) this.OnPopped.forEach(E => E(this, Popped));
        return Popped;
      }
      throw Error("Empty Stack");
    }

    public Clear() {
      while (super.length > 0) {
        this.Pop();
      }
    }
    public Discard() { super.splice(0, super.length); }
  }
}

