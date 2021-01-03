
/// <reference path="main.ts" />
/// <reference path="Interfaces.ts"/>
/// <reference path="CommandNavigationx2.ts"/>
/// <reference path="../KatKits/KatKits.TS/Event.ts"/>

namespace CommandNavigation {
  namespace CommandNavigationx4 {
    export class CommandNavigation extends Array<ICommandCtrlx4> {
      public readonly OnPopped: KatKits.Event2<CommandNavigation, ICommandCtrlx4> = new KatKits.Event2<CommandNavigation, ICommandCtrlx4>();
      public readonly OnPushed: KatKits.Event2<CommandNavigation, ICommandCtrlx4> = new KatKits.Event2<CommandNavigation, ICommandCtrlx4>();
      public readonly OnTopped: KatKits.Event2<CommandNavigation, ICommandCtrlx4> = new KatKits.Event2<CommandNavigation, ICommandCtrlx4>();
      public readonly OnOvered: KatKits.Event2<CommandNavigation, ICommandCtrlx4> = new KatKits.Event2<CommandNavigation, ICommandCtrlx4>();

      constructor() {
        super();
      }

      public CurrentOrder(): number { return this.length === 0 ? Number.MAX_SAFE_INTEGER : this.Peek().Order; }
      public NextOrder(): number { return this.length === 0 ? 0 : this.Peek().Order + 10; }

      public Peek(): ICommandCtrlx4 { return this[this.length - 1]; }

      public Push(Item: ICommandCtrlx4) {
        if (this.length === 0) {
          super.push(Item);
          Item.OnPush();
          Item.CommandState = CommandState.Topped;
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
            Topped.OnOver();
            Topped.CommandState = CommandState.Overed;
            this.OnOvered.Invoke(this, Topped);
          }
          super.push(Item);
          Item.OnPush();
          Item.CommandState = CommandState.Topped;
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
            Overed.OnTop();
            Overed.CommandState = CommandState.Topped;
            this.OnTopped.Invoke(this, Overed);
          }
          return Popped;
        }
        throw Error("Empty Stack");
      }

      public Clear() {
        while (this.length > 0) {
          const Popped = super.pop();
          Popped.OnPop();
          Popped.CommandState = CommandState.Popped;
          this.OnPopped.Invoke(this, Popped);
        }
      }
      public Discard() { super.splice(0, this.length); }
    }
  }


}