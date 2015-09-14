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
using WpfApplication3.Resources;

namespace WpfApplication3
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:WpfApplication3"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:WpfApplication3;assembly=WpfApplication3"
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
    ///     <MyNamespace:ComplexBar/>
    ///
    /// </summary>
    [TemplatePartAttribute(Name = "btnComplexCondition", Type = typeof(Button))]
    [TemplatePartAttribute(Name = "btnDeleteCondition", Type = typeof(Button))]
    [TemplatePartAttribute(Name = "cbbRelation", Type = typeof(ComboBox))]
    [TemplatePartAttribute(Name = "txtComplexCondition" ,Type = typeof(TextBlock))]
    public class ComplexBar :ConditionBaseBar
    {
        Button btnComplexCondition;
        Button btnDeleteCondition;
        ComboBox cbbRelation;
        Condition condition;
        TextBlock txtComplexCondition;
        int level;

        List<Condition> conditionList;

        public override List<Condition> ConditionList
        {
            get { return conditionList; }
        }


        System.Linq.Expressions.ParameterExpression parameterExpression;
        static ComplexBar()
        {
            
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ComplexBar), new FrameworkPropertyMetadata(typeof(ComplexBar)));
        }

        public ComplexBar(System.Linq.Expressions.ParameterExpression p,Type type,int lv)
        {
            conditionList = new List<Condition>();
            parameterExpression = p;
            targettype = type;
            level = lv;
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            btnComplexCondition = base.GetTemplateChild("btnComplexCondition") as Button;
            btnDeleteCondition = base.GetTemplateChild("btnDeleteCondition") as Button;
            cbbRelation = base.GetTemplateChild("cbbRelation") as ComboBox;
            txtComplexCondition = base.GetTemplateChild("txtComplexCondition") as TextBlock;
            btnComplexCondition.Click += btnComplexCondition_Click;
            btnDeleteCondition.Click += btnDeleteCondition_Click;

            btnComplexCondition.Content = "编辑";
            btnDeleteCondition.Content = "删除";

            condition = new Condition();
            condition.Relation = "AND";
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
        void btnDeleteCondition_Click(object sender, RoutedEventArgs e)
        {
            Delete();
        }

        void btnComplexCondition_Click(object sender, RoutedEventArgs e)
        {
            if (conditionExpreesion != null)
            { 
            }
            EditConditionWindow ecw = new EditConditionWindow(parameterExpression,targettype,level+1);
            ecw.ReturnExpressionEvent += ecw_ReturnExpressionEvent;
            ecw.ShowDialog();
           // conditionExpreesion = ecw.Expression;
        }

        void ecw_ReturnExpressionEvent(List<Condition> conditionList)
        {
            this.ConditionList.AddRange(conditionList);
        }

        
        
        public override string GetRelation()
        {
            return condition.Relation;
        }

        public override System.Linq.Expressions.Expression GetExpression(System.Linq.Expressions.ParameterExpression parameter)
        {
            return ConditionExpreesion;
        }
    }
}
