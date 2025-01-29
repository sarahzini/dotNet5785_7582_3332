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
    public BO.VolunteerInListFieldSort Field { get; set; } = BO.VolunteerInListFieldSort.VolunteerId;

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
                   s_bl?.Volunteer.GetVolunteersInList(null,null,Field)! : s_bl?.Volunteer.GetVolunteersInList(null,Ambulance,Field)!;

    private volatile bool _observerWorking = false; //stage 7
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


    private void Window_Loaded(object sender, RoutedEventArgs e)
    => s_bl.Volunteer.AddObserver(volunteerListObserver);

    private void Window_Closed(object sender, EventArgs e)
        => s_bl.Volunteer.RemoveObserver(volunteerListObserver);


    private void lsvUpdate_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    { 
        new VolunteerWindow("Update", SelectedVolunteer!.VolunteerId,requesterId,true).Show(); 

    }

    private void btnAdd_Click(object sender, RoutedEventArgs e)
    {
        new VolunteerWindow("Add", 0, requesterId,true).Show();
    }

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

    private void btnDeleteVolunteer_Click(object sender, RoutedEventArgs e)
    {
        MessageBoxResult confirmation = MessageBox.Show("Are you sure you want to delete this volunteer ?", "Delete Confirmation",
                                                    MessageBoxButton.YesNo, MessageBoxImage.Question);
        try
        {
            if (SelectedVolunteer!=null &&confirmation == MessageBoxResult.Yes)
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
