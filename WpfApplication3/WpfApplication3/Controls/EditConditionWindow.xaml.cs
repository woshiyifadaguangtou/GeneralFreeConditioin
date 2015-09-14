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

namespace WpfApplication3.Controls
{
    /// <summary>
    /// EditConditionWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditConditionWindow : Window
    {
        System.Linq.Expressions.ParameterExpression parameter;
        System.Linq.Expressions.Expression expression;

        public delegate void ReturnExpression(System.Linq.Expressions.Expression expression);
        public event ReturnExpression ReturnExpressionEvent;

        Type TargetType;
        int level;
        public System.Linq.Expressions.Expression Expression
        {
            get { return Expression; }
        }
        public EditConditionWindow()
        {
            InitializeComponent();
        }

        public EditConditionWindow(Type type):this()
        {
            TargetType = type;
        }

        public EditConditionWindow(System.Linq.Expressions.ParameterExpression inparameter,Type type,int Level)
            : this(type)
        {
            parameter = inparameter;
            level = Level;
            this.Title = String.Format("{0}阶表达式", level);
        }

        public EditConditionWindow(System.Linq.Expressions.ParameterExpression inparameter, Type type, int Level,System.Linq.Expressions.Expression expression)
            : this(inparameter, type,Level)
        {
            
            
        }

        private void btnSimpleCondition_Click(object sender, RoutedEventArgs e)
        {
            Createscb();
        }

        private void btnComplexCondition_Click(object sender, RoutedEventArgs e)
        {
            Createccb();
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Query();
                if (ReturnExpressionEvent != null)
                {
                    ReturnExpressionEvent(expression);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(Convert.ToString(ex));
            }
            this.Close();
        }

        /// <summary>
        /// 简单条件
        /// </summary>
        private void Createscb()
        {
            SimpleConditionBar scb = new SimpleConditionBar(TargetType);
            this.spDisplay.Children.Add(scb);
        }
        /// <summary>
        /// 复杂条件
        /// </summary>
        private void Createccb()
        {
            ComplexBar cb = new ComplexBar(parameter,TargetType,level);
            this.spDisplay.Children.Add(cb);
       
        }
        private void Query() 
        {
            Dictionary<System.Linq.Expressions.Expression, string> expressions = new Dictionary<System.Linq.Expressions.Expression, string>();
            foreach (var uc in spDisplay.Children)
            {
                var c = ((ConditionBaseBar)uc);
                var ConditionExpreesion = c.GetExpression(parameter);
                var ConditionRelation = c.GetRelation();
                expressions.Add(ConditionExpreesion, ConditionRelation);
            }

            expression = Condition.CombineExpression(expressions, parameter);
            
        }
    }
}
