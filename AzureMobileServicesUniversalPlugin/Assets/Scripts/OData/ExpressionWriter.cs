// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpressionWriter.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ExpressionWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Provider
{
    using System;
    using System.Collections.Generic;
    ////using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Linq2Rest.Provider.Writers;

    public static class StringExtensions
    {
        public static bool IsNullOrWhiteSpace(string value)
        {
            if (value == null) return true;
            return string.IsNullOrEmpty(value.Trim());
        }
    }

    internal class ExpressionWriter : IExpressionWriter
    {
        private static readonly ExpressionType[] CompositeExpressionTypes = new[] { ExpressionType.Or, ExpressionType.OrElse, ExpressionType.And, ExpressionType.AndAlso };
        private readonly IMethodCallWriter[] _methodCallWriters = new IMethodCallWriter[]
																{
																	new EqualsMethodWriter(), 
																	new StringReplaceMethodWriter(), 
																	new StringTrimMethodWriter(), 
																	new StringToLowerMethodWriter(), 
																	new StringToUpperMethodWriter(), 
																	new StringSubstringMethodWriter(), 
																	new StringContainsMethodWriter(), 
																	new StringIndexOfMethodWriter(), 
																	new StringEndsWithMethodWriter(), 
																	new StringStartsWithMethodWriter(), 
																	new MathRoundMethodWriter(), 
																	new MathFloorMethodWriter(), 
																	new MathCeilingMethodWriter(), 
 																	new EmptyAnyMethodWriter(), 
																	new AnyAllMethodWriter(), 
																	new DefaultMethodWriter()
																};

        public string Write(Expression expression)
        {
            return expression == null ? null : Write(expression, expression.Type, GetRootParameterName(expression));
        }

        private static Type GetUnconvertedType(Expression expression)
        {
            ////Contract.Requires(expression != null);

            switch (expression.NodeType)
            {
                case ExpressionType.Convert:
                    var unaryExpression = expression as UnaryExpression;

                    //Contract.Assume(unaryExpression != null, "Matches node type.");

                    return unaryExpression.Operand.Type;
                default:
                    return expression.Type;
            }
        }

        private static string GetMemberCall(MemberExpression memberExpression)
        {
            //Contract.Requires(memberExpression != null);
            //Contract.Ensures(Contract.Result<string>() != null);

            var declaringType = memberExpression.Member.DeclaringType;
            var name = memberExpression.Member.Name;

            if (declaringType == typeof(string) && string.Equals(name, "Length"))
            {
                return name.ToLowerInvariant();
            }

            if (declaringType == typeof(DateTime))
            {
                switch (name)
                {
                    case "Hour":
                    case "Minute":
                    case "Second":
                    case "Day":
                    case "Month":
                    case "Year":
                        return name.ToLowerInvariant();
                }
            }

            return string.Empty;
        }

        private static Expression CollapseCapturedOuterVariables(MemberExpression input)
        {
            if (input == null || input.NodeType != ExpressionType.MemberAccess)
            {
                return input;
            }

            switch (input.Expression.NodeType)
            {
                case ExpressionType.New:
                case ExpressionType.MemberAccess:
                    var value = GetValue(input);
                    return Expression.Constant(value);
                case ExpressionType.Constant:
                    var obj = ((ConstantExpression)input.Expression).Value;
                    if (obj == null)
                    {
                        return input;
                    }

                    var fieldInfo = input.Member as FieldInfo;
                    if (fieldInfo != null)
                    {
                        var result = fieldInfo.GetValue(obj);
                        return result is Expression ? (Expression)result : Expression.Constant(result);
                    }

                    var propertyInfo = input.Member as PropertyInfo;
                    if (propertyInfo != null)
                    {
                        var result = propertyInfo.GetValue(obj, null);
                        return result is Expression ? (Expression)result : Expression.Constant(result);
                    }

                    break;
                case ExpressionType.TypeAs:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    return Expression.Constant(GetValue(input));
            }

            return input;
        }

        private static object GetValue(Expression input)
        {
            //Contract.Requires(input != null);

            var objectMember = Expression.Convert(input, typeof(object));
            var getterLambda = Expression.Lambda<Func<object>>(objectMember).Compile();

            return getterLambda();
        }

        private static bool IsMemberOfParameter(MemberExpression input)
        {
            if (input == null || input.Expression == null)
            {
                return false;
            }

            var nodeType = input.Expression.NodeType;
            var tempExpression = input.Expression as MemberExpression;
            while (nodeType == ExpressionType.MemberAccess)
            {
                if (tempExpression == null || tempExpression.Expression == null)
                {
                    return false;
                }

                nodeType = tempExpression.Expression.NodeType;
                tempExpression = tempExpression.Expression as MemberExpression;
            }

            return nodeType == ExpressionType.Parameter;
        }

        private static string GetOperation(Expression expression)
        {
            //Contract.Requires(expression != null);

            switch (expression.NodeType)
            {
                case ExpressionType.Add:
                    return "add";
                case ExpressionType.AddChecked:
                    break;
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    return "and";
                case ExpressionType.Divide:
                    return "div";
                case ExpressionType.Equal:
                    return "eq";
                case ExpressionType.GreaterThan:
                    return "gt";
                case ExpressionType.GreaterThanOrEqual:
                    return "ge";
                case ExpressionType.LessThan:
                    return "lt";
                case ExpressionType.LessThanOrEqual:
                    return "le";
                case ExpressionType.Modulo:
                    return "mod";
                case ExpressionType.Multiply:
                    return "mul";
                case ExpressionType.Not:
                    return "not";
                case ExpressionType.NotEqual:
                    return "ne";
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return "or";
                case ExpressionType.Subtract:
                    return "sub";
            }

            return string.Empty;
        }

        private static ParameterExpression GetRootParameterName(Expression expression)
        {
            if (expression is UnaryExpression)
            {
                expression = ((UnaryExpression)expression).Operand;
            }

            if (expression is LambdaExpression && ((LambdaExpression)expression).Parameters.Any())
            {
                return ((LambdaExpression)expression).Parameters.First();
            }

            return null;
        }

        private string Write(Expression expression, ParameterExpression rootParameterName)
        {
            return expression == null ? null : Write(expression, expression.Type, rootParameterName);
        }

        private string Write(Expression expression, Type type, ParameterExpression rootParameter)
        {
            //Contract.Requires(expression != null);
            //Contract.Requires(type != null);

            switch (expression.NodeType)
            {
                case ExpressionType.Parameter:
                    var parameterExpression = expression as ParameterExpression;

                    //Contract.Assume(parameterExpression != null);

                    return parameterExpression.Name;
                case ExpressionType.Constant:
                    {
                        var value = GetValue(Expression.Convert(expression, type));
                        return ParameterValueWriter.Write(value);
                    }

                case ExpressionType.Add:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Divide:
                case ExpressionType.Equal:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.Modulo:
                case ExpressionType.Multiply:
                case ExpressionType.NotEqual:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.Subtract:
                    return WriteBinaryExpression(expression, rootParameter);
                case ExpressionType.Negate:
                    return WriteNegate(expression, rootParameter);
                case ExpressionType.Not:
#if !SILVERLIGHT && false
                case ExpressionType.IsFalse:
#endif
                    return WriteFalse(expression, rootParameter);
#if !SILVERLIGHT && false
                case ExpressionType.IsTrue:
                    return WriteTrue(expression, rootParameter);
#endif
                case ExpressionType.Convert:
                case ExpressionType.Quote:
                    return WriteConversion(expression, rootParameter);
                case ExpressionType.MemberAccess:
                    return WriteMemberAccess(expression, rootParameter);
                case ExpressionType.Call:
                    return WriteCall(expression, rootParameter);
                case ExpressionType.New:
                case ExpressionType.ArrayIndex:
                case ExpressionType.ArrayLength:
                case ExpressionType.Conditional:
                case ExpressionType.Coalesce:
                    var newValue = GetValue(expression);
                    return ParameterValueWriter.Write(newValue);
                case ExpressionType.Lambda:
                    return WriteLambda(expression, rootParameter);
                default:
                    throw new InvalidOperationException("Expression is not recognized or supported");
            }
        }

        private string WriteLambda(Expression expression, ParameterExpression rootParameter)
        {
            var lambdaExpression = expression as LambdaExpression;

            //Contract.Assume(lambdaExpression != null);

            var body = lambdaExpression.Body;
            return Write(body, rootParameter);
        }

        private string WriteFalse(Expression expression, ParameterExpression rootParameterName)
        {
            var unaryExpression = expression as UnaryExpression;

            //Contract.Assume(unaryExpression != null);

            var operand = unaryExpression.Operand;

            return string.Format("not({0})", Write(operand, rootParameterName));
        }

        private string WriteTrue(Expression expression, ParameterExpression rootParameterName)
        {
            var unaryExpression = expression as UnaryExpression;

            //Contract.Assume(unaryExpression != null);

            var operand = unaryExpression.Operand;

            return Write(operand, rootParameterName);
        }

        private string WriteConversion(Expression expression, ParameterExpression rootParameterName)
        {
            var unaryExpression = expression as UnaryExpression;

            //Contract.Assume(unaryExpression != null);

            var operand = unaryExpression.Operand;
            return Write(operand, rootParameterName);
        }

        private string WriteCall(Expression expression, ParameterExpression rootParameterName)
        {
            var methodCallExpression = expression as MethodCallExpression;

            //Contract.Assume(methodCallExpression != null);

            return GetMethodCall(methodCallExpression, rootParameterName);
        }

        private string WriteMemberAccess(Expression expression, ParameterExpression rootParameterName)
        {
            var memberExpression = expression as MemberExpression;

            //Contract.Assume(memberExpression != null);

            if (memberExpression.Expression == null)
            {
                var memberValue = GetValue(memberExpression);
                return ParameterValueWriter.Write(memberValue);
            }

            var pathPrefixes = new List<string>();

            var currentMemberExpression = memberExpression;
            while (currentMemberExpression != null)
            {
                pathPrefixes.Add(currentMemberExpression.Member.Name);
                if (currentMemberExpression.Expression is ParameterExpression
                    && rootParameterName != null
                    && ((ParameterExpression)currentMemberExpression.Expression).Name != rootParameterName.Name)
                {
                    pathPrefixes.Add(((ParameterExpression)currentMemberExpression.Expression).Name);
                }

                currentMemberExpression = currentMemberExpression.Expression as MemberExpression;
            }

            pathPrefixes.Reverse();
            var prefix = string.Join("/", pathPrefixes.ToArray());

            if (!IsMemberOfParameter(memberExpression))
            {
                var collapsedExpression = CollapseCapturedOuterVariables(memberExpression);
                if (!(collapsedExpression is MemberExpression))
                {
                    //Contract.Assume(collapsedExpression != null);

                    return Write(collapsedExpression, rootParameterName);
                }

                memberExpression = (MemberExpression)collapsedExpression;
            }

            var memberCall = GetMemberCall(memberExpression);

            var innerExpression = memberExpression.Expression;

            //Contract.Assume(innerExpression != null);

            return StringExtensions.IsNullOrWhiteSpace(memberCall)
                       ? prefix
                       : string.Format("{0}({1})", memberCall, Write(innerExpression, rootParameterName));
        }

        private string WriteNegate(Expression expression, ParameterExpression rootParameterName)
        {
            var unaryExpression = expression as UnaryExpression;

            //Contract.Assume(unaryExpression != null);

            var operand = unaryExpression.Operand;

            return string.Format("-{0}", Write(operand, rootParameterName));
        }

        private string WriteBinaryExpression(Expression expression, ParameterExpression rootParameterName)
        {
            var binaryExpression = expression as BinaryExpression;

            //Contract.Assume(binaryExpression != null);

            var operation = GetOperation(binaryExpression);

            if (binaryExpression.Left.NodeType == ExpressionType.Call)
            {
                var compareResult = ResolveCompareToOperation(
                    rootParameterName,
                    (MethodCallExpression)binaryExpression.Left,
                    operation,
                    binaryExpression.Right as ConstantExpression);
                if (compareResult != null)
                {
                    return compareResult;
                }
            }

            if (binaryExpression.Right.NodeType == ExpressionType.Call)
            {
                var compareResult = ResolveCompareToOperation(
                    rootParameterName,
                    (MethodCallExpression)binaryExpression.Right,
                    operation,
                    binaryExpression.Left as ConstantExpression);
                if (compareResult != null)
                {
                    return compareResult;
                }
            }

            var isLeftComposite = CompositeExpressionTypes.Any(x => x == binaryExpression.Left.NodeType);
            var isRightComposite = CompositeExpressionTypes.Any(x => x == binaryExpression.Right.NodeType);

            var leftType = GetUnconvertedType(binaryExpression.Left);
            var leftString = Write(binaryExpression.Left, rootParameterName);
            var rightString = Write(binaryExpression.Right, leftType, rootParameterName);

            return string.Format(
                "{0} {1} {2}",
                string.Format(isLeftComposite ? "({0})" : "{0}", leftString),
                operation,
                string.Format(isRightComposite ? "({0})" : "{0}", rightString));
        }

        private string ResolveCompareToOperation(
            ParameterExpression rootParameterName,
            MethodCallExpression methodCallExpression,
            string operation,
            ConstantExpression comparisonExpression)
        {
            if (methodCallExpression != null
                && methodCallExpression.Method.Name == "CompareTo"
                && methodCallExpression.Method.ReturnType == typeof(int)
                && comparisonExpression != null
                && Equals(comparisonExpression.Value, 0))
            {
                return string.Format(
                    "{0} {1} {2}",
                    Write(methodCallExpression.Object, rootParameterName),
                    operation,
                    Write(methodCallExpression.Arguments[0], rootParameterName));
            }

            return null;
        }

        private string GetMethodCall(MethodCallExpression expression, ParameterExpression rootParameterName)
        {
            //Contract.Requires(expression != null);

            var methodCallWriter = _methodCallWriters.FirstOrDefault(w => w.CanHandle(expression));
            if (methodCallWriter == null)
            {
                throw new NotSupportedException(expression + " is not supported");
            }

            return methodCallWriter.Handle(expression, e => Write(e, rootParameterName));
        }
    }
}