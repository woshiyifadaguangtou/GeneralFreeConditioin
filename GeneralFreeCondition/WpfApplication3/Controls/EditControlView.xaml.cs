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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApplication3.Controls;

namespace WpfApplication3
{
    /// <summary>
    /// EditControlView.xaml 的交互逻辑
    /// </summary>
    public partial class EditControlView : UserControl
    {
        System.Linq.Expressions.Expression expression;

        Type targetType;
        public EditControlView()
        {
            InitializeComponent();
        }

        private void btnSimpleCondition_Click(object sender, RoutedEventArgs e)
        {
            SimpleConditionBar scb = new SimpleConditionBar(typeof(Customer),0);
            this.spDisplay.Children.Add(scb);
        }

        private void btnComplexCondition_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
