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

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerWindow.xaml
    /// </summary>
    public partial class VolunteerWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public VolunteerWindow(string AddOrUpdate, int id)
        {
            try
            {
                ButtonText = AddOrUpdate == "Add" ? "Add" : "Update";
                InitializeComponent();

                if (ButtonText == "Add")
                    CurrentVolunteer = new BO.Volunteer() { 
                        VolunteerId = 0,
                        Name = "",
                        PhoneNumber = "",
                        Email = "",
                        Password = "",
                        VolunteerAddress = null,
                        VolunteerLatitude = null,
                        VolunteerLongitude = null,
                        VolunteerJob = BO.Job.Volunteer,
                        IsActive = true,
                        MaxVolunteerDistance = null,
                        VolunteerDT = BO.DistanceType.AirDistance),
                        CompletedCalls = 0,
                        CancelledCalls = 0,
                        ExpiredCalls = 0,
                        CurrentCall = null };
                else 
                    CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(id);
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

        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("CurrentCourse", typeof(BO.Volunteer), typeof(VolunteerWindow), new PropertyMetadata(null));
        string ButtonText
        {
            get => (string)GetValue(ButtonTextProperty);
            init => SetValue(ButtonTextProperty, value);
        }
        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register(nameof(ButtonText), typeof(string), typeof(VolunteerWindow));

        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ButtonText == "Add")
                {
                    s_bl.Volunteer.AddVolunteer(CurrentVolunteer!);
                    MessageBox.Show($"The volunteer {CurrentVolunteer?.VolunteerId} was successfully added!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    s_bl.Volunteer.UpdateVolunteer(CurrentVolunteer!.VolunteerId, CurrentVolunteer!);
                    //we will change the parameter of id in stage 6 because it depens of screen login
                    MessageBox.Show($"The volunteer {CurrentVolunteer?.VolunteerId} was successfully updated!", "", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void volunteerObserver() 
        {
            int id = CurrentVolunteer!.VolunteerId;
            CurrentVolunteer = null;
            CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(id);
                
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentVolunteer!.VolunteerId != 0)
                s_bl.Volunteer.AddObserver(CurrentVolunteer!.VolunteerId, volunteerObserver);
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Volunteer.RemoveObserver(CurrentVolunteer!.VolunteerId, volunteerObserver);
        }
    }
}
