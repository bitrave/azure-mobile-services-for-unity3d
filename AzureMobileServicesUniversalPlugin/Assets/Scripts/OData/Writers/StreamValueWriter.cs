// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamValueWriter.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the StreamValueWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Provider.Writers
{
	using System;
	using System.IO;

	internal class StreamValueWriter : ValueWriterBase<Stream>
	{
		public override string Write(object value)
		{
			var stream = (Stream)value;
			if (stream.CanSeek)
			{
				stream.Seek(0, SeekOrigin.Begin);
			}

			var buffer = new byte[stream.Length];
			stream.Read(buffer, 0, buffer.Length);
			var base64 = Convert.ToBase64String(buffer);

			return string.Format("X'{0}'", base64);
		}
	}
}