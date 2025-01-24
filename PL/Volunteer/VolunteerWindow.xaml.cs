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
using System.Xml.Linq;
using PL;

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerWindow.xaml
    /// </summary>
    public partial class VolunteerWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public VolunteerWindow(string AddOrUpdate, int id,int requesterId)
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
                        VolunteerDT = BO.DistanceType.AirDistance,
                        CompletedCalls = 0,
                        CancelledCalls = 0,
                        ExpiredCalls = 0,
                        CurrentCall = null };
                else 
                    CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(id);

                requesterId = requesterId;
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

        public int requesterId { get; set; }
        /// <summary>
        /// This method returns the value of the Button text
        /// </summary>
        public BO.Volunteer? CurrentVolunteer
        {
            get { return (BO.Volunteer?)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }

        /// <summary>
        /// This method returns the value of the Button text
        /// </summary>
        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(VolunteerWindow), new PropertyMetadata(null));
        
        /// <summary>
        /// 
        /// </summary>
        public string ButtonText
        {
            get => (string)GetValue(ButtonTextProperty);
            init => SetValue(ButtonTextProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register(nameof(ButtonText), typeof(string), typeof(VolunteerWindow));

        private void btnCurrentCall_Click(object sender, RoutedEventArgs e)
        {
           if (CurrentVolunteer!.CurrentCall != null)
           {
               new CurrentCall(CurrentVolunteer!.CurrentCall!).ShowDialog();
           }
            
        }

            /// <summary>
            /// This method adds or updates a volunteer depending on the button text.
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            /// 
            private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ButtonText == "Add")
                {
                    CurrentVolunteer!.Password = GenerateRandomPassword();
                    MessageBoxResult choice = MessageBox.Show($"The Password per default: {CurrentVolunteer!.Password}. Do you want to change it ?", "Set Password",
                        MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (choice == MessageBoxResult.Yes)
                    {
                        s_bl.Volunteer.AddVolunteer(CurrentVolunteer!);

                        new PasswordWindow(CurrentVolunteer!).ShowDialog();
                    }
                    else
                    {
                        s_bl.Volunteer.AddVolunteer(CurrentVolunteer!);
                    }
                    MessageBox.Show($"The volunteer with the ID number : {CurrentVolunteer?.VolunteerId} was successfully added!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                   
                }
                else
                {
                    s_bl.Volunteer.UpdateVolunteer(CurrentVolunteer!.VolunteerId, CurrentVolunteer!);
                    MessageBox.Show($"The volunteer with the ID number: {CurrentVolunteer?.VolunteerId} was successfully updated!", "", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private static string GenerateRandomPassword()
        {
            const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
            const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digits = "0123456789";
            const string allChars = lowerChars + upperChars + digits;

            Random random = new Random();
            StringBuilder Password = new StringBuilder();

            // Ensure at least one uppercase letter and one digit
            Password.Append(upperChars[random.Next(upperChars.Length)]);
            Password.Append(digits[random.Next(digits.Length)]);

            // Fill the rest of the Password length (6 characters) with random characters
            for (int i = 2; i < 6; i++)
            {
                Password.Append(allChars[random.Next(allChars.Length)]);
            }

            // Shuffle the characters in the Password to ensure randomness
            return new string(Password.ToString().OrderBy(c => random.Next()).ToArray());
        }

        private volatile bool _observerWorking = false; //stage 7


        /// <summary>
        /// This method calls the volunteer observer.
        /// </summary>
        private void volunteerObserver() //stage 7
        {
            if (!_observerWorking)
            {
                _observerWorking = true;
                _ = Dispatcher.BeginInvoke(() =>
                {
                    int id = CurrentVolunteer!.VolunteerId;
                    CurrentVolunteer = null;
                    CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(id);
                    _observerWorking = false;
                });
            }
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
