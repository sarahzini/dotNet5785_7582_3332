using System.Windows;

namespace PL.Call;

/// <summary>
/// Interaction logic for CallWindow.xaml
/// </summary>
public partial class CallWindow : Window
{
    // Static instance of the business logic layer
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    // Constructor for CallWindow
    public CallWindow(int CallId)
    {
        try
        {
            if (CallId == 0)
            {
                // If the CallId is 0, create a new call object
                CurrentCall = new BO.Call()
                {
                    CallId = 0,
                    TypeOfCall = BO.SystemType.None,
                    CallAddress = "",
                    CallLatitude = null,
                    CallLongitude = null,
                    Description = "",
                    BeginTime = s_bl.Admin.GetClock(),
                    Status = null,
                    MaxEndTime = s_bl.Admin.GetClock(),
                    CallAssigns = null
                };
                ButtonText = "Add";
            }
            else
            {
                // If CallId is not 0, retrieve existing call details
                CurrentCall = s_bl.Call.GetCallDetails(CallId);
                ButtonText = "Update";
            }
            InitializeComponent();
        }
        catch (BO.BLDoesNotExistException ex)
        {
            // Handle case where call does not exist
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (Exception ex)
        {
            // Handle any other exceptions
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Property to get or set the current call
    /// </summary>
    public BO.Call? CurrentCall
    {
        get { return (BO.Call?)GetValue(CurrentCallProperty); }
        set { SetValue(CurrentCallProperty, value); }
    }

    public static readonly DependencyProperty CurrentCallProperty =
        DependencyProperty.Register("CurrentCall", typeof(BO.Call), typeof(CallWindow), new PropertyMetadata(null));

    /// <summary>
    /// Property to get or set the button text
    /// </summary>
    public string ButtonText
    {
        get { return (string)GetValue(ButtonTextProperty); }
        set { SetValue(ButtonTextProperty, value); }
    }

    public static readonly DependencyProperty ButtonTextProperty =
        DependencyProperty.Register("ButtonText", typeof(string), typeof(CallWindow), new PropertyMetadata(""));

    /// <summary>
    /// Adds an observer to the call when the window is loaded
    /// </summary>
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        s_bl.Call.AddObserver(CurrentCall!.CallId, callObserver);
    }

    /// <summary>
    /// Removes the observer when the window is closed
    /// </summary>
    private void Window_Closed(object sender, EventArgs e)
    {
        s_bl.Call.RemoveObserver(CurrentCall!.CallId, callObserver);
    }

    /// <summary>
    /// Handles the add/update button click event
    /// </summary>
    private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (ButtonText == "Update")
            {
                // Update an existing call
                s_bl.Call.UpdateCallDetails(CurrentCall!);
                MessageBox.Show($"The Call with the ID number : {CurrentCall?.CallId} was successfully updated!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // Add a new call
                s_bl.Call.AddCall(CurrentCall!);
                MessageBox.Show($"The Call was successfully added!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
        }
        catch (BO.BLAlreadyExistException ex)
        {
            // Handle case where call already exists
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (BO.BLDoesNotExistException ex)
        {
            // Handle case where call does not exist
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (BO.BLFormatException ex)
        {
            // Handle case where there is a format issue
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (Exception ex)
        {
            // Handle any other exceptions
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Opens the CallAssignmentWindow when the assignments button is clicked
    /// </summary>
    private void btnAssignments_Click(object sender, RoutedEventArgs e)
    {
        new CallAssignmentWindow(CurrentCall!.CallAssigns!).ShowDialog();
    }

    private volatile bool _observerWorking = false; // Flag to avoid multiple observer calls

    /// <summary>
    /// Observer method to update call details when a change is detected
    /// </summary>
    private void callObserver()
    {
        if (!_observerWorking)
        {
            _observerWorking = true;
            _ = Dispatcher.BeginInvoke(() =>
            {
                int id = CurrentCall!.CallId;
                CurrentCall = null;
                CurrentCall = s_bl.Call.GetCallDetails(id);
                _observerWorking = false;
            });
        }
    }
}