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

namespace PL.Call
{
    /// <summary>
    /// Interaction logic for CurrentCallWindow.xaml
    /// </summary>
    public partial class CurrentCallWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public CurrentCallWindow(BO.Volunteer v)
        {
            InitializeComponent();
            VolunteerId = v.VolunteerId;
        }

        public int VolunteerId {get; set;}
        public BO.CallInProgress CurrentCall
        {
            get { return (BO.CallInProgress)GetValue(CurrentCallProperty); }
            set { SetValue(CurrentCallProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Volunteer.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentCallProperty =
            DependencyProperty.Register("Volunteer", typeof(BO.Volunteer), typeof(CurrentCallWindow), new PropertyMetadata(0));

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult confirmation = MessageBox.Show("Are you sure you want to cancel this assignment ?", "Delete Confirmation",
                                                        MessageBoxButton.YesNo, MessageBoxImage.Question);
            try
            {
                if (confirmation == MessageBoxResult.Yes)
                { 
                    s_bl.Call.CancelAssignment(VolunteerId, CurrentCall.AssignId);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void btnFinish_Click(object sender, RoutedEventArgs e)
        {
           

        }

    }
}
