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
        public AssignmentWindow(int id)
        {
            InitializeComponent();
            OpenCallList = s_bl.Call.SortOpenCalls(id,null,null);
            id = id;
        }

        public IEnumerable<BO.OpenCallInList>? OpenCallList
        {
            get { return (IEnumerable<BO.OpenCallInList>)GetValue(OpenCallInListProperty); }
            set { SetValue(OpenCallInListProperty, value); }
        }

        public static readonly DependencyProperty OpenCallInListProperty =
            DependencyProperty.Register("OpenCallList", typeof(IEnumerable<BO.OpenCallInList>), typeof(AssignmentWindow), new PropertyMetadata(null));

        private void queryOpenCalInlList() 
        {
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

        public int id { get; set; }

        private void btnChoose_Click(object sender, RoutedEventArgs e)
        {
            //VolunteerInProgressCalls.Add(selectedCall);
            s_bl.Call.AssignCallToVolunteer(id, SelectedCall.CallId);

            // Notify the user
            MessageBox.Show($"You are now assigned to call {SelectedCall.CallId}:{SelectedCall.Description}.",
                                "Call Assigned", MessageBoxButton.OK, MessageBoxImage.Information);
        }

    }
}
