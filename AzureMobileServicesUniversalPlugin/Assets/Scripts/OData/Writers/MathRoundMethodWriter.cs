// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MathRoundMethodWriter.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MathRoundMethodWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Provider.Writers
{
	using System;
	//using System.Diagnostics.Contracts;
	using System.Linq.Expressions;

	internal class MathRoundMethodWriter : MathMethodWriter
	{
		protected override string MethodName
		{
			get { return "round"; }
		}

		public override bool CanHandle(MethodCallExpression expression)
		{
			//////Contract.Assert(expression.Method != null);

			return expression.Method.DeclaringType == typeof(Math)
				   && expression.Method.Name == "Round";
		}
	}
}