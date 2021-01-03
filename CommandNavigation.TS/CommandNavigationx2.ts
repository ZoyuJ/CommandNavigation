/// <reference path="main.ts" />
/// <reference path="Interfaces.ts" />
/// <reference path="CommandNavigationx4.ts"/>
/// <reference path="../KatKits/KatKits.TS/Event.ts"/>

namespace CommandNavigation {
  export namespace CommandNavigationx2 {
    export class CommandNavigation extends Array<ICommandCtrl> {
      public readonly OnPopped: KatKits.Event2<CommandNavigation, ICommandCtrl> = new KatKits.Event2<CommandNavigation, ICommandCtrl>();
      public readonly OnPushed: KatKits.Event2<CommandNavigation, ICommandCtrl> = new KatKits.Event2<CommandNavigation, ICommandCtrl>();

      constructor() {
        super();
      }
      public CurrentOrder(): number { return this.length === 0 ? Number.MIN_SAFE_INTEGER : this.Peek().Order; }
      public NextOrder(): number { return this.length === 0 ? 0 : this.Peek().Order + 10; }

      public Peek(): ICommandCtrl { return this[this.length - 1]; }

      public Push(Item: ICommandCtrl): void {
        if (this.length === 0) {
          super.push(Item);
          Item.OnPush();
          this.OnPushed.Invoke(this, Item);
        }
        else if (Item.Order <= this.Peek().Order) {
          this.Pop();
          this.Push(Item);
        }
        else {
          super.push(Item);
          Item.OnPush();
          this.OnPushed.Invoke(this, Item);
        }
      }

      public Pop(): ICommandCtrl {
        if (this.length > 0) {
          this.Peek().OnPop();
          const Popped = super.pop();
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
  }

}


