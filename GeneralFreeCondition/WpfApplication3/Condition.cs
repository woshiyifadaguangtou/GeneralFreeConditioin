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
        /// 

        /// 字段  
        /// 

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
        public static Condition[] BuildConditions(string[] fileds, string[] operators, string[] values, string[] relations)
        {
            if (fileds == null || operators == null || values == null || relations == null)
            {
                return null;
            }
            Condition[] conditions = new Condition[fileds.Length];
            try
            {
                for (int i = 0; i < conditions.Length; i++)
                {
                    conditions[i] = new Condition()
                    {
                        Field = fileds[i],
                        Operator = operators[i],
                        Value = values[i],
                        Relation = relations[i]
                    };
                }
            }
            catch
            {
                return null;
            }
            return conditions;
        }

       

        public static Expression ConditionToExpression<T>(Condition condition, Expression parameter)
        {
            if (parameter == null)
            {
                parameter = Expression.Parameter(typeof(T), "r");
            }
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
        public static Func<T,bool> Match<T>(List<Condition> conditions,List<T> resource)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T),"r");
            
            Expression body = null;
            foreach (var condition in conditions)
            {
                if (body == null)
                {
                    body = ConditionToExpression<T>(condition, parameter);
                }
                else
                {
                    Expression right = ConditionToExpression<T>(condition, parameter);
                    body = condition.Relation.ToUpper().Equals("AND") ? Expression.And(body, right) : Expression.Or(body, right);
                }
            
            }
            Expression<Func<T, bool>> lambda = Expression.Lambda<Func<T, bool>>(body, parameter);
            Expression<Func<Customer, bool>> lambda1 = e => e.Age > 20 & e.Income < 4000 & e.Level == 1;
            var where = lambda.Compile();
            Debug.WriteLine(body);
            Debug.WriteLine(lambda);
            Debug.WriteLine(where);
            //var result =  resource.Where()
            return where;
        }

        public static Func<T, bool> Match<T>(Dictionary<Expression, string> conditions, ParameterExpression parameter)
        {
            
            Expression result = null;
            conditions = OrderConditions(conditions);
            foreach (var conditon in conditions)
            {
                if (result == null)
                {
                    result = conditon.Key;
                }
                else
                {
                    result = conditon.Value.Equals("AND") ? Expression.And(result, conditon.Key) : Expression.Or(result, conditon.Key);
                
                   // result = conditon.Value.Equals("AND") ? Expression.AndAlso(result, conditon.Key) : Expression.OrElse(result, conditon.Key);
                }
            }
            Expression<Func<T, bool>> lambda = Expression.Lambda<Func<T, bool>>(result, parameter);
            
            if (lambda.CanReduce)
            {
                lambda.Reduce();
            }
            var list = AnalysisExpression(result,"Begin");
           
            var where = lambda.Compile();
            Debug.WriteLine(lambda.ToString());
            return where;
        }

        public static Expression CombineExpression(Dictionary<Expression, string> conditions, ParameterExpression parameter)
        {
            Expression result = null;
            foreach (var conditon in conditions)
            {
                if (result == null)
                {
                    result = conditon.Key;
                }
                else
                {
                    result = conditon.Value.Equals("AND") ? Expression.And(result, conditon.Key) : Expression.Or(result, conditon.Key);
                }
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
    } 


}
