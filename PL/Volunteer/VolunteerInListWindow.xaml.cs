using PL.Call;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

namespace PL.Volunteer;

/// <summary>
/// Interaction logic for VolunteerInList.xaml
/// This class represents a window that displays a list of volunteers.
/// </summary>
public partial class VolunteerInListWindow : Window
{
    /// <summary>
    /// Access to the Business Logic (BL) layer through the Factory class.
    /// </summary>
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    /// <summary>
    /// Constructor that initializes the window with a given requester ID.
    /// </summary>
    public VolunteerInListWindow(int id)
    {
        InitializeComponent();
        requesterId = id;
    }

    public int requesterId { get; set; }
    public BO.SystemType Ambulance { get; set; } = BO.SystemType.All;
    public BO.VolunteerInListFieldSort Field { get; set; } = BO.VolunteerInListFieldSort.VolunteerId;

    /// <summary>
    /// Dependency property for storing the volunteer list.
    /// </summary>
    public IEnumerable<BO.VolunteerInList> VolunteerList
    {
        get { return (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerListProperty); }
        set { SetValue(VolunteerListProperty, value); }
    }

    public static readonly DependencyProperty VolunteerListProperty =
        DependencyProperty.Register("VolunteerList",
            typeof(IEnumerable<BO.VolunteerInList>),
            typeof(VolunteerInListWindow),
            new PropertyMetadata(null));

    /// <summary>
    /// Dependency property for storing the selected volunteer.
    /// </summary>
    public BO.VolunteerInList? SelectedVolunteer
    {
        get { return (BO.VolunteerInList?)GetValue(SelectedVolunteerProperty); }
        set { SetValue(SelectedVolunteerProperty, value); }
    }

    public static readonly DependencyProperty SelectedVolunteerProperty =
        DependencyProperty.Register(nameof(SelectedVolunteer),
            typeof(BO.VolunteerInList),
            typeof(VolunteerInListWindow),
            new PropertyMetadata(null));

    /// <summary>
    /// Event handler for selection change in filter combo boxes.
    /// Triggers a query to refresh the volunteer list.
    /// </summary>
    private void FilteredVolunteer_SelectionChanged(object sender, SelectionChangedEventArgs e) => queryVolunteerList();

    /// <summary>
    /// Queries and updates the volunteer list based on selected filters.
    /// </summary>
    private void queryVolunteerList()
         => VolunteerList = (Ambulance == BO.SystemType.All) ?
                   s_bl?.Volunteer.GetVolunteersInList(null, null, Field)! : s_bl?.Volunteer.GetVolunteersInList(null, Ambulance, Field)!;

    private volatile bool _observerWorking = false; // Stage 7: Used to track if the observer is already working

    /// <summary>
    /// Observer function that updates the volunteer list asynchronously.
    /// Ensures that only one observer is working at a time.
    /// </summary>
    private void volunteerListObserver()
    {
        if (!_observerWorking)
        {
            _observerWorking = true;
            _ = Dispatcher.BeginInvoke(() =>
            {
                queryVolunteerList();
                _observerWorking = false;
            });
        }
    }

    /// <summary>
    /// Event handler for window load event.
    /// Adds the volunteer list observer to track changes in real-time.
    /// </summary>
    private void Window_Loaded(object sender, RoutedEventArgs e)
    => s_bl.Volunteer.AddObserver(volunteerListObserver);

    /// <summary>
    /// Event handler for window close event.
    /// Removes the volunteer list observer when the window is closed.
    /// </summary>
    private void Window_Closed(object sender, EventArgs e)
        => s_bl.Volunteer.RemoveObserver(volunteerListObserver);

    /// <summary>
    /// Event handler for double-clicking an item in the ListView.
    /// Opens a new window to update the selected volunteer.
    /// </summary>
    private void lsvUpdate_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        new VolunteerWindow("Update", SelectedVolunteer!.VolunteerId, requesterId, true).Show();
    }

    /// <summary>
    /// Event handler for adding a new volunteer.
    /// Opens a new window for volunteer creation.
    /// </summary>
    private void btnAdd_Click(object sender, RoutedEventArgs e)
    {
        new VolunteerWindow("Add", 0, requesterId, true).Show();
    }

    /// <summary>
    /// Event handler for deleting a selected volunteer.
    /// Prompts the user for confirmation before deletion.
    /// </summary>
    private void btnDeleteVolunteer_Click(object sender, RoutedEventArgs e)
    {
        MessageBoxResult confirmation = MessageBox.Show("Are you sure you want to delete this volunteer ?", "Delete Confirmation",
                                                    MessageBoxButton.YesNo, MessageBoxImage.Question);
        try
        {
            if (SelectedVolunteer != null && confirmation == MessageBoxResult.Yes)
            {
                s_bl.Volunteer.DeleteVolunteer(SelectedVolunteer!.VolunteerId);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
