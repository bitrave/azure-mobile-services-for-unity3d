// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ByteArrayValueWriter.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ByteArrayValueWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Provider.Writers
{
	using System;

	internal class ByteArrayValueWriter : ValueWriterBase<byte[]>
	{
		public override string Write(object value)
		{
			var base64 = Convert.ToBase64String((byte[])value);
			return string.Format("X'{0}'", base64);
		}
	}
}