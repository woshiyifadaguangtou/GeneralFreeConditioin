using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
using WpfApplication3.Resources;

namespace WpfApplication3
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        Condition condition;
        public Action Delete;
        public Condition Condition 
        {
            get { return condition; }
        }
        public UserControl1()
        {
            condition = new Condition();
            InitializeComponent();
            this.DataContext = this;
        }

        public UserControl1(Type type)
            : this()
        {
            target = type;
           Init();
        }
        Type target;
        public void Init()
        {
            #region 字段列表

            Dictionary<string, string> Field = MakeDictionary(target, Resource1.ResourceManager);
            this.cbbField.ItemsSource = Field;
            this.cbbField.DisplayMemberPath = "Key";
            this.cbbField.SelectedValuePath = "Value";

            Binding SelectValue = new Binding()
            {
                Source = condition,
                Mode = BindingMode.TwoWay,
                Path = new PropertyPath("Field"),
            };
            
            this.cbbField.SetBinding(ComboBox.SelectedValueProperty, SelectValue);
            
            #endregion

            #region 比较操作
            Dictionary<string, string> Operatos = MakeDictionary<ConditionOperators>(ConditionOperatorResource.ResourceManager);
            this.cbbOperator.ItemsSource = Operatos;
            this.cbbOperator.DisplayMemberPath = "Key";
            this.cbbOperator.SelectedValuePath = "Key";

            Binding OperatorBinding = new Binding ()
            {
                Source = condition,
                Path = new PropertyPath ("Operator"),
                Mode = BindingMode.TwoWay,

            };

            this.cbbOperator.SetBinding(ComboBox.SelectedValueProperty,OperatorBinding);
            #endregion

            #region 关系操作
            Dictionary<string, string> relations = MakeDictionary<Relation>(RelationResourceresx.ResourceManager);
            this.cbbRelation.ItemsSource = relations;
            this.cbbRelation.DisplayMemberPath = "Key";
            this.cbbRelation.SelectedValuePath = "Value";
            Binding RelationBinding = new Binding ()
            {
                Source = condition,
                Path = new PropertyPath ("Relation"),
                Mode = BindingMode.TwoWay,
            };
            this.cbbRelation.SetBinding(ComboBox.SelectedValueProperty,RelationBinding);
            #endregion 

            #region
            Binding txtbinding = new Binding()
            {
                Mode = BindingMode.TwoWay,
                Source = condition,
                Path = new PropertyPath("Value"),
            };
            txtValue.SetBinding(TextBox.TextProperty, txtbinding);
            #endregion
        }

        private Dictionary<string, string> MakeDictionary<T>(System.Resources.ResourceManager resourceManager)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            Type type = typeof(T);
            dynamic conditions;
            if (type.IsEnum)
            {
                conditions = Enum.GetNames(typeof(T));
                foreach (var condition in conditions)
                {
                    string conditionName = resourceManager.GetString(condition);
                    result.Add(conditionName, condition);
                }
            }
            else
            {
                conditions = type.GetProperties();
                foreach (var condition in conditions)
                {
                    string conditionName = resourceManager.GetString(condition.Name);
                    result.Add(conditionName, condition.Name);
                }
                
            }
            return result;
        }

        private Dictionary<string, string> MakeDictionary(Type type, System.Resources.ResourceManager resourceManager)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (var property in type.GetProperties())
            {
                string propertyName = resourceManager.GetString(property.Name);
                result.Add(propertyName,property.Name);
            }
            return result;
        }
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (Delete != null)
            {
                Delete();
            }
        }

        public System.Linq.Expressions.Expression GetExpression<T>()
        {
                return Condition.ConditionToExpression<T>(condition, null);
        }
    }
}
