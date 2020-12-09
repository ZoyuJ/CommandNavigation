namespace CommandNavigation.WPF {
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Threading;

  using CommandNavigation.CommandnNavigation4;

  public abstract class CommandNavigationWindowBase : Window, IUICommandCtrlx4 {
    public abstract override void OnApplyTemplate();
    protected abstract void OnTimer(object sender, EventArgs e);
    public CommandNavigationWindowBase() : base() {
      _Navigation = new CommandNavigation<IUICommandCtrlx4>(5);
      _Timer = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Normal, OnTimer, Dispatcher);
    }

    protected readonly CommandNavigation<IUICommandCtrlx4> _Navigation;
    protected readonly DispatcherTimer _Timer;

    public CommandNavigation<IUICommandCtrlx4> Navigation { get => _Navigation; }

    public event CommandPushedHandler OnCommandPushed;
    public event CommandPoppedHandler OnCommandPopped;
    public event CommandToppedHandler OnCommandTopped;
    public event CommandOveredHandler OnCommandOvered;

    public event VisibilityChangedHandler OnVisibilityChanged;

    public Visibility VisibilityState { get => (Visibility)GetValue(VisibilityStateProperty); set => SetValue(VisibilityStateProperty, value); }
    public static readonly DependencyProperty VisibilityStateProperty
      = DependencyProperty.Register("VisibilityState", typeof(Visibility), typeof(CommandNavigationWindowBase), new PropertyMetadata(Visibility.Hidden, _OnVisobilityPropertyChanged));
    private static void _OnVisobilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      var State = (Visibility)(e.NewValue);
      var Target = ((CommandNavigationWindowBase)(d));
      if (Target.Visibility != State) {
        Target.Visibility = State;
        Target.OnVisibilityChanged?.Invoke(d, new VisibilityChangedEventArgs((Visibility)(e.OldValue), State));
      }
    }
    public int TimerInterval { get => (int)GetValue(TimerIntervalProperty); set => SetValue(TimerIntervalProperty, value); }
    public static readonly DependencyProperty TimerIntervalProperty
      = DependencyProperty.Register("TimerInterval", typeof(int), typeof(CommandNavigationWindowBase), new PropertyMetadata(1000, _OnIntervalPropertyChanged));
    private static void _OnIntervalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      var MS = (int)(e.NewValue);
      var Target = ((CommandNavigationWindowBase)(d));
      if (MS <= 0)
        Target._Timer.Stop();
      else
        Target._Timer.Interval = TimeSpan.FromMilliseconds(MS);
    }

    public CommandState CommandState { get; set; }
    public int Order { get; set; }
    public virtual void OnOver() {
      OnCommandOvered?.Invoke(this, new CommandOveredEventArgs());
    }
    public virtual void OnTop() {
      OnCommandTopped?.Invoke(this, new CommandToppedEventArgs());
    }
    public virtual void OnPush() {
      this.Show();
      _Timer?.Start();
      OnCommandPushed?.Invoke(this, new CommandPushedEventArgs());
    }
    public virtual void OnPop() {
      this.Close();
      _Timer?.Stop();
      OnCommandPopped?.Invoke(this, new CommandPoppedEventArgs());
    }

  }
  public abstract class CommandNavigationUserControl : UserControl, IUICommandCtrlx4 {
    public abstract override void OnApplyTemplate();
    protected abstract void OnTimer(object sender, EventArgs e);
    public CommandNavigationUserControl() : base() {
      _Navigation = new CommandNavigation<IUICommandCtrlx4>(5);
      _Timer = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Normal, OnTimer, Dispatcher);
    }

    protected readonly CommandNavigation<IUICommandCtrlx4> _Navigation;
    protected readonly DispatcherTimer _Timer;

    public CommandNavigation<IUICommandCtrlx4> Navigation { get => _Navigation; }

    public event CommandPushedHandler OnCommandPushed;
    public event CommandPoppedHandler OnCommandPopped;
    public event CommandToppedHandler OnCommandTopped;
    public event CommandOveredHandler OnCommandOvered;

    public event VisibilityChangedHandler OnVisibilityChanged;

    public Visibility VisibilityState { get => (Visibility)GetValue(VisibilityStateProperty); set => SetValue(VisibilityStateProperty, value); }
    public static readonly DependencyProperty VisibilityStateProperty
      = DependencyProperty.Register("VisibilityState", typeof(Visibility), typeof(CommandNavigationUserControl), new PropertyMetadata(Visibility.Hidden, _OnVisobilityPropertyChanged));
    private static void _OnVisobilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      var State = (Visibility)(e.NewValue);
      var Target = ((CommandNavigationUserControl)(d));
      if (Target.Visibility != State) {
        Target.Visibility = State;
        Target.OnVisibilityChanged?.Invoke(d, new VisibilityChangedEventArgs((Visibility)(e.OldValue), State));
      }
    }
    public int TimerInterval { get => (int)GetValue(TimerIntervalProperty); set => SetValue(TimerIntervalProperty, value); }
    public static readonly DependencyProperty TimerIntervalProperty
      = DependencyProperty.Register("TimerInterval", typeof(int), typeof(CommandNavigationUserControl), new PropertyMetadata(1000, _OnIntervalPropertyChanged));
    private static void _OnIntervalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      var MS = (int)(e.NewValue);
      var Target = ((CommandNavigationUserControl)(d));
      if (MS <= 0)
        Target._Timer.Stop();
      else
        Target._Timer.Interval = TimeSpan.FromMilliseconds(MS);
    }

    public CommandState CommandState { get; set; }
    public int Order { get; set; }
    public virtual void OnOver() {
      this.Visibility = Visibility.Hidden;
      _Timer?.Stop();
      OnCommandOvered?.Invoke(this, new CommandOveredEventArgs());
    }
    public virtual void OnTop() {
      this.Visibility = Visibility.Visible;
      _Timer?.Start();
      OnCommandTopped?.Invoke(this, new CommandToppedEventArgs());
    }
    public virtual void OnPush() {
      this.Visibility = Visibility.Visible;
      _Timer?.Start();
      OnCommandPushed?.Invoke(this, new CommandPushedEventArgs());
    }
    public virtual void OnPop() {
      this.Visibility = Visibility.Collapsed;
      _Timer?.Stop();
      OnCommandPopped?.Invoke(this, new CommandPoppedEventArgs());
    }
  }

  public interface IUICommandCtrlx4 : ICommandCtrlx4 {
    event CommandPushedHandler OnCommandPushed;
    event CommandPoppedHandler OnCommandPopped;
    event CommandToppedHandler OnCommandTopped;
    event CommandOveredHandler OnCommandOvered;
  }

  public delegate void CommandPushedHandler(IUICommandCtrlx4 sender, CommandPushedEventArgs args);
  public delegate void CommandPoppedHandler(IUICommandCtrlx4 sender, CommandPoppedEventArgs args);
  public delegate void CommandToppedHandler(IUICommandCtrlx4 sender, CommandToppedEventArgs args);
  public delegate void CommandOveredHandler(IUICommandCtrlx4 sender, CommandOveredEventArgs args);

  public class CommandPushedEventArgs : EventArgs { }
  public class CommandPoppedEventArgs : EventArgs { }
  public class CommandToppedEventArgs : EventArgs { }
  public class CommandOveredEventArgs : EventArgs { }

  public delegate void VisibilityChangedHandler(object sender, VisibilityChangedEventArgs args);
  public class VisibilityChangedEventArgs : EventArgs {
    public readonly Visibility LastVisibility, CurrentVisibility;
    public VisibilityChangedEventArgs(Visibility Last, Visibility Current) {
      LastVisibility = Last;
      CurrentVisibility = Current;
    }
  }

}
