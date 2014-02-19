// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RationalValueWriter.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the RationalValueWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Provider.Writers
{
	using System.Globalization;

	internal abstract class RationalValueWriter<T> : ValueWriterBase<T>
	{
		public override string Write(object value)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}", value);
		}
	}
}