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
/// Interaction logic for CallAssignmentWindow.xaml
/// </summary>
public partial class CallAssignmentWindow : Window
{
    public CallAssignmentWindow(IEnumerable<BO.CallAssignInList> c)
    {
        InitializeComponent();
        CallAssign = c;
    }
    public IEnumerable<CallAssignInList>? CallAssign
    {
        get { return (IEnumerable<CallAssignInList>)GetValue(CallAssignProperty); }
        set { SetValue(CallAssignProperty, value); }
    }

    // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CallAssignProperty =
        DependencyProperty.Register("CallAssign", typeof(IEnumerable<CallAssignInList>), typeof(CallAssignmentWindow), new PropertyMetadata(null));
}

