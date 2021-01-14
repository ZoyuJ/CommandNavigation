
import { Func2, Event2 } from "./../KatKits/KatKits.TS/Event";

export default class CommandStack<T> extends Array<T>{
  public readonly OnPushed: Event2<CommandStack<T>, T> = new Event2<CommandStack<T>, T>();
  public readonly OnPopped: Event2<CommandStack<T>, T> = new Event2<CommandStack<T>, T>();

  protected readonly ComparerDelegate: Func2<T, T, boolean>;

  constructor(Comparer: Func2<T, T, boolean>) {
    super();
    this.ComparerDelegate = Comparer;
  }
  public Peek(): T { return this[this.length - 1]; }
  public Push(Item: T): void {
    if (this.length == 0) {
      super.push(Item);
      this.OnPushed.Invoke(this, Item);
    }
    else if (this.ComparerDelegate(this.Peek(), Item)) {
      this.OnPopped.Invoke(this, super.pop());
      this.Push(Item);
    }
    else {
      super.push(Item);
      this.OnPushed.Invoke(this, Item);
    }
  }

  public Pop(): T {
    var Item = super.pop();
    this.OnPopped.Invoke(this, Item);
    return Item;
  }

  public Clear() {
    while (this.length > 0) {
      this.Pop();
    }
  }
  public Discard() { super.splice(0, this.length); }

}
