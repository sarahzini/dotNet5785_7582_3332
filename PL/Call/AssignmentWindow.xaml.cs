using BO;
using DO;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PL.Call
{
    /// <summary>
    /// Interaction logic for AssignmentWindow.xaml
    /// This window is used to display and manage open calls, allowing volunteers to assign themselves to a call.
    /// </summary>
    public partial class AssignmentWindow : Window
    {
        /// <summary>
        /// Static instance of the BL interface for accessing business logic.
        /// </summary>
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        /// <summary>
        /// Constructor for AssignmentWindow.
        /// Initializes the window and fetches the sorted list of open calls based on the volunteer's ID.
        /// </summary>
        /// <param name="Id">ID of the volunteer.</param>
        public AssignmentWindow(int Id)
        {
            id = Id;
            OpenCallList = s_bl.Call.SortOpenCalls(Id, null, null);
            InitializeComponent();
        }

        /// <summary>
        /// Property representing the current status of calls being displayed.
        /// Default value is "All".
        /// </summary>
        public BO.Statuses Status { get; set; } = BO.Statuses.All;

        /// <summary>
        /// Property representing the type of ambulance system being filtered.
        /// Default value is "All".
        /// </summary>
        public BO.SystemType Ambulance { get; set; } = BO.SystemType.All;

        /// <summary>
        /// Property representing the sorting field for the open calls list.
        /// Default value is "CallId".
        /// </summary>
        public BO.OpenCallInListField Field { get; set; } = BO.OpenCallInListField.CallId;

        /// <summary>
        /// ID of the volunteer.
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Property for the currently selected call in the list.
        /// Enables two-way binding for UI updates.
        /// </summary>
        public BO.OpenCallInList? SelectedCall
        {
            get { return (BO.OpenCallInList?)GetValue(SelectedCallProperty); }
            set { SetValue(SelectedCallProperty, value); }
        }

        /// <summary>
        /// DependencyProperty backing for SelectedCall, enabling binding and styling.
        /// </summary>
        public static readonly DependencyProperty SelectedCallProperty =
            DependencyProperty.Register("SelectedCall", typeof(BO.OpenCallInList), typeof(AssignmentWindow), new PropertyMetadata(null));

        /// <summary>
        /// Property representing the list of open calls.
        /// Enables two-way binding for UI updates.
        /// </summary>
        public IEnumerable<BO.OpenCallInList>? OpenCallList
        {
            get { return (IEnumerable<BO.OpenCallInList>)GetValue(OpenCallInListProperty); }
            set { SetValue(OpenCallInListProperty, value); }
        }

        /// <summary>
        /// DependencyProperty backing for OpenCallList, enabling binding and styling.
        /// </summary>
        public static readonly DependencyProperty OpenCallInListProperty =
            DependencyProperty.Register("OpenCallList", typeof(IEnumerable<BO.OpenCallInList>), typeof(AssignmentWindow), new PropertyMetadata(null));

        /// <summary>
        /// Event handler for the window's Loaded event.
        /// Adds the OpenCallListObserver to observe updates to the call list.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Call.AddObserver(OpenCallListObserver);

        /// <summary>
        /// Event handler for the window's Closed event.
        /// Removes the OpenCallListObserver when the window is closed.
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Call.RemoveObserver(OpenCallListObserver);

        /// <summary>
        /// Queries the BL layer to update the OpenCallList based on the current filter and sort settings.
        /// </summary>
        private void queryOpenCalInlList()
        {
            OpenCallList = (Ambulance == BO.SystemType.All) ?
            s_bl?.Call.SortOpenCalls(id, null, Field)! : s_bl?.Call.SortOpenCalls(id, Ambulance, Field)!;
        }

        /// <summary>
        /// Event handler for selection changes in the filter ComboBox.
        /// Updates the OpenCallList.
        /// </summary>
        private void FilteredCall_SelectionChanged(object sender, SelectionChangedEventArgs e) => queryOpenCalInlList();

        /// <summary>
        /// Event handler for selection changes in the sorting ComboBox.
        /// Updates the OpenCallList.
        /// </summary>
        private void SortedCall_SelectionChanged(object sender, SelectionChangedEventArgs e) => queryOpenCalInlList();

        /// <summary>
        /// Flag to ensure the observer works in a thread-safe manner.
        /// Prevents concurrent updates to the OpenCallList.
        /// </summary>
        private volatile bool _observerWorking = false;

        /// <summary>
        /// Observer method for handling updates to the OpenCallList.
        /// Invokes the queryOpenCalInlList method to refresh the list on the UI thread.
        /// </summary>
        private void OpenCallListObserver()
        {
            if (!_observerWorking)
            {
                _observerWorking = true;
                _ = Dispatcher.BeginInvoke(() =>
                {
                    queryOpenCalInlList();
                    _observerWorking = false;
                });
            }
        }

        /// <summary>
        /// Event handler for the "Take Action" button click.
        /// Assigns the selected call to the volunteer and notifies the user.
        /// </summary>
        private void btnChoose_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var call = button?.CommandParameter as BO.OpenCallInList;

            s_bl.Call.AssignCallToVolunteer(id, call!.CallId);

            // Notify the user
            MessageBox.Show($"You are now assigned to call {call.CallId}:{call.Description}.",
                                "Call Assigned", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }
    }
}
