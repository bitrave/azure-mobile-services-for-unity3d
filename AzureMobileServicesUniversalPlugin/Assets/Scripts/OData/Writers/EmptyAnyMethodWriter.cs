// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmptyAnyMethodWriter.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the EmptyAnyMethodWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Provider.Writers
{
	using System;
	//using System.Diagnostics.Contracts;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;

	internal class EmptyAnyMethodWriter : IMethodCallWriter
	{
		private static readonly MethodInfo AnyMethod = typeof(Enumerable)
#if !NETFX_CORE
.GetMethods()
#else
.GetRuntimeMethods()
#endif
.FirstOrDefault(m => m.Name == "Any" && m.GetParameters()
													 .Length == 2);

		public bool CanHandle(MethodCallExpression expression)
		{
			//Contract.Assert(expression.Method != null);

			return expression.Method.Name == "Any" && expression.Arguments.Count == 1;
		}

		public string Handle(MethodCallExpression expression, Func<Expression, string> expressionWriter)
		{
			//Contract.Assume(expression.Arguments.Count > 0);

#if !NETFX_CORE
			var argumentType = expression.Arguments[0].Type;
#else
			var argumentType = expression.Arguments[0].Type.GetTypeInfo();
#endif
			var parameterType = argumentType.IsGenericType
#if !NETFX_CORE
 ? argumentType.GetGenericArguments()[0]
#else
									? argumentType.GetGenericParameterConstraints()[0]
#endif
 : typeof(object);
			var anyMethod = AnyMethod.MakeGenericMethod(parameterType);

            var parameter = Expression.Parameter(parameterType, "vaughan");

			var lambda = Expression.Lambda(Expression.Constant(true), parameter);
			var rewritten = Expression.Call(expression.Object, anyMethod, expression.Arguments[0], lambda);
			return expressionWriter(rewritten);
		}
	}
}