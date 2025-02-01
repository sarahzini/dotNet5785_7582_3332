using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        /// <summary>
        /// Reference to the business logic layer.
        /// </summary>
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        /// <summary>
        /// Initializes a new instance of the CurrentCallWindow class.
        /// </summary>
        /// <param name="c">The call in progress.</param>
        /// <param name="id">The ID of the volunteer handling the call.</param>
        public CurrentCallWindow(BO.CallInProgress c, int id = 0)
        {
            InitializeComponent();
            CurrentCall = c;
            VolunteerId = id;
        }

        /// <summary>
        /// Gets or sets the volunteer ID.
        /// </summary>
        public int VolunteerId
        {
            get { return (int)GetValue(VolunteerIdProperty); }
            set { SetValue(VolunteerIdProperty, value); }
        }

        /// <summary>
        /// Dependency property for VolunteerId, enabling binding, styling, and animations.
        /// </summary>
        public static readonly DependencyProperty VolunteerIdProperty =
            DependencyProperty.Register("VolunteerId", typeof(int), typeof(CurrentCallWindow), new PropertyMetadata(0));

        /// <summary>
        /// Gets or sets the current call in progress.
        /// </summary>
        public BO.CallInProgress CurrentCall
        {
            get { return (BO.CallInProgress)GetValue(CurrentCallProperty); }
            set { SetValue(CurrentCallProperty, value); }
        }

        /// <summary>
        /// Dependency property for CurrentCall, enabling binding, styling, and animations.
        /// </summary>
        public static readonly DependencyProperty CurrentCallProperty =
            DependencyProperty.Register(nameof(CurrentCall), typeof(BO.CallInProgress), typeof(CurrentCallWindow), new PropertyMetadata(null));

        /// <summary>
        /// Handles the cancel button click event. 
        /// Displays a confirmation message and cancels the call assignment if confirmed.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult confirmation = MessageBox.Show("Are you sure you want to cancel this assignment ?", "Cancel Confirmation",
                                                            MessageBoxButton.YesNo, MessageBoxImage.Question);
            try
            {
                if (confirmation == MessageBoxResult.Yes)
                {
                    s_bl.Call.CancelAssignment(VolunteerId, CurrentCall.AssignId, CurrentCall.Status);
                    MessageBox.Show($"The call {CurrentCall.CallId} is now canceled. Thank you for confirming!", "Call Canceled", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Handles the finish button click event. 
        /// Marks the call as complete and displays a confirmation message.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void btnFinish_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Call.CompleteCall(VolunteerId, CurrentCall.AssignId);
                MessageBox.Show($"The call {CurrentCall.CallId} is now complete. We sincerely appreciate your help!", "Call Completed", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
