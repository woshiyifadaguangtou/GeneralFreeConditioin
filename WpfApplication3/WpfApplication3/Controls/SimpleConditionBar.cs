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
using WpfApplication3.Resources;

namespace WpfApplication3.Controls
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:WpfApplication3.Controls"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:WpfApplication3.Controls;assembly=WpfApplication3.Controls"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误: 
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:SimpleConditionBar/>
    ///
    /// </summary>
    /// 
    [TemplatePartAttribute(Name = "cbbField", Type = typeof(ComboBox))]
    [TemplatePartAttribute(Name = "cbbOperator", Type = typeof(ComboBox))]
    [TemplatePartAttribute(Name = "cbbRelation", Type = typeof(ComboBox))]
    [TemplatePartAttribute(Name = "btnDelete", Type = typeof(Button))]
    [TemplatePartAttribute(Name = "txtValue", Type = typeof(TextBox))]
    public class SimpleConditionBar : ConditionBaseBar
    {
        ComboBox cbbField;
        ComboBox cbbOperator;
        ComboBox cbbRelation;
        Button btnDelete;
        TextBox txtValue;
        
        Condition condition;
        static SimpleConditionBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SimpleConditionBar), new FrameworkPropertyMetadata(typeof(SimpleConditionBar)));
        }

        public SimpleConditionBar(Type type)
        {
            targettype = type;
        }
        
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            cbbField = base.GetTemplateChild("cbbField") as ComboBox;
            cbbOperator = base.GetTemplateChild("cbbOperator") as ComboBox;
            cbbRelation = base.GetTemplateChild("cbbRelation") as ComboBox;
            btnDelete = base.GetTemplateChild("btnDelete") as Button;
            txtValue = base.GetTemplateChild("txtValue") as TextBox;

            btnDelete.Click += btnDelete_Click;
            Init();
        }
        void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            Delete();
        }
        public void Init()
        {
            #region 字段列表
            condition = new Condition();
            condition.Relation = "AND";
            Dictionary<string, string> Field = MakeDictionary(targettype,Resource1.ResourceManager);
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

            Binding OperatorBinding = new Binding()
            {
                Source = condition,
                Path = new PropertyPath("Operator"),
                Mode = BindingMode.TwoWay,

            };

            this.cbbOperator.SetBinding(ComboBox.SelectedValueProperty, OperatorBinding);
            #endregion

            #region 关系操作
            Dictionary<string, string> relations = MakeDictionary<Relation>(RelationResourceresx.ResourceManager);
            this.cbbRelation.ItemsSource = relations;
            this.cbbRelation.DisplayMemberPath = "Key";
            this.cbbRelation.SelectedValuePath = "Value";
            Binding RelationBinding = new Binding()
            {
                Source = condition,
                Path = new PropertyPath("Relation"),
                Mode = BindingMode.TwoWay,
            };
            this.cbbRelation.SetBinding(ComboBox.SelectedValueProperty, RelationBinding);
            #endregion

            #region 值操作
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
                result.Add(propertyName, property.Name);
            }
            return result;
        }

        public override System.Linq.Expressions.Expression GetExpression(System.Linq.Expressions.ParameterExpression parameter)
        {
            return Condition.ConditionToExpression(targettype, condition, parameter);
        }

        public override string GetRelation()
        {
            return condition.Relation;
        }
    }
}
