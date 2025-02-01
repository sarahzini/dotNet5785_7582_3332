using System;
using System.Windows;
using System.Windows.Controls;

namespace PL.Volunteer
{
    public partial class PasswordWindow : Window
    {
        // ViewModel for managing the password data
        private readonly PasswordViewModel _viewModel;

        // Business logic instance to interact with the application's backend
        private readonly BlApi.IBl bl = BlApi.Factory.Get();

        // The volunteer whose password is being updated
        private readonly BO.Volunteer volunteer;

        // Placeholder for the actual password
        private string actualPassword = string.Empty;

        // Constructor that initializes the window with a given volunteer
        public PasswordWindow(BO.Volunteer Volunteer)
        {
            InitializeComponent();
            _viewModel = new PasswordViewModel();
            DataContext = _viewModel;
            volunteer = Volunteer;
        }

        // Event handler for updating the volunteer's password
        private void UpdatePassword_Click(object sender, RoutedEventArgs e)
        {
            string actualPassword = _viewModel.ActualPassword;
            try
            {
                // Check if the password field is empty
                if (string.IsNullOrWhiteSpace(actualPassword))
                {
                    MessageBox.Show("Password cannot be empty!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Update the volunteer's password
                volunteer.Password = actualPassword;
                bl.Volunteer.UpdateVolunteer(volunteer.VolunteerId, volunteer);

                // Notify the user of the successful update
                MessageBox.Show($"Password updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Close the window after updating
                this.Close();
            }
            catch (Exception ex)
            {
                // Display any error messages that occur during the process
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
