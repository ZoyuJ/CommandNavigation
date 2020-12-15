export interface ICommandCtrl {
  Order: number;
  OnPush();
  OnPop();
}

export interface ICommandCtrlx4 extends ICommandCtrl {
  CommandState: CommandState;
  OnOver();
  OnTop();
}

export enum CommandState {
  Popped = 0,
  Overed,
  Topped,
}

