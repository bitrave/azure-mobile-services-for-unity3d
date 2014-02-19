// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BooleanValueWriter.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the BooleanValueWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Provider.Writers
{
	internal class BooleanValueWriter : ValueWriterBase<bool>
	{
		public override string Write(object value)
		{
			var boolean = (bool)value;

			return boolean ? "true" : "false";
		}
	}
}