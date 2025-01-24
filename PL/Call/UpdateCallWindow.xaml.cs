using PL.Volunteer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PL.Call;

/// <summary>
/// Interaction logic for UpdateCallWindow.xaml
/// </summary>
public partial class UpdateCallWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public UpdateCallWindow(int CallId)
    {
        try
        {

            CurrentCall = s_bl.Call.GetCallDetails(CallId);
            InitializeComponent();

        }
        catch (BO.BLDoesNotExistException ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    /// <summary>
    /// This method gets the current call and set 
    /// </summary>
    public BO.Call? CurrentCall
    {
        get { return (BO.Call?)GetValue(CurrentCallProperty); }
        set { SetValue(CurrentCallProperty, value); }
    }

    public static readonly DependencyProperty CurrentCallProperty =
        DependencyProperty.Register("CurrentCall", typeof(BO.Call), typeof(UpdateCallWindow), new PropertyMetadata(null));

    /// <summary>
    /// This method depending on the button text will either add or uptade a call.
    /// </summary>
    private void btnUpdate_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Combine date and time for MaxEndTime
            CurrentCall.MaxEndTime = CombineDateAndTime(MaxEndDatePicker.SelectedDate, MaxEndTimeTextBox.Text);

            
                s_bl.Call.UpdateCallDetails(CurrentCall!);
                MessageBox.Show($"The Call with the ID number : {CurrentCall?.CallId} was successfully updated!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            
        }
        catch (BO.BLDoesNotExistException ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (BO.BLFormatException ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void btnAssignments_Click(object sender, RoutedEventArgs e) { 

         new CallAssignmentWindow(CurrentCall!.CallAssigns!).ShowDialog();
    }
        

    /// <summary>
    /// This method combines a date and time to create a DateTime object.
    /// </summary>
    private DateTime CombineDateAndTime(DateTime? date, string time)
    {
        if (date == null || string.IsNullOrEmpty(time))
            throw new ArgumentException("Date or time is invalid");

        var timeParts = time.Split(':');
        if (timeParts.Length != 3)
            throw new ArgumentException("Time format is invalid");

        int hours = int.Parse(timeParts[0]);
        int minutes = int.Parse(timeParts[1]);
        int seconds = int.Parse(timeParts[2]);

        return new DateTime(date.Value.Year, date.Value.Month, date.Value.Day, hours, minutes, seconds);
    }

    private volatile bool _observerWorking = false; //stage 7

    /// <summary>
    /// This method calls the observer.
    /// </summary>

    private void callObserver() //stage 7
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
   

    /// <summary>
    /// This method adds an observer to the call.
    /// </summary>
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        s_bl.Call.AddObserver(CurrentCall!.CallId, callObserver);
    }

    /// <summary>
    /// This method removes the observer from the call.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Window_Closed(object sender, EventArgs e)
    {
        s_bl.Call.RemoveObserver(CurrentCall!.CallId, callObserver);
    }
}
