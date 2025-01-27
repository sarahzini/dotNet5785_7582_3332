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

namespace PL.Call
{
    /// <summary>
    /// Interaction logic for CurrentCallWindow.xaml
    /// </summary>
    public partial class CurrentCallWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public CurrentCallWindow(BO.CallInProgress c,int id=0)
        {
            InitializeComponent();
            CurrentCall = c;
            VolunteerId = id;
        }

        public int VolunteerId
        {
            get { return (int)GetValue(VolunteerIdProperty); }
            set { SetValue(VolunteerIdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VolunteerId.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VolunteerIdProperty =
            DependencyProperty.Register("VolunteerId", typeof(int), typeof(CurrentCallWindow), new PropertyMetadata(0));


        public BO.CallInProgress CurrentCall
        {
            get { return (BO.CallInProgress)GetValue(CurrentCallProperty); }
            set { SetValue(CurrentCallProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Volunteer.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentCallProperty =
            DependencyProperty.Register(nameof(CurrentCall), typeof(BO.CallInProgress), typeof(CurrentCallWindow), new PropertyMetadata(null));

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult confirmation = MessageBox.Show("Are you sure you want to cancel this assignment ?", "Cancel Confirmation",
                                                        MessageBoxButton.YesNo, MessageBoxImage.Question);
            try
            {
                if (confirmation == MessageBoxResult.Yes)
                { 
                    s_bl.Call.CancelAssignment(VolunteerId, CurrentCall.AssignId);
                    MessageBox.Show($"Your recent call {CurrentCall.CallId} succesfully canceled.", "Call Canceled", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void btnFinish_Click(object sender, RoutedEventArgs e)
        {
          
            try
            {
                
                s_bl.Call.CompleteCall(VolunteerId, CurrentCall.AssignId);
                MessageBox.Show($"Your recent call {CurrentCall.CallId} is completed. Thank you for your help !.", "Call Completed", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

    }
}
