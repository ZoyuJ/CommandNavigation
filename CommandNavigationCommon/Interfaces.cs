namespace CommandNavigation {
  /// <summary>
  /// basic 2 ctrl methods for stack ctrl details
  /// </summary>
  public interface ICommandManualCtrl {
    void OnPush();
    void OnPop();
  }
  /// <summary>
  /// add order in order to do check and pop automaticly
  /// </summary>
  public interface ICommandCtrl : ICommandManualCtrl {
    /// <summary>
    /// (readonly property)
    /// only set this property before do stack ctrl
    /// </summary>
    int Order { get; set; }
  }
  /// <summary>
  /// add other 2 ctrl methods for stack ctrl details
  /// </summary>
  public interface ICommandManualCtrlx4 : ICommandManualCtrl {
    void OnOver();
    void OnTop();
  }
  /// <summary>
  /// add command state in order to avoid invoke ctrl method too many times
  /// </summary>
  public interface ICommandCtrlx4 : ICommandCtrl, ICommandManualCtrlx4 {
    /// <summary>
    /// (readonly property)
    /// never set this property manually,
    /// or only set 'CommandState.Popped' to this property before do stack ctrl
    /// </summary>
    CommandState CommandState { get; set; }
  }

  public enum CommandState {
    Popped = 0,
    Overed,
    Topped,
  }

}
/*
 ManualCtrl ┬→ ManualCtrlx4 ┐
            └→ Ctrl         ┴→ Ctrlx4
      
 */