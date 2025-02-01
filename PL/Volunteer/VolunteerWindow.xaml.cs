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
        // A static reference to the business logic (BL) interface
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        // Constructor that initializes the window for adding or updating a volunteer
        public VolunteerWindow(string AddOrUpdate, int id, int requesterid, bool ismanager)
        {
            isManager = ismanager;
            try
            {
                // Set button text based on whether it's adding or updating a volunteer
                ButtonText = AddOrUpdate == "Add" ? "Add" : "Update";
                InitializeComponent();

                // Initialize a new volunteer for adding
                if (ButtonText == "Add")
                    CurrentVolunteer = new BO.Volunteer()
                    {
                        VolunteerId = 0,
                        Name = "",
                        PhoneNumber = "",
                        Email = "",
                        Password = "",
                        VolunteerAddress = null,
                        VolunteerLatitude = 0,
                        VolunteerLongitude = 0,
                        VolunteerJob = BO.Job.Volunteer,
                        IsActive = true,
                        MaxVolunteerDistance = null,
                        VolunteerDT = BO.DistanceType.AirDistance,
                        CompletedCalls = 0,
                        CancelledCalls = 0,
                        ExpiredCalls = 0,
                        CurrentCall = null
                    };
                // Otherwise, load the details of the existing volunteer
                else
                    CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(id);

                requesterId = requesterid;
            }
            catch (BO.BLDoesNotExistException ex)
            {
                // Handle exception when the volunteer doesn't exist
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                // Handle any other general exception
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        // Property for storing the requester ID
        public int requesterId { get; set; }

        // Property for managing whether the user is a manager
        public bool isManager
        {
            get { return (bool)GetValue(isManagerProperty); }
            set { SetValue(isManagerProperty, value); }
        }

        // Using a DependencyProperty for the isManager property to enable binding and styling
        public static readonly DependencyProperty isManagerProperty =
            DependencyProperty.Register("isManagerProperty", typeof(bool), typeof(VolunteerWindow), new PropertyMetadata(true));

        // Property for binding the volunteer details to the window
        public BO.Volunteer? CurrentVolunteer
        {
            get { return (BO.Volunteer?)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for binding CurrentVolunteer
        /// </summary>
        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(VolunteerWindow), new PropertyMetadata(null));

        /// <summary>
        /// Property for the text of the button (Add or Update)
        /// </summary>
        public string ButtonText
        {
            get => (string)GetValue(ButtonTextProperty);
            init => SetValue(ButtonTextProperty, value);
        }

        /// <summary>
        /// DependencyProperty for ButtonText
        /// </summary>
        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register(nameof(ButtonText), typeof(string), typeof(VolunteerWindow));

        // Event handler for when the "Current Call" button is clicked
        private void btnCurrentCall_Click(object sender, RoutedEventArgs e)
        {
            // If the volunteer has a current call, show the current call window
            if (CurrentVolunteer!.CurrentCall != null)
            {
                new Call.CurrentCallWindow(CurrentVolunteer!.CurrentCall!, 0).ShowDialog();
            }
        }

        /// <summary>
        /// This method adds or updates a volunteer depending on the button text.
        /// </summary>
        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // If adding a new volunteer
                if (ButtonText == "Add")
                {
                    // Generate a random password for the volunteer
                    CurrentVolunteer!.Password = GenerateRandomPassword();
                    MessageBoxResult choice = MessageBox.Show($"The Password per default is : {CurrentVolunteer!.Password}. Do you want to change it ?", "Set Password",
                        MessageBoxButton.YesNo, MessageBoxImage.Question);

                    // If the user chooses to change the password
                    if (choice == MessageBoxResult.Yes)
                    {
                        // Add the volunteer and show the password window
                        s_bl.Volunteer.AddVolunteer(CurrentVolunteer!);
                        new PasswordWindow(CurrentVolunteer!).ShowDialog();
                    }
                    else
                    {
                        // Add the volunteer without changing the default password
                        s_bl.Volunteer.AddVolunteer(CurrentVolunteer!);
                    }

                    // Inform the user that the volunteer was successfully added
                    MessageBox.Show($"The volunteer with the ID number : {CurrentVolunteer?.VolunteerId} was successfully added!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else
                {
                    // If updating an existing volunteer
                    s_bl.Volunteer.UpdateVolunteer(requesterId, CurrentVolunteer!);
                    MessageBox.Show($"The volunteer with the ID number: {CurrentVolunteer?.VolunteerId} was successfully updated!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (BO.BLAlreadyExistException ex)
            {
                // Handle exception when the volunteer already exists
                string errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += $" {ex.InnerException.Message}";
                }
                MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BO.BLDoesNotExistException ex)
            {
                // Handle exception when the volunteer doesn't exist
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BO.BLFormatException ex)
            {
                // Handle exception for invalid format
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                // Handle any other general exception
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Method to generate a random password with at least one uppercase letter and one digit
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

            // Fill the rest of the password length (6 characters) with random characters
            for (int i = 2; i < 6; i++)
            {
                Password.Append(allChars[random.Next(allChars.Length)]);
            }

            // Shuffle the characters in the password to ensure randomness
            return new string(Password.ToString().OrderBy(c => random.Next()).ToArray());
        }

        // Flag to ensure the observer is not working simultaneously in multiple threads
        private volatile bool _observerWorking = false;

        /// <summary>
        /// This method calls the volunteer observer to refresh the volunteer's details.
        /// </summary>
        private void volunteerObserver()
        {
            // Ensure only one observer is working at a time
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
        /// This method is called when the window is loaded, and it adds the observer for the volunteer.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Add observer if the volunteer has an ID (not a new volunteer)
            if (CurrentVolunteer!.VolunteerId != 0)
                s_bl.Volunteer.AddObserver(CurrentVolunteer!.VolunteerId, volunteerObserver);
        }

        /// <summary>
        /// This method is called when the window is closed, and it removes the observer for the volunteer.
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
        {
            // Remove the observer for the volunteer
            s_bl.Volunteer.RemoveObserver(CurrentVolunteer!.VolunteerId, volunteerObserver);
        }
    }
}
