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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PL.Call;

/// <summary>
/// Interaction logic for CallWindow.xaml
/// </summary>
public partial class CallWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    
    public CallWindow(string AddOrUpdate, int id)
    {
        try
        {
            ButtonText = AddOrUpdate == "Add" ? "Add" : "Update";
            InitializeComponent();

            if (ButtonText == "Add")
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
                    CallAssigns =null
                };
            else
                CurrentCall = s_bl.Call.GetCallDetails(id);
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

    string ButtonText
    {
        get => (string)GetValue(ButtonTextProperty);
        init => SetValue(ButtonTextProperty, value);
    }
    public static readonly DependencyProperty ButtonTextProperty =
        DependencyProperty.Register(nameof(ButtonText), typeof(string), typeof(CallWindow));

    public BO.Call? CurrentCall
    {
        get { return (BO.Call?)GetValue(CurrentCallProperty); }
        set { SetValue(CurrentCallProperty, value); }
    }

    public static readonly DependencyProperty CurrentCallProperty =
        DependencyProperty.Register("CurrentCourse", typeof(BO.Call), typeof(CallWindow), new PropertyMetadata(null));
    private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (ButtonText == "Add")
            {
                s_bl.Call.AddCall(CurrentCall!);
                MessageBox.Show($"The volunteer {CurrentCall?.CallId} was successfully added!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                s_bl.Call.UpdateCallDetails(CurrentCall!);
                MessageBox.Show($"The volunteer {CurrentCall?.CallId} was successfully updated!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (BO.BLAlreadyExistException ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
    private void callObserver()
    {
        int id = CurrentCall!.CallId;
        CurrentCall = null;
        CurrentCall = s_bl.Call.GetCallDetails(id);

    }
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        if (CurrentCall!.CallId != 0)
            s_bl.Call.AddObserver(CurrentCall!.CallId, callObserver);
    }
    private void Window_Closed(object sender, EventArgs e)
    {
        s_bl.Call.RemoveObserver(CurrentCall!.CallId, callObserver);
    }
}
