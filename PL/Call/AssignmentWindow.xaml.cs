using BO;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PL.Call
{
    /// <summary>
    /// Interaction logic for AssignmentWindow.xaml
    /// </summary>
    public partial class AssignmentWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public BO.Statuses Status { get; set; } = BO.Statuses.All;
        public AssignmentWindow()
        {
            InitializeComponent();
        }

        public IEnumerable<BO.OpenCallInList> OpenCallList
        {
            get { return (IEnumerable<BO.OpenCallInList>)GetValue(OpenCallInListProperty); }
            set { SetValue(OpenCallInListProperty, value); }
        }

        public static readonly DependencyProperty OpenCallInListProperty =
            DependencyProperty.Register("OpenCallInList", typeof(IEnumerable<BO.OpenCallInList>), typeof(AssignmentWindow));

        private void queryOpenCalInlList() 
        {
            if(s_bl.Call != null)
            {
                if (Status == BO.Statuses.Open || Status == BO.Statuses.OpenToRisk)
                { OpenCallList = (IEnumerable<OpenCallInList>)s_bl.Call.GetSortedCallsInList(); }
            }
        }
        private void OpenCallListObserver()
            => queryOpenCalInlList();

        private void Window_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Call.AddObserver(OpenCallListObserver);

        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Call.RemoveObserver(OpenCallListObserver);

        /// <summary>
        /// This method gets the selected call.
        /// </summary>
        public BO.OpenCallInList? SelectedCall { get; set; }

        private void btnChoose_Click(object sender, RoutedEventArgs e)
        {
         
            if (SelectedCall != null)
            {
                // Move the call to the "In Progress" state
                //OpenCallInList.(selectedCall);

                //VolunteerInProgressCalls.Add(selectedCall);

                // Optionally notify the user
                MessageBox.Show($"You are now assigned to call {SelectedCall.CallId}.",
                                "Call Assigned", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        private void btnDetails_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCall != null)
            {
                // Display the details of the selected call
                MessageBox.Show(
                                $"Description: {SelectedCall.Description}\n" +
                                //faire tosafot
                                , MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
