
/**
 * basic 2 ctrl methods for stack ctrl details
 * */
export interface ICommandManualCtrl {
  OnPush();
  OnPop();
}

/**
 * add other 2 ctrl methods for stack ctrl details
 * */
export interface ICommandManualCtrlx4 extends ICommandManualCtrl {
  OnOver();
  OnTop();
}

/**
 * add order in order to do check and pop automaticly
 * */
export interface ICommandCtrl extends ICommandManualCtrl {
  /**
   * (readonly property)
   * only set this property before do stack ctrl
   * */
  Order: number;
}

/**
 * add command state in order to avoid invoke ctrl method too many times
 * */
export interface ICommandCtrlx4 extends ICommandCtrl, ICommandManualCtrlx4 {
  /**
   * (readonly property)
   * never set this property manually,
   * or only set 'CommandState.Popped' to this property before do stack ctrl
   * */
  CommandState: CommandState;
}

export enum CommandState {
  Popped = 0,
  Overed,
  Topped,
}

/*
 ManualCtrl ©Ð¡ú ManualCtrlx4 ©´
            ©¸¡ú Ctrl         ©Ø¡ú Ctrlx4
      
 */