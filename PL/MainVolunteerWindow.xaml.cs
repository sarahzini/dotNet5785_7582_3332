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
    // Static reference to the business logic layer (BL)
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    /// <summary>
    /// Constructor to initialize the MainVolunteerWindow with a volunteer id.
    /// </summary>
    public MainVolunteerWindow(int id)
    {
        try
        {
            InitializeComponent();
            // Fetch the volunteer details using the provided ID
            CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(id);

            // Set the button text based on the volunteer's current call status
            if (CurrentVolunteer.CurrentCall != null)
                ButtonText = "Current Call Details";
            else
                ButtonText = "Assignment to a call";
        }
        catch (BO.BLDoesNotExistException ex)
        {
            // Handle the case where the volunteer does not exist
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (Exception ex)
        {
            // Handle any other exceptions
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Dependency property for the CurrentVolunteer.
    /// </summary>
    public BO.Volunteer CurrentVolunteer
    {
        get { return (BO.Volunteer)GetValue(CurrentVolunteerProperty); }
        set { SetValue(CurrentVolunteerProperty, value); }
    }

    /// <summary>
    /// Dependency property for CurrentVolunteer.
    /// </summary>
    public static readonly DependencyProperty CurrentVolunteerProperty =
        DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(MainVolunteerWindow), new PropertyMetadata(null));

    /// <summary>
    /// Property for the text to be displayed on the button.
    /// </summary>
    public string ButtonText
    {
        get => (string)GetValue(ButtonTextProperty);
        set => SetValue(ButtonTextProperty, value);
    }

    /// <summary>
    /// Dependency property for ButtonText.
    /// </summary>
    public static readonly DependencyProperty ButtonTextProperty =
        DependencyProperty.Register("ButtonText", typeof(string), typeof(MainVolunteerWindow), new PropertyMetadata(null));

    /// <summary>
    /// Method to update the volunteer's details and button text.
    /// </summary>
    private void volunteerObserver()
    {
        int id = CurrentVolunteer!.VolunteerId;
        // Update the volunteer details
        CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(id);

        // Update the button text based on whether the volunteer has a current call or not
        if (CurrentVolunteer.CurrentCall != null)
            ButtonText = "Current Call Details";
        else
            ButtonText = "Assignment to a call";
    }

    /// <summary>
    /// This method is triggered when the window is loaded.
    /// It adds an observer to track changes to the current volunteer.
    /// </summary>
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        s_bl.Volunteer.AddObserver(CurrentVolunteer!.VolunteerId, volunteerObserver);
    }

    /// <summary>
    /// This method is triggered when the window is closed.
    /// It removes the observer that was added during the loading of the window.
    /// </summary>
    private void Window_Closed(object sender, EventArgs e)
    {
        s_bl.Volunteer.RemoveObserver(CurrentVolunteer!.VolunteerId, volunteerObserver);
    }

    /// <summary>
    /// This method is triggered when the "Historic Calls" button is clicked.
    /// It opens the HistoricWindow for the current volunteer.
    /// </summary>
    private void btnHistoric_Click(object sender, RoutedEventArgs e)
    {
        new HistoricWindow(CurrentVolunteer.VolunteerId).ShowDialog();
    }

    /// <summary>
    /// This method is triggered when the "Assignment" button is clicked.
    /// It opens the appropriate window based on whether the volunteer has a current call.
    /// </summary>
    private void btnAssignment_Click(object sender, RoutedEventArgs e)
    {
        if (ButtonText == "Assignment to a call")
            new AssignmentWindow(CurrentVolunteer.VolunteerId).ShowDialog();
        else
            new CurrentCallWindow(CurrentVolunteer.CurrentCall!, CurrentVolunteer.VolunteerId).ShowDialog();
    }

    /// <summary>
    /// This method is triggered when the "Update Details" button is clicked.
    /// It opens the VolunteerWindow for updating the volunteer's details.
    /// </summary>
    private void btnUpdate_Click(object sender, RoutedEventArgs e)
    {
        new VolunteerWindow("Update", CurrentVolunteer.VolunteerId, CurrentVolunteer.VolunteerId, false).ShowDialog();
    }
}
