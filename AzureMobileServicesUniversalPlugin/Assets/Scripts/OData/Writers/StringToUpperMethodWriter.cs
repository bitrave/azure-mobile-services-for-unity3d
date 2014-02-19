// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringToUpperMethodWriter.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the StringToUpperMethodWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Provider.Writers
{
	using System;
    ////using System.Diagnostics.Contracts;
	using System.Linq.Expressions;

	internal class StringToUpperMethodWriter : IMethodCallWriter
	{
		public bool CanHandle(MethodCallExpression expression)
		{
			//Contract.Assert(expression.Method != null);

			return expression.Method.DeclaringType == typeof(string)
				   && (expression.Method.Name == "ToUpper" || expression.Method.Name == "ToUpperInvariant");
		}

		public string Handle(MethodCallExpression expression, Func<Expression, string> expressionWriter)
		{
			var obj = expression.Object;

			//Contract.Assume(obj != null);

			return string.Format("toupper({0})", expressionWriter(obj));
		}
	}
}