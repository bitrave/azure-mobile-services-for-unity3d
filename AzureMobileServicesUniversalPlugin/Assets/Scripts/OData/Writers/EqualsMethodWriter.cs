// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EqualsMethodWriter.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the EqualsMethodWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Provider.Writers
{
	using System;
	//using System.Diagnostics.Contracts;
	using System.Linq.Expressions;

	internal class EqualsMethodWriter : IMethodCallWriter
	{
		public bool CanHandle(MethodCallExpression expression)
		{
			return expression.Method.Name == "Equals";
		}

		public string Handle(MethodCallExpression expression, Func<Expression, string> expressionWriter)
		{
			//Contract.Assert(expression.Arguments != null);
			//Contract.Assume(expression.Arguments.Count > 0);

			return string.Format(
				"{0} eq {1}", 
				expressionWriter(expression.Object), 
				expressionWriter(expression.Arguments[0]));
		}
	}
}