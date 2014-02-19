// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringReplaceMethodWriter.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the StringReplaceMethodWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Provider.Writers
{
	using System;
	//using System.Diagnostics.Contracts;
	using System.Linq.Expressions;

	internal class StringReplaceMethodWriter : IMethodCallWriter
	{
		public bool CanHandle(MethodCallExpression expression)
		{
			//////Contract.Assert(expression.Method != null);

			return expression.Method.DeclaringType == typeof(string)
				   && expression.Method.Name == "Replace";
		}

		public string Handle(MethodCallExpression expression, Func<Expression, string> expressionWriter)
		{
			//////Contract.Assert(expression.Arguments != null);
			////Contract.Assume(expression.Arguments.Count > 1);

			var firstArgument = expression.Arguments[0];
			var secondArgument = expression.Arguments[1];
			var obj = expression.Object;

			////Contract.Assume(firstArgument != null);
			//Contract.Assume(secondArgument != null);
			//Contract.Assume(obj != null);

			return string.Format(
				"replace({0}, {1}, {2})", 
				expressionWriter(obj), 
				expressionWriter(firstArgument), 
				expressionWriter(secondArgument));
		}
	}
}