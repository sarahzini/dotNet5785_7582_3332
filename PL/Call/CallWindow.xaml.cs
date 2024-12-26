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
    public BO.CallInList? SelectedCall { get; set; }
    string ButtonText
    {
        get => (string)GetValue(ButtonTextProperty);
        init => SetValue(ButtonTextProperty, value);
    }
    public static readonly DependencyProperty ButtonTextProperty =
        DependencyProperty.Register(nameof(ButtonText), typeof(string), typeof(CallWindow));
    
    public CallWindow(string AddOrUpdate, int id)
    {
        ButtonText = AddOrUpdate == "Add"?"Add" : "Update";
        InitializeComponent();
        if(AddOrUpdate == "Update")
        {
            CurrentCall = s_bl.Call.GetCallDetails(id);
        }
        if (AddOrUpdate == "Add")
        {
            CurrentCall = new BO.Call() 
            {
                BeginTime = DateTime.Now,
                CallAddress = "",
                Description = "",
            };
        }
    }
    private void lsvCallList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (SelectedCall != null)
            new CallWindow("uptade", SelectedCall.CallId).Show();
    }

    public BO.Call? CurrentCall
    {
        get { return (BO.Call?)GetValue(CurrentCallProperty); }
        set { SetValue(CurrentCallProperty, value); }
    }

    public static readonly DependencyProperty CurrentCallProperty =
        DependencyProperty.Register("CurrentCourse", typeof(BO.Call), typeof(CallWindow), new PropertyMetadata(null));
    private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
    {
        if (ButtonText == "Add")
        {
            try
            {

                s_bl.Call.AddCall(CurrentCall!);
                MessageBox.Show($"Call {CurrentCall?.CallId} was successfully added!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (BO.BLAlreadyExistException ex)
            {
                MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        else 
        {
            try
            {
              

                s_bl.Call.UpdateCallDetails(CurrentCall!);
                MessageBox.Show($"Student {CurrentCall?.CallId} was successfully updated!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (BO.BLDoesNotExistException ex)
            {
                MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
