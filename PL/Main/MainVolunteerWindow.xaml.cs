using PL.Call;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PL.Main;

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
            BO.Volunteer? CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(id);
            if(CurrentVolunteer.CurrentCall != null)
                ButtonText = "Actual Call";
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

    public BO.Volunteer? CurrentVolunteer
    {
        get { return (BO.Volunteer?)GetValue(CurrentVolunteerProperty); }
        set { SetValue(CurrentVolunteerProperty, value); }
    }

    /// <summary>
    /// This method returns the value of the Current Volunteer
    /// </summary>
    public static readonly DependencyProperty CurrentVolunteerProperty =
        DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(VolunteerWindow), new PropertyMetadata(null));

    string ButtonText
    {
        get => (string)GetValue(ButtonTextProperty);
        init => SetValue(ButtonTextProperty, value);
    }

    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty ButtonTextProperty =
        DependencyProperty.Register(nameof(ButtonText), typeof(string), typeof(VolunteerWindow));

    private void btnHistoric_Click(object sender, RoutedEventArgs e)
    {
        new HistoricWindow(CurrentVolunteer!.VolunteerId).ShowDialog();
    }
    private void btnAssignment_Click(object sender, RoutedEventArgs e)
    {
        if (ButtonText == "Assignment to a call")
            new AssignmentWindow().ShowDialog();
        else 
           new CurrentCallWindow(CurrentVolunteer).ShowDialog();
    }

    private void btnUpdate_Click(object sender, RoutedEventArgs e)
    {
        new UpdateVolunteer(CurrentVolunteer).ShowDialog();
    }
}
