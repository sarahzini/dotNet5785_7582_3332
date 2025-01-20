using PL.Call;
using PL.Volunteer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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

namespace PL;

/// <summary>
/// Interaction logic for MainVolunteerWindow.xaml
/// </summary>
public partial class MainVolunteerWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public MainVolunteerWindow(int id)
    {
        try
        {
            InitializeComponent();
            CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(id);
            if (CurrentVolunteer.CurrentCall != null)
                ButtonText = "Current Call Details";
            else
                ButtonText = "Assignment to a call";
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

    public BO.Volunteer CurrentVolunteer
    {
        get { return (BO.Volunteer)GetValue(CurrentVolunteerProperty); }
        set { SetValue(CurrentVolunteerProperty, value); }
    }

    /// <summary>
    /// This method returns the value of the Current Volunteer
    /// </summary>
    public static readonly DependencyProperty CurrentVolunteerProperty =
        DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(MainVolunteerWindow), new PropertyMetadata(null));

    public string ButtonText
    {
        get => (string)GetValue(ButtonTextProperty);
        set => SetValue(ButtonTextProperty, value);
    }

    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty ButtonTextProperty =
        DependencyProperty.Register("ButtonText", typeof(string), typeof(MainVolunteerWindow), new PropertyMetadata(null));

    /// <summary>
    /// This method calls the volunteer observer.
    /// </summary>
    private void volunteerObserver()
    {
        int id = CurrentVolunteer!.VolunteerId;
        CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(id);
        if (CurrentVolunteer.CurrentCall != null)
            ButtonText = "Current Call Details";
        else
            ButtonText = "Assignment to a call";

    }

    /// <summary>
    /// This method loads the window.
    /// </summary>
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      s_bl.Volunteer.AddObserver(CurrentVolunteer!.VolunteerId, volunteerObserver);
    }

    /// <summary>
    /// This method closes the window.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>

    private void Window_Closed(object sender, EventArgs e)
    {
        s_bl.Volunteer.RemoveObserver(CurrentVolunteer!.VolunteerId, volunteerObserver);
    }
    private void btnHistoric_Click(object sender, RoutedEventArgs e)
    {
        new HistoricWindow(CurrentVolunteer!.VolunteerId).ShowDialog(); 
    }
    private void btnAssignment_Click(object sender, RoutedEventArgs e)
    {
        if (ButtonText == "Assignment to a call")
            new AssignmentWindow(CurrentVolunteer.VolunteerId).ShowDialog();
        else
            new CurrentCallWindow(CurrentVolunteer.CurrentCall!, CurrentVolunteer.VolunteerId).ShowDialog();
    }

    private void btnUpdate_Click(object sender, RoutedEventArgs e)
    {
        new UpdateVolunteerWindow(CurrentVolunteer).ShowDialog();
    }
}
