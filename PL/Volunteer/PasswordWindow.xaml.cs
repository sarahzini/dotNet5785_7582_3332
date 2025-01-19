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

namespace PL.Volunteer;

/// <summary>
/// Interaction logic for PasswordWindow.xaml
/// </summary>
public partial class PasswordWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public PasswordWindow(BO.Volunteer? CurrentVolunteer)
    {
        InitializeComponent();
        Password = CurrentVolunteer!.Password;
        CurrentVolunteer = CurrentVolunteer;
    }

    BO.Volunteer? CurrentVolunteer { get; set; }
    public string Password
    {
        get { return (string)GetValue(PasswordProperty); }
        set { SetValue(PasswordProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Password.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty PasswordProperty =
        DependencyProperty.Register("Password", typeof(string), typeof(PasswordWindow), new PropertyMetadata(0));

    private void btnPassword_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (!(Password.Any(char.IsUpper) && Password.Any(char.IsDigit)))
            {
                throw new Exception("Password must contain at least one uppercase letter and one number. Please try again.");
            }

            CurrentVolunteer!.Password = Password;
            s_bl.Volunteer.UpdateVolunteer(CurrentVolunteer!.VolunteerId, CurrentVolunteer!);
            MessageBox.Show($"The password is recorded", "", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();

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
}
