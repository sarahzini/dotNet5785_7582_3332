using PL.Call;
using PL.Volunteer;
using System.Windows;
using System.Windows.Controls;

namespace PL
{
    public partial class LoginWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public LoginWindow()
        {
            InitializeComponent();
            
        }


        public int Id
        {
            get { return (int)GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Id.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register("Id", typeof(int), typeof(LoginWindow));


        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                
                Password = passwordBox.Password;
            }
        }

        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(LoginWindow), new PropertyMetadata(string.Empty));

        private void btnLoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            { // Perform login logic here
                var job = s_bl.Volunteer?.Login(Id, Password);
                if (job != null)
                {
                    if (job == DO.Job.Manager)
                    {
                        MessageBox.Show("Login successful! Welcome, Manager.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        new MainWindow().Show();
                    }
                    else
                    {
                        MessageBox.Show("Login successful! Welcome, Volunteer.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        new MainVolunteerWindow().Show();
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
