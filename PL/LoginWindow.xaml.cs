using PL.Call;
using PL.Volunteer;
using System.Diagnostics.Eventing.Reader;
using System.Diagnostics.Metrics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PL;

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

    private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
           btnLoginButton_Click(sender, e);
        }
    }

    private void btnLoginButton_Click(object sender, RoutedEventArgs e)
    {
        try
        { // Perform login logic here
            var job = s_bl.Volunteer?.Login(Id, Password);
            string name = s_bl.Volunteer!.GetName(Id);
            if (job != null)
            {
                if (job == DO.Job.Manager)
                {
                    MessageBoxResult choice = MessageBox.Show("Do you wish to enter the Manager Menu? ","Choice",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (choice == MessageBoxResult.Yes)
                    {
                        foreach (Window window in Application.Current.Windows)
                        {
                            if (window is MainManagerWindow)
                            {
                                throw new Exception("There is another manager that logged in. Please login as volunteer or come back later!");
                            }
                        }
                        MessageBox.Show($"Login successful! Welcome {name}.", "Login", MessageBoxButton.OK, MessageBoxImage.Information);
                        new MainManagerWindow(Id).Show();
                    }
                    else
                    {
                        MessageBox.Show($"Login successful! Welcome, {name}.", "Login", MessageBoxButton.OK, MessageBoxImage.Information);
                        new MainVolunteerWindow(Id).Show();
                    }
                }
                else
                {

                    MessageBox.Show($"Login successful! Welcome, {name}.", "Login", MessageBoxButton.OK, MessageBoxImage.Information);
                    new MainVolunteerWindow(Id).Show();
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
