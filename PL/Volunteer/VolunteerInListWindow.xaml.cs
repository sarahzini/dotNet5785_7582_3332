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
/// </summary>
public partial class VolunteerInListWindow : Window
{
    /// To gain access to the BL layer, we need to use the Factory class.
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public VolunteerInListWindow(int id)
    {
        InitializeComponent();
        requesterId = id;
    }

    public int requesterId { get; set; } 
    public BO.SystemType Ambulance { get; set; } = BO.SystemType.All;

    private void FilteredVolunteer_SelectionChanged(object sender, SelectionChangedEventArgs e) => queryVolunteerList();
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
    private void queryVolunteerList()
         => VolunteerList = (Ambulance == BO.SystemType.All) ?
                   s_bl?.Volunteer.GetVolunteersInList()! : s_bl?.Volunteer.GetFilteredVolunteersInList(Ambulance)!;

    private void volunteerListObserver()
        => queryVolunteerList();

    private void Window_Loaded(object sender, RoutedEventArgs e)
    => s_bl.Volunteer.AddObserver(volunteerListObserver);

    private void Window_Closed(object sender, EventArgs e)
        => s_bl.Volunteer.RemoveObserver(volunteerListObserver);


    private void lsvUpdate_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    { 
        new VolunteerWindow("Update", SelectedVolunteer.VolunteerId,requesterId).Show(); 

    }

    private void btnAdd_Click(object sender, RoutedEventArgs e)
    {
        new VolunteerWindow("Add", 0, requesterId).Show();
    }

    /*
    public BO.VolunteerInList? SelectedVolunteer
    {
        get { return (BO.VolunteerInList?)GetValue(SelectedVolunteerProperty); }
        set { SetValue(SelectedVolunteerProperty, value); }
    }

    public static readonly DependencyProperty SelectedVolunteerProperty =
        DependencyProperty.Register("SelectedVolunteer", typeof(BO.VolunteerInList), typeof(VolunteerInListWindow), new PropertyMetadata(null));
    */

    public BO.VolunteerInList? SelectedVolunteer { get; set; }


    private void btnDeleteVolunteer_Click(object sender, RoutedEventArgs e)
    {

        var button = sender as Button;
        var volunteer = button?.CommandParameter as BO.VolunteerInList; // Assuming Volunteer is the type of your items in the list

        MessageBoxResult confirmation = MessageBox.Show("Are you sure you want to delete this volunteer ?", "Delete Confirmation",
                                                    MessageBoxButton.YesNo, MessageBoxImage.Question);
        try
        {
            if (confirmation == MessageBoxResult.Yes)
            {
                s_bl.Volunteer.DeleteVolunteer(volunteer.VolunteerId);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

    }

}
