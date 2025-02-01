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

public partial class MainManagerWindow : Window
{
    /// <summary>
    /// Static reference to the business logic layer through the Factory class.
    /// </summary>
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    /// <summary>
    /// Constructor for the main window. Initializes the window with default values.
    /// </summary>
    public MainManagerWindow(int Id = 0)
    {
        id = Id;
        Interval = 15; // Default interval value for the simulator
        ButtonText = "Start Simulator"; // Default button text
        isNotRun = true; // Flag to indicate if the simulator is running
        CallCounts = s_bl.Call.TypeOfCallCounts(); // Fetch the call counts from BL
        InitializeComponent(); // Initialize the window components
    }

    /// <summary>
    /// Gets or sets the manager ID.
    /// </summary>
    public int id { get; set; }

    /// <summary>
    /// Gets or sets the counts of various types of calls.
    /// </summary>
    public int[] CallCounts
    {
        get { return (int[])GetValue(CallCountsProperty); }
        set { SetValue(CallCountsProperty, value); }
    }

    /// <summary>
    /// Dependency property for CallCounts.
    /// </summary>
    public static readonly DependencyProperty CallCountsProperty =
        DependencyProperty.Register("CallCounts", typeof(int[]), typeof(MainManagerWindow));

    /// <summary>
    /// Gets or sets the risk range in the system.
    /// </summary>
    public TimeSpan RiskRange
    {
        get { return (TimeSpan)GetValue(RiskRangeProperty); }
        set { SetValue(RiskRangeProperty, value); }
    }

    /// <summary>
    /// Dependency property for RiskRange.
    /// </summary>
    public static readonly DependencyProperty RiskRangeProperty =
        DependencyProperty.Register("RiskRange", typeof(TimeSpan), typeof(MainManagerWindow));

    /// <summary>
    /// Gets or sets the current system time.
    /// </summary>
    public DateTime CurrentTime
    {
        get { return (DateTime)GetValue(CurrentTimeProperty); }
        set { SetValue(CurrentTimeProperty, value); }
    }

