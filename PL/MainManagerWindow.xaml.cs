using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PL.Volunteer;
using PL.Call;

namespace PL;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainManagerWindow : Window
{
    /// To gain access to the BL layer, we need to use the Factory class.
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    public MainManagerWindow(int Id = 0)
    {
        id = Id;
        Interval = 15;
        ButtonText = "Start Simulator";
        isNotRun = true;
        CallCounts = s_bl.Call.TypeOfCallCounts();
        InitializeComponent();
      
    }
    public int[] CallCounts
    {
        get { return (int[])GetValue(CallCountsProperty); }
        set { SetValue(CallCountsProperty, value); }
    }

    // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CallCountsProperty =
        DependencyProperty.Register("CallCounts", typeof(int[]), typeof(MainManagerWindow));
    public TimeSpan RiskRange
    {
        get { return (TimeSpan)GetValue(RiskRangeProperty); }
        set { SetValue(RiskRangeProperty, value); }
    }
    public static readonly DependencyProperty RiskRangeProperty =
        DependencyProperty.Register("RiskRange", typeof(TimeSpan), typeof(MainManagerWindow));

    public DateTime CurrentTime
    {
        get { return (DateTime)GetValue(CurrentTimeProperty); }
        set { SetValue(CurrentTimeProperty, value); }
    }
    public static readonly DependencyProperty CurrentTimeProperty =
        DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainManagerWindow));
    public string ButtonText
    {
        get { return (string)GetValue(ButtonTextProperty); }
        set { SetValue(ButtonTextProperty, value); }
    }

    // Using a DependencyProperty as the backing store for ButtonText.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ButtonTextProperty =
        DependencyProperty.Register("ButtonText", typeof(string), typeof(MainManagerWindow), new PropertyMetadata(""));
    public int Interval
    {
        get { return (int)GetValue(IntervalProperty); }
        set { SetValue(IntervalProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Interval.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty IntervalProperty =
        DependencyProperty.Register("Interval", typeof(int), typeof(MainManagerWindow), new PropertyMetadata(0));
    public bool isNotRun
    {
        get { return (bool)GetValue(isNotRunProperty); }
        set { SetValue(isNotRunProperty, value); }
    }

    // Using a DependencyProperty as the backing store for isRun.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty isNotRunProperty =
        DependencyProperty.Register("isRun", typeof(bool), typeof(MainManagerWindow), new PropertyMetadata(false));



    private void btnSimulator_Click(object sender, RoutedEventArgs e)
    {
        if (ButtonText == "Start Simulator")
        {
            ButtonText = "Stop Simulator";
            isNotRun = false;
            s_bl.Admin.StartSimulator(Interval);
        }
        else
        {
            ButtonText = "Start Simulator";
            isNotRun = true;
            s_bl.Admin.StopSimulator();

        }
    }

    private volatile bool _clockObserverWorking = false;
    private void clockObserver() //stage 5
    {
        if (!_clockObserverWorking)
        {
            _clockObserverWorking = true;
            _ = Dispatcher.BeginInvoke(() =>
            {
                CurrentTime = s_bl.Admin.GetClock();
                _clockObserverWorking = false;
            });  //stage 7 (for multithreading)
        }

    }

    private volatile bool _callObserverWorking = false;

    private void callListObserver()
    {
        if (!_callObserverWorking)
        {
            _callObserverWorking = true;
            _ = Dispatcher.BeginInvoke(() =>
            {
                CallCounts = s_bl.Call.TypeOfCallCounts();
                _callObserverWorking = false;
            });
        }
    }

    private volatile bool _configObserverWorking = false; //stage 7
    private void configObserver() //stage 5
    {
        if (!_configObserverWorking)
        {
            _configObserverWorking = true;
            _ = Dispatcher.BeginInvoke(() =>
            { //stage 7 (for multithreading)
                RiskRange = s_bl.Admin.GetRiskRange();
                _configObserverWorking = false;
            });  //stage 7 (for multithreading)
        }

    }

    private void btnAddOneMinute_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.ForwardClock(BO.TimeUnit.Minute);
    }

    private void btnAddOneHour_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.ForwardClock(BO.TimeUnit.Hour);
    }

    private void btnAddOneDay_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.ForwardClock(BO.TimeUnit.Day);
    }

    private void btnAddOneMonth_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.ForwardClock(BO.TimeUnit.Month);
    }

    private void btnAddOneYear_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.ForwardClock(BO.TimeUnit.Year);
    }
    private void btnSetRR_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.SetRiskRange(RiskRange);
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        CurrentTime = s_bl.Admin.GetClock();
        RiskRange = s_bl.Admin.GetRiskRange();
        s_bl.Admin.AddClockObserver(clockObserver);
        s_bl.Admin.AddConfigObserver(configObserver);
        s_bl.Call.AddObserver(callListObserver);

    }

    private void Window_Closed(object sender, EventArgs e)
    { 
        if (!isNotRun)
            s_bl.Admin.StopSimulator();
        s_bl.Admin.RemoveClockObserver(clockObserver);
        s_bl.Admin.RemoveConfigObserver(configObserver);
        s_bl.Call.RemoveObserver(callListObserver);
    }
    public int id { get; set; }

    private void btnVolunteers_Click(object sender, RoutedEventArgs e)
    { new VolunteerInListWindow(id).Show(); }

    private void btnCalls_Click(object sender, RoutedEventArgs e)
    { new CallInListWindow(id,null).Show(); }
    private void btnOpenCalls_Click(object sender, RoutedEventArgs e)
    {
        new CallInListWindow(id, BO.Statuses.Open).Show();
    }

    private void btnOpenCallsInRisk_Click(object sender, RoutedEventArgs e)
    {
        new CallInListWindow(id, BO.Statuses.OpenToRisk).Show();
    }

    private void btnInActionCalls_Click(object sender, RoutedEventArgs e)
    {
        new CallInListWindow(id, BO.Statuses.InAction).Show();
    }

    private void btnInActionCallsInRisk_Click(object sender, RoutedEventArgs e)
    {
        new CallInListWindow(id, BO.Statuses.InActionToRisk).Show();
    }

    private void btnClosedCalls_Click(object sender, RoutedEventArgs e)
    {
        new CallInListWindow(id, BO.Statuses.Closed).Show();
    }

    private void btnExpiredCalls_Click(object sender, RoutedEventArgs e)
    {
        new CallInListWindow(id, BO.Statuses.Expired).Show();
    }




    private void btnInitialization_Click(object sender, RoutedEventArgs e)
    {
        MessageBoxResult confirmation = MessageBox.Show("You are going to Initialize", "Init Confirmation",
                                                    MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (confirmation == MessageBoxResult.Yes)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window != this)
                {
                    window.Close();
                }
            }

            Mouse.OverrideCursor = Cursors.Wait;
            s_bl.Admin.InitializeDB();
            CallCounts = s_bl.Call.TypeOfCallCounts();
            Mouse.OverrideCursor = null;
        }
    }
    private void btnReset_Click(object sender, RoutedEventArgs e)
    {
        MessageBoxResult confirmation = MessageBox.Show("You are going to Reset !", "Reset Confirmation",
                                                    MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (confirmation == MessageBoxResult.Yes)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window != this)
                {
                    window.Close();
                }
            }

            Mouse.OverrideCursor = Cursors.Wait;
            s_bl.Admin.ResetDB();
            CallCounts = s_bl.Call.TypeOfCallCounts();
            Mouse.OverrideCursor = null;
        }
    }


}