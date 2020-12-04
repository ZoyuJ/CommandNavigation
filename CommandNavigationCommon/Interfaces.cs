namespace CommandNavigation {
  public interface ICommandCtrl {
    int Order { get; set; }
    CommandState CommandState { get; set; }
    void OnPush();
    void OnPop();
  }
  public interface ICommandCtrlx4 : ICommandCtrl {
    void OnOver();
    void OnTop();
  }
  public enum CommandState {
    Popped = 0,
    Overed,
    Topped,
  }

}
