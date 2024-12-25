using BO;
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


}
