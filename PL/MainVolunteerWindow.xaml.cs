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

namespace PL
{
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


        /// <summary>
        /// This method adds or updates a volunteer depending on the button text.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Volunteer.UpdateVolunteer(CurrentVolunteer!.VolunteerId, CurrentVolunteer!);
                MessageBox.Show($"The volunteer with the ID number: {CurrentVolunteer?.VolunteerId} was successfully updated!", "", MessageBoxButton.OK, MessageBoxImage.Information);

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
            new HistoricWindow.ShowDialog(CurrentVolunteer!.VolunteerId);
        }
        private void btnAssignment_Click(object sender, RoutedEventArgs e)
        {
            if(ButtonText=="Assignment to a call")
               new AssignmentWindow.ShowDialog();
            else 
                new ActualCallWindow.ShowDialog(CurrentVolunteer!.VolunteerId);
        }

        /// <summary>
        /// This method calls the volunteer observer.
        /// </summary>
        private void volunteerObserver()
        {
            int id = CurrentVolunteer!.VolunteerId;
            CurrentVolunteer = null;
            CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(id);

        }

        /// <summary>
        /// This method loads the window.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentVolunteer!.VolunteerId != 0)
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
    }
}
