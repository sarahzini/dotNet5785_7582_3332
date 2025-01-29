using System;
using System.Windows;
using System.Windows.Controls;

namespace PL.Volunteer
{

    public partial class PasswordWindow : Window
    {
        private readonly PasswordViewModel _viewModel;
        private readonly BlApi.IBl bl = BlApi.Factory.Get();
        private readonly BO.Volunteer volunteer;
        private string actualPassword = string.Empty;

        public PasswordWindow(BO.Volunteer Volunteer)
        {
            InitializeComponent();
            _viewModel = new PasswordViewModel();
            DataContext = _viewModel;
            volunteer = Volunteer;
        }
        private void UpdatePassword_Click(object sender, RoutedEventArgs e)
        {
            string actualPassword = _viewModel.ActualPassword;
            try
            {
                if (string.IsNullOrWhiteSpace(actualPassword))
                {
                    MessageBox.Show("Password cannot be empty!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                volunteer.Password = actualPassword;
                bl.Volunteer.UpdateVolunteer(volunteer.VolunteerId, volunteer);

                MessageBox.Show($"Password '{actualPassword}' updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}