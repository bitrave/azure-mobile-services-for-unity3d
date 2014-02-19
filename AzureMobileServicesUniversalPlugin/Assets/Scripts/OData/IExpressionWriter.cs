// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExpressionWriter.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the public interface for an expression visitor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Provider
{
    using System.Linq.Expressions;

    /// <summary>
    /// Defines the public interface for an expression visitor.
    /// </summary>
    public interface IExpressionWriter
    {
        /// <summary>
        /// Generates a string representation of the passed expression.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to visit.</param>
        /// <returns>A string value.</returns>
        string Write(Expression expression);
    }
}