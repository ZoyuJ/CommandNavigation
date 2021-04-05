namespace CommandsNavigation.CommandsNavigationx2 {
  using CommandNavigation;

  using System.Collections.Generic;

  public class CommandNavigation<T> : Stack<T> where T : class, ICommandCtrl {




  }

}
