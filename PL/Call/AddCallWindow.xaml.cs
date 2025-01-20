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
/// Interaction logic for CallWindow.xaml
/// </summary>
public partial class AddCallWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    public AddCallWindow()
    {
        
            InitializeComponent();

                CurrentCall = new BO.Call()
                {
                    CallId = 0,
                    TypeOfCall = BO.SystemType.None,
                    Description = "",
                    CallAddress = "",
                    CallLatitude = 0,
                    CallLongitude = 0,
                    BeginTime = s_bl.Admin.GetClock(),
                    MaxEndTime = null,
                    Status = BO.Statuses.Open,
                    CallAssigns = null
                };
        
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
        DependencyProperty.Register("CurrentCall", typeof(BO.Call), typeof(AddCallWindow), new PropertyMetadata(null));

    /// <summary>
    /// This method depending on the button text will either add or uptade a call.
    /// </summary>
    private void btnAdd_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Combine date and time for MaxEndTime
            CurrentCall.MaxEndTime = CombineDateAndTime(MaxEndDatePicker.SelectedDate, MaxEndTimeTextBox.Text);

           
                s_bl.Call.AddCall(CurrentCall!);
                MessageBox.Show($"The Call with the ID number : {CurrentCall?.CallId} was successfully added!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
            
        }
        catch (BO.BLAlreadyExistException ex)
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

}
