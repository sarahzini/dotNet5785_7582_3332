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

    /// <summary>
    /// Constructor for the LoginWindow. Initializes the components.
    /// </summary>
    public LoginWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Gets or sets the Id property (User's ID).
    /// </summary>
    public int Id
    {
        get { return (int)GetValue(IdProperty); }
        set { SetValue(IdProperty, value); }
    }

    /// <summary>
    /// Dependency property for Id. This enables binding and other features.
    /// </summary>
    public static readonly DependencyProperty IdProperty =
        DependencyProperty.Register("Id", typeof(int), typeof(LoginWindow));

    /// <summary>
    /// Gets or sets the Password property.
    /// </summary>
    public string Password
    {
        get { return (string)GetValue(PasswordProperty); }
        set { SetValue(PasswordProperty, value); }
    }

    /// <summary>
    /// Dependency property for Password. This enables binding and other features.
    /// </summary>
    public static readonly DependencyProperty PasswordProperty =
        DependencyProperty.Register("Password", typeof(string), typeof(LoginWindow), new PropertyMetadata(string.Empty));

    /// <summary>
    /// Handles the PasswordBox's PasswordChanged event to update the Password property.
    /// </summary>
    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is PasswordBox passwordBox)
        {
            Password = passwordBox.Password;
        }
    }

    /// <summary>
    /// Handles the KeyDown event for PasswordBox to submit the login when the Enter key is pressed.
    /// </summary>
    private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            btnLoginButton_Click(sender, e);
        }
    }

    /// <summary>
    /// Handles the click event of the login button. Validates the user credentials.
    /// If login is successful, it navigates to either the Manager or Volunteer window.
    /// </summary>
    private void btnLoginButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Perform login logic here
            var job = s_bl.Volunteer?.Login(Id, Password);
            string name = s_bl.Volunteer!.GetName(Id);
            if (job != null)
            {
                if (job == DO.Job.Manager)
                {
                    // If the user is a manager, ask if they want to enter the Manager Menu
                    MessageBoxResult choice = MessageBox.Show("Do you wish to enter the Manager Menu?", "Choice",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (choice == MessageBoxResult.Yes)
                    {
                        // Check if another manager is already logged in
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
                    // If the user is a volunteer, navigate to the volunteer window
                    MessageBox.Show($"Login successful! Welcome, {name}.", "Login", MessageBoxButton.OK, MessageBoxImage.Information);
                    new MainVolunteerWindow(Id).Show();
                }
            }
        }
        catch (BO.BLIncorrectPassword ex)
        {
            // Handle incorrect password exception
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (BO.BLDoesNotExistException ex)
        {
            // Handle user does not exist exception
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (Exception ex)
        {
            // Handle any other exceptions
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
