namespace CommandNavigation {
  public interface ICommandCtrl {
    int Order { get; set; }
    void OnPush();
    void OnPop();
  }
  public interface ICommandCtrlx4 : ICommandCtrl {
    CommandState CommandState { get; set; }
    void OnOver();
    void OnTop();
  }

  //public interface ICommandAuto {
  //  void OnPush();
  //  void OnPop();
  //}
  //public interface ICommandAutox4 : ICommandAuto {
  //  void OnOver();
  //  void OnTop();
  //}


  public enum CommandState {
    Popped = 0,
    Overed,
    Topped,
  }

}