    /// <summary>
    /// Dependency property for CurrentTime.
    /// </summary>
    public static readonly DependencyProperty CurrentTimeProperty =
        DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainManagerWindow));

    /// <summary>
    /// Gets or sets the text for the simulator button.
    /// </summary>
    public string ButtonText
    {
        get { return (string)GetValue(ButtonTextProperty); }
        set { SetValue(ButtonTextProperty, value); }
    }

    /// <summary>
    /// Dependency property for ButtonText.
    /// </summary>
    public static readonly DependencyProperty ButtonTextProperty =
        DependencyProperty.Register("ButtonText", typeof(string), typeof(MainManagerWindow), new PropertyMetadata(""));

    /// <summary>
    /// Gets or sets the interval for the simulator.
    /// </summary>
    public int Interval
    {
        get { return (int)GetValue(IntervalProperty); }
        set { SetValue(IntervalProperty, value); }
    }

    /// <summary>
    /// Dependency property for Interval.
    /// </summary>
    public static readonly DependencyProperty IntervalProperty =
        DependencyProperty.Register("Interval", typeof(int), typeof(MainManagerWindow), new PropertyMetadata(0));

    /// <summary>
    /// Gets or sets the flag indicating whether the simulator is running.
    /// </summary>
    public bool isNotRun
    {
        get { return (bool)GetValue(isNotRunProperty); }
        set { SetValue(isNotRunProperty, value); }
    }

    /// <summary>
    /// Dependency property for isNotRun.
    /// </summary>
    public static readonly DependencyProperty isNotRunProperty =
        DependencyProperty.Register("isNotRun", typeof(bool), typeof(MainManagerWindow), new PropertyMetadata(true));

    /// <summary>
    /// Helper method to update the current system time on the UI.
    /// </summary>
    private volatile bool _clockObserverWorking = false;

    /// <summary>
    /// Observes the clock and updates the current time in the window.
    /// </summary>
    private void clockObserver()
    {
        if (!_clockObserverWorking)
        {
            _clockObserverWorking = true;
            _ = Dispatcher.BeginInvoke(() =>
            {
                CurrentTime = s_bl.Admin.GetClock(); // Fetch the current time from BL
                _clockObserverWorking = false;
            });
        }
    }

    /// <summary>
    /// Helper method to update the call counts on the UI.
    /// </summary>
    private volatile bool _callObserverWorking = false;

    /// <summary>
    /// Observes the call list and updates the call counts in the window.
    /// </summary>
    private void callListObserver()
    {
        if (!_callObserverWorking)
        {
            _callObserverWorking = true;
            _ = Dispatcher.BeginInvoke(() =>
            {
                CallCounts = s_bl.Call.TypeOfCallCounts(); // Fetch updated call counts
                _callObserverWorking = false;
            });
        }
    }

    /// <summary>
    /// Helper method to update the configuration settings (Risk Range).
    /// </summary>
    private volatile bool _configObserverWorking = false;

    /// <summary>
    /// Observes the configuration and updates the risk range in the window.
    /// </summary>
    private void configObserver()
    {
        if (!_configObserverWorking)
        {
            _configObserverWorking = true;
            _ = Dispatcher.BeginInvoke(() =>
            {
                RiskRange = s_bl.Admin.GetRiskRange(); // Fetch the risk range from BL
                _configObserverWorking = false;
            });
        }
    }

    /// <summary>
    /// Button click event to add one minute to the system clock.
    /// </summary>
    private void btnAddOneMinute_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.ForwardClock(BO.TimeUnit.Minute); // Advance the clock by one minute
    }

    /// <summary>
    /// Button click event to add one hour to the system clock.
    /// </summary>
    private void btnAddOneHour_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.ForwardClock(BO.TimeUnit.Hour); // Advance the clock by one hour
    }

    /// <summary>
    /// Button click event to add one day to the system clock.
    /// </summary>
    private void btnAddOneDay_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.ForwardClock(BO.TimeUnit.Day); // Advance the clock by one day
    }

    /// <summary>
    /// Button click event to add one month to the system clock.
    /// </summary>
    private void btnAddOneMonth_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.ForwardClock(BO.TimeUnit.Month); // Advance the clock by one month
    }

    /// <summary>
    /// Button click event to add one year to the system clock.
    /// </summary>
    private void btnAddOneYear_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.ForwardClock(BO.TimeUnit.Year); // Advance the clock by one year
    }

    /// <summary>
    /// Button click event to set the risk range for the system.
    /// </summary>
    private void btnSetRR_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.SetRiskRange(RiskRange); // Set the risk range in the system
    }

    /// <summary>
    /// Window loaded event to initialize data and set up observers.
    /// </summary>
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        CurrentTime = s_bl.Admin.GetClock(); // Initialize current time
        RiskRange = s_bl.Admin.GetRiskRange(); // Initialize risk range
        s_bl.Admin.AddClockObserver(clockObserver); // Add observer for clock updates
        s_bl.Admin.AddConfigObserver(configObserver); // Add observer for config updates
        s_bl.Call.AddObserver(callListObserver); // Add observer for call updates
    }

    /// <summary>
    /// Window closed event to clean up observers and stop the simulator if necessary.
    /// </summary>
    private void Window_Closed(object sender, EventArgs e)
    {
        if (!isNotRun)
            s_bl.Admin.StopSimulator(); // Stop the simulator if it was running
        s_bl.Admin.RemoveClockObserver(clockObserver); // Remove clock observer
        s_bl.Admin.RemoveConfigObserver(configObserver); // Remove config observer
        s_bl.Call.RemoveObserver(callListObserver); // Remove call observer
    }

    /// <summary>
    /// Button click event to open the volunteer list window.
    /// </summary>
    private void btnVolunteers_Click(object sender, RoutedEventArgs e)
    { new VolunteerInListWindow(id).Show(); }

    /// <summary>
    /// Button click event to open the general call list window.
    /// </summary>
    private void btnCalls_Click(object sender, RoutedEventArgs e)
    { new CallInListWindow(id, null).Show(); }

    /// <summary>
    /// Button click event to open the open calls list window.
    /// </summary>
    private void btnOpenCalls_Click(object sender, RoutedEventArgs e)
    {
        new CallInListWindow(id, BO.Statuses.Open).Show(); // Open calls only
    }

    /// <summary>
    /// Button click event to open the open calls in risk list window.
    /// </summary>
    private void btnOpenCallsInRisk_Click(object sender, RoutedEventArgs e)
    {
        new CallInListWindow(id, BO.Statuses.OpenToRisk).Show(); // Open calls in risk only
    }

    /// <summary>
    /// Button click event to open the in-action calls list window.
    /// </summary>
    private void btnInActionCalls_Click(object sender, RoutedEventArgs e)
    {
        new CallInListWindow(id, BO.Statuses.InAction).Show(); // In action calls only
    }

    /// <summary>
    /// Button click event to open the in-action calls in risk list window.
    /// </summary>
    private void btnInActionCallsInRisk_Click(object sender, RoutedEventArgs e)
    {
        new CallInListWindow(id, BO.Statuses.InActionToRisk).Show(); // In action calls in risk only
    }

    /// <summary>
    /// Button click event to open the closed calls list window.
    /// </summary>
    private void btnClosedCalls_Click(object sender, RoutedEventArgs e)
    {
        new CallInListWindow(id, BO.Statuses.Closed).Show(); // Closed calls only
    }

    /// <summary>
    /// Button click event to open the expired calls list window.
    /// </summary>
    private void btnExpiredCalls_Click(object sender, RoutedEventArgs e)
    {
        new CallInListWindow(id, BO.Statuses.Expired).Show(); // Expired calls only
    }

    /// <summary>
    /// Button click event to start or stop the simulator.
    /// </summary>
    private void btnSimulator_Click(object sender, RoutedEventArgs e)
    {
        if (ButtonText == "Start Simulator")
        {
            ButtonText = "Stop Simulator"; // Change button text when starting
            isNotRun = false; // Set flag indicating simulator is running
            s_bl.Admin.StartSimulator(Interval); // Start the simulator
        }
        else
        {
            ButtonText = "Start Simulator"; // Change button text when stopping
            isNotRun = true; // Set flag indicating simulator is stopped
            s_bl.Admin.StopSimulator(); // Stop the simulator
        }
    }

    /// <summary>
    /// Button click event to initialize the database with confirmation.
    /// </summary>
    private void btnInitialization_Click(object sender, RoutedEventArgs e)
    {
        MessageBoxResult confirmation = MessageBox.Show("You are going to Initialize !", "Init Confirmation",
                                                    MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (confirmation == MessageBoxResult.Yes)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window != this)
                {
                    window.Close(); // Close all other windows
                }
            }

            Mouse.OverrideCursor = Cursors.Wait; // Show wait cursor while initializing
            s_bl.Admin.InitializeDB(); // Initialize the database
            Mouse.OverrideCursor = null; // Restore the cursor
        }
    }

    /// <summary>
    /// Button click event to reset the database with confirmation.
    /// </summary>
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
                    window.Close(); // Close all other windows
                }
            }

            Mouse.OverrideCursor = Cursors.Wait; // Show wait cursor while resetting
            s_bl.Admin.ResetDB(); // Reset the database
            Mouse.OverrideCursor = null; // Restore the cursor
        }
    }
}
