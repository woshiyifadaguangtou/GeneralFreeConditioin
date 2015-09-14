using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Linq.Expressions;
using System.Windows.Input;
using System.Windows.Media;


namespace WpfApplication3
{
    public abstract class ConditionBaseBar :Control
    {

        public abstract List<Condition> ConditionList
        {
            get;
        }
        
        public Expression ConditionExpreesion
        {
            get { return conditionExpreesion; }
        }

        protected void Delete()
        {
            var parent = VisualTreeHelper.GetParent(this) as StackPanel;
            if (parent != null)
            {
                parent.Children.Remove(this);
            }
        }

        protected Expression conditionExpreesion;
        protected string conditonRelation;

        public ConditionBaseBar()
        {
            
        }

        public Type targettype
        {
            get;
            set;
        }

        public abstract Expression GetExpression(ParameterExpression parameter);
        public abstract string GetRelation();
    }
}
