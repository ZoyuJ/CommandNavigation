import { ICommandManualCtrl } from "./Interfaces";
import { Event2 } from "./../node_modules/katkits.ts/lib/Event"

export default class CommandManualNavigation extends Array<ICommandManualCtrl>{
  public readonly OnPopped: Event2<CommandManualNavigation, ICommandManualCtrl>;
  public readonly OnPushed: Event2<CommandManualNavigation, ICommandManualCtrl>;

  constructor() {
    super();
    this.OnPopped = new Event2();
    this.OnPushed = new Event2();
  }

  public Peek(): ICommandManualCtrl { return this[this.length - 1]; }

  public Push(Item: ICommandManualCtrl): void {
    super.push(Item);
    Item.OnPush();
    this.OnPushed.Invoke(this, Item);
  }

  public Pop(): ICommandManualCtrl {
    if (this.length > 0) {
      const Popped = super.pop();
      Popped.OnPop();
      this.OnPopped.Invoke(this, Popped);
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
