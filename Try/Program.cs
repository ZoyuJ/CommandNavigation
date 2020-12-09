namespace Try {
  using System;

  using CommandNavigation;
  using CommandNavigation.CommandnNavigation4;
  class Program {
    static void Main(string[] args) {
      Console.WriteLine("Hello World!");

      var Navi = new CommandNavigation<ICommandCtrlx4>(10);

      //Navi.Push(new A { Name = "0-0", Order = 0 });
      //Console.WriteLine("=====");
      //Navi.Push(new A { Name = "0-1", Order = 0 });
      //Console.WriteLine("=====");
      //Navi.Push(new A { Name = "0-2", Order = 0 });
      //Console.WriteLine("=====");
      //Navi.Push(new A { Name = "1-0", Order = 10 });
      //Console.WriteLine("=====");
      //Navi.Push(new A { Name = "2-0", Order = 20 });
      //Console.WriteLine("=====");
      //Navi.Push(new A { Name = "2-1", Order = 20 });
      //Console.WriteLine("=====");
      //Navi.Pop();
      //Console.WriteLine("=====");

      Console.WriteLine(Navi.Count);
      Console.ReadKey();
    }

    //struct A : ICommandCtrlx4 {
    //  public CommandState CommandState { get; set; }
    //  public string Name;
    //  public void OnOver() {
    //    Console.WriteLine($"{Name} Over {CommandState}");
    //  }

    //  public void OnTop() {
    //    Console.WriteLine($"{Name} Top {CommandState}");
    //  }

    //  public int Order { get; set; }

    //  public void OnPush() {
    //    Console.WriteLine($"{Name} Push {CommandState}");
    //  }

    //  public void OnPop() {
    //    Console.WriteLine($"{Name} Pop {CommandState}");
    //  }
    //}

  }
}
