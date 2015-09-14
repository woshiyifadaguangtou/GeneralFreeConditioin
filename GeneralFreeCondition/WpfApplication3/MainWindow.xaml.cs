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
            Condition.Initparameter<Customer>();
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

             customers.Where(e => 1 == 1);
        }

        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
           // TestQuery();

            Query();
        }


        private void Query()
        {
            Queue<Condition> conditionQueue = new Queue<Condition>();
            conditionList.Clear();
            foreach (var uc in spDisplay.Children)
            {
                var c = ((ConditionBaseBar)uc);
                c.ConditionList.First().Relation = c.GetRelation();
                conditionList.AddRange(c.ConditionList);
            }
            foreach (var c in this.conditionList)
            {
                conditionQueue.Enqueue(c);
                Debug.WriteLine(c.Field + " " + c.Operator + " " + c.Value + " " + c.Relation+ " "+c.Level);
                
            }
            try
            {
                var expression = Condition.GetExpression<Customer>(conditionQueue);
                System.Linq.Expressions.Expression<Func<Customer, bool>> lambda = System.Linq.Expressions.Expression.Lambda<Func<Customer, bool>>(expression, Condition.parameter);
                var where = lambda.Compile();
                var result = customers.Where(where).ToList() ;
            }
            catch (Exception ex)
            {
 
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Createuc();

            Createscb();
            
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
