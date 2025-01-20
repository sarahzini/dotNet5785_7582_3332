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

namespace PL.Volunteer;

/// <summary>
/// Interaction logic for CurrentCall.xaml
/// </summary>
public partial class CurrentCall : Window
{
    public CurrentCall(BO.CallInProgress c)
    {
        InitializeComponent();
        Current = c;
    }
    public BO.CallInProgress Current
    {
        get { return (BO.CallInProgress)GetValue(CurrentProperty); }
        set { SetValue(CurrentProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Current.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CurrentProperty =
        DependencyProperty.Register("Current", typeof(BO.CallInProgress), typeof(CurrentCall), new PropertyMetadata(null));
}
