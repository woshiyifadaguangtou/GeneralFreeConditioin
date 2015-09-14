using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
         System.Linq.Expressions.ParameterExpression parameter;
         public System.Linq.Expressions.ParameterExpression Parameter
         {
             get { return parameter; }
             
         }
        public MainWindow()
        {
            InitializeComponent();
            parameter = System.Linq.Expressions.Expression.Parameter(typeof(Customer));
            InitDate();
        }
        List<Customer> customers;
        List<Condition> conditionList;
        public void InitDate()
        {
            customers = new List<Customer>()
            {
                new Customer(){Name = "张三",Age =20,Sex = "男",Income =2000,Level =1},
                new Customer(){Name = "张四",Age =21,Sex = "男",Income =3000,Level =2},
                new Customer(){Name = "张五",Age =22,Sex = "女",Income =4000,Level =1},
                new Customer(){Name = "张六",Age =23,Sex = "女",Income =5000,Level =1},
                new Customer(){Name = "张七",Age =24,Sex = "男",Income =6000,Level =1},
            };
             conditionList = new List<Condition>
            {
                //new Condition (){Field = "Sex",Operator="like",Value = "男",Relation="AND"},
                //new Condition (){Field = "Age",Operator=">=",Value = "21",Relation="AND"},
                //new Condition (){Field = "Level",Operator="=",Value = "1",Relation="AND"},
                //new Condition (){Field = "Income",Operator=">=",Value = "5000",Relation="AND"},
            };

            
        }

        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
           // TestQuery();

            Query();
        }

        private void TestQuery()
        {
            foreach (var uc in spDisplay.Children)
            {
                conditionList.Add(((UserControl1)uc).Condition);
            }
            var where = Condition.Match<Customer>(conditionList, customers);
            var result = customers.Where(where);
        }

        private void Query()
        {
            conditionList.Clear();
            foreach (var uc in spDisplay.Children)
            {
                var c = ((ConditionBaseBar)uc);
                conditionList.AddRange(c.ConditionList);
            }
            foreach (var c in this.conditionList)
            {
                Debug.WriteLine(c.Field + " " + c.Operator + " " + c.Value + " " + c.Relation+ ""+c.Level);
            }
        }
        private void QueryExpression()
        {
            Dictionary<System.Linq.Expressions.Expression, string> expressions = new Dictionary<System.Linq.Expressions.Expression, string>();
            System.Linq.Expressions.ParameterExpression parameter = System.Linq.Expressions.Expression.Parameter(typeof(Customer), "r");
            foreach (var uc in spDisplay.Children)
            {
                var c = ((ConditionBaseBar)uc);
                var ConditionExpreesion = c.GetExpression(Parameter);
                var ConditionRelation = c.GetRelation();
                expressions.Add(ConditionExpreesion, ConditionRelation);
            }

            var where = Condition.Match<Customer>(expressions, Parameter);

            
            var result = customers.Where(where);
            var rlist = result.ToList();
            foreach (var r in rlist) 
            {
                Debug.WriteLine(r.ToString());
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Createuc();

            Createscb();
            
        }

        private void Createuc()
        {
            UserControl1 uc = new UserControl1(typeof(Customer));
            uc.Delete = () =>
            {
                this.spDisplay.Children.Remove(uc);
            };
            this.spDisplay.Children.Add(uc);
        }

        private void Createscb()
        {
            SimpleConditionBar scb = new SimpleConditionBar(typeof(Customer),0);
            this.spDisplay.Children.Add(scb);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ComplexBar cb = new ComplexBar(Parameter,typeof(Customer),0);
            this.spDisplay.Children.Add(cb);
        }

        
    }
}
