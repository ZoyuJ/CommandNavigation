import { ICommandManualCtrlx4 } from "./Interfaces";
import { Event2 } from "./../node_modules/katkits.ts/lib/Event"
export namespace CommandNavigationx4 {
  export class CommandManualNavigation extends Array<ICommandManualCtrlx4>{
    public readonly OnPopped: Event2<CommandManualNavigation, ICommandManualCtrlx4>;
    public readonly OnPushed: Event2<CommandManualNavigation, ICommandManualCtrlx4>;
    public readonly OnOvered: Event2<CommandManualNavigation, ICommandManualCtrlx4>;
    public readonly OnTopped: Event2<CommandManualNavigation, ICommandManualCtrlx4>;

    constructor() {
      super();
      this.OnPopped = new Event2();
      this.OnPushed = new Event2();
    }

    public Peek(): ICommandManualCtrlx4 { return this[this.length - 1]; }

    public Push(Item: ICommandManualCtrlx4): void {
      if (this.length > 0) {
        const TopItem = this.Peek();
        TopItem.OnOver();
        this.OnOvered.Invoke(this, TopItem);
      }
      super.push(Item);
      Item.OnPush();
      this.OnPushed.Invoke(this, Item);
    }

    public Pop(): ICommandManualCtrlx4 {
      if (this.length > 0) {
        const Popped = super.pop();
        Popped.OnPop();
        this.OnPopped.Invoke(this, Popped);
        if (this.length > 0) {
          const Item2 = this.Peek();
          Item2.OnTop();
          this.OnTopped.Invoke(this, Item2);
        }
        return Popped;
      }
      throw Error("Empty Stack");
    }

    public Clear() {
      while (this.length > 0) {
        this.Pop();
      }
    }
    public Discard() { super.splice(0, this.length); }


  }
}