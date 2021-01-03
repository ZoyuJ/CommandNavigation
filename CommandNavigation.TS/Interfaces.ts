
/// <reference path="main.ts" />
/// <reference path="CommandNavigationx2.ts"/>
/// <reference path="CommandNavigationx4.ts"/>

 namespace CommandNavigation {

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


}