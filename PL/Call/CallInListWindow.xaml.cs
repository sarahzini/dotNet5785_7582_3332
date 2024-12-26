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
    public CallInListWindow() => InitializeComponent();
    public BO.Statuses Status { get; set; } = BO.Statuses.All;

    public IEnumerable<BO.CallInList> CallList
    {
        get { return (IEnumerable<BO.CallInList>)GetValue(CallListProperty); }
        set { SetValue(CallListProperty, value); }
    }

    public static readonly DependencyProperty CallListProperty =
        DependencyProperty.Register("CourseList", typeof(IEnumerable<BO.CallInList>), typeof(CallInListWindow), new PropertyMetadata(null));

    private void FilteredCall_SelectionChanged(object sender, SelectionChangedEventArgs e) => queryCallList();

    private void queryCallList()
         =>CallList = (Status == BO.Statuses.All) ?
                   s_bl?.Call.GetSortedCallsInList()! : s_bl?.Call.GetSortedCallsInList(BO.CallInListField.Status, Status,null)!;
    
    private void callListObserver()
        => queryCallList();
 
    private void Window_Loaded(object sender, RoutedEventArgs e)
    => s_bl.Call.AddObserver(callListObserver);

    private void Window_Closed(object sender, EventArgs e)
        => s_bl.Call.RemoveObserver(callListObserver);

    public BO.CallInList? SelectedCall { get; set; }

    private void lsvUpdate_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (SelectedCall != null)
            new CallWindow("Update", SelectedCall.CallId).Show();
    }
    private void btnAdd_Click(object sender, RoutedEventArgs e)
    {
        new CallWindow("Add", 0).Show();

    }

    private void btnDelete_Click(object sender, RoutedEventArgs e)
    {
        MessageBoxResult confirmation = MessageBox.Show("Are you sure you want to delete this call ?", "Delete Confirmation",
                                                    MessageBoxButton.YesNo, MessageBoxImage.Question);
        try
        {
            if (confirmation == MessageBoxResult.Yes)
            {
                s_bl.Volunteer.DeleteVolunteer(SelectedCall.CallId);
                queryCallList();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

    }
}
