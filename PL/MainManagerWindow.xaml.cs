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

    public MainManagerWindow(int id = 0)
    {
        id = id;
        ButtonText = "Start Simulator";
        isRun = false;
        InitializeComponent();

    }

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
        DependencyProperty.Register("ButtonText", typeof(string), typeof(MainManagerWindow), new PropertyMetadata(0));
    public int Interval
    {
        get { return (int)GetValue(IntervalProperty); }
        set { SetValue(IntervalProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Interval.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty IntervalProperty =
        DependencyProperty.Register("Interval", typeof(int), typeof(MainManagerWindow), new PropertyMetadata(0));
    public bool isRun
    {
        get { return (bool)GetValue(isRunProperty); }
        set { SetValue(isRunProperty, value); }
    }

    // Using a DependencyProperty as the backing store for isRun.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty isRunProperty =
        DependencyProperty.Register("isRun", typeof(bool), typeof(MainManagerWindow), new PropertyMetadata(0));



    private void btnSimulator_Click(object sender, RoutedEventArgs e)
    {
        if (ButtonText == "Start Simulator")
        {
            ButtonText = "Stop Simulator";
            isRun = true;
            s_bl.Admin.StartSimulator(Interval);
        }
        else
        {
            ButtonText = "Start Simulator";
            isRun = false;
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
    }

    private void Window_Closed(object sender, EventArgs e)
    {
        s_bl.Admin.RemoveClockObserver(clockObserver);
        s_bl.Admin.RemoveConfigObserver(configObserver);
        if (!isRun)
            s_bl.Admin.StopSimulator();
    }

    public int id { get; set; }

    private void btnVolunteers_Click(object sender, RoutedEventArgs e)
    { new VolunteerInListWindow(id).Show(); }

    private void btnCalls_Click(object sender, RoutedEventArgs e)
    { new CallInListWindow(id).Show(); }

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

            Mouse.OverrideCursor = null;
        }
    }


}