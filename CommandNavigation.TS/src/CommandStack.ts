
import { Func2, Action2 } from "katkits/lib/Event"

export default class CommandStack<T> extends Array<T>{

  protected readonly _Detecting: Func2<T, T, boolean>;
  protected readonly _OnPushed: Action2<CommandStack<T>, T>;
  protected readonly _OnPopped: Action2<CommandStack<T>, T>;

  constructor(OnPushed: Action2<CommandStack<T>, T>, OnPopped: Action2<CommandStack<T>, T>, PopDetectingHandle: Func2<T, T, boolean>) {
    super();
    this._Detecting = PopDetectingHandle;
    this._OnPopped = OnPopped;
    this._OnPushed = OnPushed;
  }
  public Peek(): T { return this[this.length - 1]; }
  public Push(Item: T) {
    if (this.length == 0) {
      super.push(Item);
      this._OnPushed(this, Item);
    }
    else if (this._Detecting(this.Peek(), Item)) {
      this._OnPopped(this, this.Pop());
      this.Push(Item);
    }
    else {
      super.push(Item);
      this._OnPushed(this, Item);
    }
  }

  public Pop(): T {
    var Item = super.pop();
    this._OnPopped(this, Item);
    return Item;
  }

  public Clear() {
    while (this.length > 0) {
      this.Pop();
    }
  }
  public Discard() { super.splice(0, this.length); }

}