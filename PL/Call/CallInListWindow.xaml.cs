using BO;
using PL.Volunteer;
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

namespace PL.Call;

/// <summary>
/// Interaction logic for CallInProgressWindow.xaml
/// </summary>
public partial class CallInListWindow : Window
{
    /// To gain access to the BL layer, we need to use the Factory class.
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public CallInListWindow(int id) {
        InitializeComponent();
        requesterId = id;
    }
    public BO.CallInListField Sort { get; set; } = BO.CallInListField.CallId;
    public BO.Statuses Filter { get; set; } = BO.Statuses.All;


    /// <summary>
    /// This method returns the value of the CallList (with IEnumerable).
    /// </summary>
    public IEnumerable<BO.CallInList> CallList
    {
        get { return (IEnumerable<BO.CallInList>)GetValue(CallListProperty); }
        set { SetValue(CallListProperty, value); }
    }

    /// <summary>
    /// This method returns the value of the CallList (with DependencyProperty).
    /// </summary>
    public static readonly DependencyProperty CallListProperty =
        DependencyProperty.Register("CallList", typeof(IEnumerable<BO.CallInList>), typeof(CallInListWindow), new PropertyMetadata(null));

    /// <summary>
    /// This method filters the call list based on the selected status.
    /// </summary>
    private void FilteredCall_SelectionChanged(object sender, SelectionChangedEventArgs e) => queryCallList();

    /// <summary>
    /// This method queries the call list based on the selected status.
    /// </summary>
    private void queryCallList()
         =>CallList = (Filter == BO.Statuses.All) ?
                   s_bl?.Call.GetSortedCallsInList(null,null,Sort)! : s_bl?.Call.GetSortedCallsInList(BO.CallInListField.Status, Filter,Sort)!;

    /// <summary>
    /// This method calls the call list observer.
    /// </summary>

    private volatile bool _observerWorking = false; //stage 7

    private void callListObserver()
    {
        if (!_observerWorking)
        {
            _observerWorking = true;
            _ = Dispatcher.BeginInvoke(() =>
            {
                queryCallList();
                _observerWorking = false;
            });
        }
    }


    /// <summary>
    /// This method loads the call list window.
    /// </summary>
    private void Window_Loaded(object sender, RoutedEventArgs e)
    => s_bl.Call.AddObserver(callListObserver);

    /// <summary>
    /// This method closes the call list window.
    /// </summary>
    private void Window_Closed(object sender, EventArgs e)
        => s_bl.Call.RemoveObserver(callListObserver);

    /// <summary>
    /// This method gets the selected call.
    /// </summary>
    

    public BO.CallInList SelectedCall
    {
        get { return (BO.CallInList)GetValue(SelectedCallProperty); }
        set { SetValue(SelectedCallProperty, value); }
    }

    // Using a DependencyProperty as the backing store for SelectedCall.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty SelectedCallProperty =
        DependencyProperty.Register("SelectedCall", typeof(BO.CallInList), typeof(CallInListWindow), new PropertyMetadata(null));



    /// <summary>
    /// This method opens the call window for updating the selected call.
    /// </summary>
    private void lsvUpdate_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (SelectedCall != null)
            new UpdateCallWindow(SelectedCall.CallId).Show();
    }

    /// <summary>
    /// This method opens the call window for adding a new call.
    /// </summary>
    private void btnAdd_Click(object sender, RoutedEventArgs e)
    {
        new AddCallWindow().Show();

    }

    /// <summary>
    /// This method deletes the selected call.
    /// </summary>
   private void btnDelete_Click(object sender, RoutedEventArgs e)
    {
        MessageBoxResult confirmation = MessageBox.Show("Are you sure you want to delete this call ?", "Delete Confirmation",
                                                    MessageBoxButton.YesNo, MessageBoxImage.Question);
        try
        {
            if (confirmation == MessageBoxResult.Yes)
            {
                s_bl.Call.DeleteCall(SelectedCall.CallId);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

    }

    public int requesterId { get; set; }
    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        MessageBoxResult confirmation = MessageBox.Show("Are you sure you want to cancel the actual assignment to this call ?", "Cancel Confirmation",
                                                    MessageBoxButton.YesNo, MessageBoxImage.Question);
        try
        {
            if (confirmation == MessageBoxResult.Yes)
            {
                s_bl.Call.CancelAssignment(requesterId,(SelectedCall.AssignId));
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

    }

}
