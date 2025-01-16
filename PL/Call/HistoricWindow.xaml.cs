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

namespace PL
{
    /// <summary>
    /// Interaction logic for HistoricWindow.xaml
    /// </summary>
    public partial class HistoricWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public HistoricWindow(int id)
        {
            InitializeComponent();
            ClosedCalls = s_bl.Call.SortClosedCalls(id,null,null);
            volunteerId = id;
        }

        public int volunteerId { get; set; }
        private void FilteredCall_SelectionChanged(object sender, SelectionChangedEventArgs e) => queryClosedCallsListFilter();

        private void SortedCall_SelectionChanged(object sender, SelectionChangedEventArgs e) => queryClosedCallsListFilter();


        public BO.ClosedCallInList? SelectedCall { get; set; }

        public IEnumerable<ClosedCallInList>? ClosedCalls
        {
            get { return (IEnumerable<ClosedCallInList>)GetValue(ClosedCallsProperty); }
            set { SetValue(ClosedCallsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClosedCallsProperty =
            DependencyProperty.Register(nameof(ClosedCalls), typeof(IEnumerable<ClosedCallInList>), typeof(HistoricWindow), new PropertyMetadata(null));

        public BO.SystemType Ambulance { get; set; } = BO.SystemType.All;

        public BO.ClosedCallInListField Field { get; set; } = BO.ClosedCallInListField.CallId;
        

        private void queryClosedCallsListFilter()
             => ClosedCalls = (Ambulance == BO.SystemType.All) ?
                       s_bl?.Call.SortClosedCalls(volunteerId, null, Field)! : s_bl?.Call.SortClosedCalls(volunteerId, Ambulance, Field)!; 

    }
}
