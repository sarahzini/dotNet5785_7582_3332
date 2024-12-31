using PL.Call;
using PL.Volunteer;
using System.Windows;

namespace PL
{
    public partial class Login : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public Login()
        {
            InitializeComponent();
        }
        BO.Volunteer? CurrentVolunteer
        {
            get { return (BO.Volunteer?)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }

        public static readonly DependencyProperty CurrentVolunteerProperty=
       DependencyProperty.Register("CurrentCall", typeof(BO.Volunteer), typeof(VolunteerWindow), new PropertyMetadata(null));

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            { // Perform login logic here
                var job = s_bl.Volunteer?.Login(CurrentVolunteer.VolunteerId, CurrentVolunteer.Password);
                if (job != null)
                {
                    if (job == DO.Job.Manager)
                    {
                        MessageBox.Show("Login successful! Welcome, Manager.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        // Open manager window
                        new ManagerWindow().Show();
                    }
                    else
                    {
                        MessageBox.Show("Login successful! Welcome, Volunteer.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        // Open volunteer window
                        new VolunteerWindow().Show();
                    }

                }
            }
            
            catch(BO.BLIncorrectPassword ex )
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
    }
}
