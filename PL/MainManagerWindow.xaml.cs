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

    public MainManagerWindow(int id=0)
    {
        InitializeComponent();
        id = id;
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

    private void clockObserver() => CurrentTime = s_bl.Admin.GetClock();
    private void configObserver() => RiskRange = s_bl.Admin.GetRiskRange();

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
    }

    public int id { get; set; };

    private void btnVolunteers_Click(object sender, RoutedEventArgs e)
    { new VolunteerInListWindow( id).Show(); }

    private void btnCalls_Click(object sender, RoutedEventArgs e)
    { new CallInListWindow().Show(); }

    private void btnInitialization_Click(object sender, RoutedEventArgs e)
    {
        //helped by IA
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
        //helped by IA
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