using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace WpfApplication3
{
    public class Condition
    {
        public static ParameterExpression parameter ;
        public bool IsComplex { get; set; }
        public Expression expression { get; set; }
        /// 

        /// 字段  
        /// 
        public static void Initparameter<T>()
        {
            parameter = Expression.Parameter(typeof(T), "r");
        }

        public string Field { get; set; }
        /// 

        /// 表达式  
        /// 

        public string Operator { get; set; }
        /// 

        /// 值  
        /// 

        public string Value { get; set; }
        /// 

        /// 关系  
        /// 

        public string Relation { get; set; }

        /// 

        ///   
        /// 
        public int Level { get; set; }
        ///   
        ///   
        ///   
        ///   
        ///   
       

       

        public static Expression ConditionToExpression<T>(Condition condition)
        {
            
            Expression result =null;
            Type type = typeof(T);
            PropertyInfo lpi = type.GetProperty(condition.Field);
            Expression left = Expression.Property(parameter, lpi);

            object value = Convert.ChangeType(condition.Value, lpi.PropertyType);
            Expression right = Expression.Constant(value);

            switch (condition.Operator)
            {
                case "=":
                    result = Expression.Equal(left, right);
                    break;
                case "<":
                    result = Expression.LessThan(left, right);
                    break;
                case "<=":
                    result = Expression.LessThanOrEqual(left, right);
                    break;
                case ">":
                    result = Expression.GreaterThan(left, right);
                    break;
                case ">=":
                    result = Expression.GreaterThanOrEqual(left, right);
                    break;
                case "like":
                    result = Expression.Call(left,typeof(string).GetMethod("Contains",new Type[]{typeof(string)}),right);
                    break;
            }

            return result;
        }
        public static Expression ConditionToExpression(Type type,Condition condition, Expression parameter)
        {
            Expression result = null;
            
            PropertyInfo lpi = type.GetProperty(condition.Field);
            Expression left = Expression.Property(parameter, lpi);

            object value = Convert.ChangeType(condition.Value, lpi.PropertyType);
            Expression right = Expression.Constant(value);

            switch (condition.Operator)
            {
                case "=":
                    result = Expression.Equal(left, right);
                    break;
                case "<":
                    result = Expression.LessThan(left, right);
                    break;
                case "<=":
                    result = Expression.LessThanOrEqual(left, right);
                    break;
                case ">":
                    result = Expression.GreaterThan(left, right);
                    break;
                case ">=":
                    result = Expression.GreaterThanOrEqual(left, right);
                    break;
                case "like":
                    result = Expression.Call(left, typeof(string).GetMethod("Contains", new Type[] { typeof(string) }), right);
                    break;
            }

            return result;
        }
      

       

      

        public static Dictionary<Expression,string> AnalysisExpression(Expression resource,string Relation)
        {
            Dictionary<Expression, string> result = new Dictionary<Expression, string>();
            var br = resource as BinaryExpression;
            if (br != null && (resource.NodeType == ExpressionType.AndAlso || resource.NodeType == ExpressionType.OrElse || resource.NodeType == ExpressionType.And || resource.NodeType == ExpressionType.Or))
            {
                string ChildRelation = resource.NodeType.ToString();
                result.Concat(AnalysisExpression(br.Left, ChildRelation));
                result.Concat(AnalysisExpression(br.Right, ChildRelation));
                
            }
            else 
            {
                result.Add(resource, Relation);

            }
            return result;
            
        }


        public static Dictionary<Expression,string> OrderConditions(Dictionary<Expression, string> conditions)
        {
            Dictionary<Expression, string> result = new Dictionary<Expression, string>();
            GetConditions(result,conditions, "AND");
            GetConditions(result,conditions, "OR");
            return result;
        }

        public static void GetConditions(Dictionary<Expression, string> result,Dictionary<Expression, string> conditions, string Relation)
        {
            foreach (var condition in conditions.Where(e => e.Value.Equals(Relation)))
            {
                result.Add(condition.Key, condition.Value);
            }
            return;
        }


        public static Expression GetExpression<T>(Queue<Condition> conditionList)
        {
            var c = conditionList.Last();
            //如果最后一个元素不是0级表达式 ,那么将导致循环不完整 于是添加0阶恒F表达式做或运算
            if (c.Level != 0)
            {
                ConstantExpression left = Expression.Constant(1, typeof(int));
                ConstantExpression right = Expression.Constant(2, typeof(int));
                Condition lastCondition = new Condition()
                {
                    Relation = "OR",
                    IsComplex = true,
                    Level = 0,
                    expression = Expression.Equal(left, right),
                };
                conditionList.Enqueue(lastCondition);
            }

            int CurrentLevel = 0;
            Stack<Condition> conditionStack = new Stack<Condition>();
            Stack<string> relationStack = new Stack<string>();
            Stack<Condition> expressionStack = new Stack<Condition>();
            while(conditionList.Count != 0)
            {
                var condition = conditionList.Peek();
                #region 入栈
                if (condition.Level >= CurrentLevel)
                {
                    
                    conditionStack.Push(conditionList.Dequeue());
                    CurrentLevel = condition.Level;
                }

                #endregion

                #region 出栈
                else
                {
                    Stack<Condition> CurrentConditionList = new Stack<Condition>();
                    Condition newCondition = new Condition ();
                    string relation = conditionStack.Peek().Relation;
                    while(conditionStack.Peek().Level == CurrentLevel)
                    {
                        relation = conditionStack.Peek().Relation;
                        CurrentConditionList.Push(conditionStack.Pop());
                    }

                    Condition resultCondition = Calculate<T>(CurrentConditionList, CurrentLevel);

                    conditionStack.Push(resultCondition);
                    CurrentLevel--;
                }

                #endregion 
            }
            if (conditionStack.Count == 1)
                return conditionStack.First().expression;
            else 
            {
                Stack<Condition> reStack = new Stack<Condition> ();
                while(conditionStack.Count != 0)
                {
                    reStack.Push(conditionStack.Pop());
                }
                return Calculate<T>(reStack, 0).expression;
            }
        }

        private static Condition Calculate<T>(Stack<Condition> CurrentConditionList,int level)
        {
            Stack<Condition> TempCondition = new Stack<Condition>();
            TempCondition.Push(CurrentConditionList.Pop());
            //与最外层的运算符
            string Reltion = TempCondition.Peek().Relation;

            while(CurrentConditionList.Count != 0)
            {
                var nextCondition = CurrentConditionList.Pop();
                if (nextCondition.Relation == "AND")//先计算AND操作
                {
                    var CurrentCondition = TempCondition.Pop();
                    var newexpresstion = TransForm<T>(CurrentCondition, nextCondition, "AND");
                    Condition newCondition = new Condition()
                    {
                        Relation = CurrentCondition.Relation,
                        expression = newexpresstion,
                        Level = level,
                        IsComplex = true
                    };
                    TempCondition.Push(newCondition);
                }
                else
                {
                    TempCondition.Push(nextCondition);
                }
            };
            #region 接下来都是OR的操作
            
            while(TempCondition.Count >1)
            {
                var current = TempCondition.Pop();
                var next = TempCondition.Pop();
                Condition newCondition = new Condition()
                {
                    expression = TransForm<T>(current,next,current.Relation),
                    Relation = next.Relation,
                    Level = level,
                    IsComplex = true,
                };
                TempCondition.Push(newCondition);
            }

            var result = TempCondition.First();
            result.Level--;
            #endregion
            return result;
        }

        private static Expression TransForm<T>(Condition left,Condition right,string Relation)
        {
            if (left.expression == null && left.IsComplex == false)
            {
                left.expression = ConditionToExpression<T>(left);
            }
            if (right.expression == null && right.IsComplex == false)
            {
                right.expression = ConditionToExpression<T>(right);
            }
            var result = Relation.Equals("AND") ? Expression.And(left.expression, right.expression) : Expression.Or(left.expression, right.expression);
            return result;
        }

        
    } 


}
