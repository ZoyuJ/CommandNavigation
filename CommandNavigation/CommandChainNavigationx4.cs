namespace CommandNavigation.CommandnNavigation4 {
  using System;
  using System.Collections.Generic;
  using System.Text;

  public class CommandChainNavigationx4 : Stack<CommandChainNavigationx4>, ICommandCtrlx4 {

    public CommandState CommandState { get; set; }
    public int Order { get; set; }

    public void OnOver() {
      throw new NotImplementedException();
    }

    public void OnTop() {
      throw new NotImplementedException();
    }


    public void OnPush() {
      throw new NotImplementedException();
    }

    public void OnPop() {
      throw new NotImplementedException();
    }
  }

}
