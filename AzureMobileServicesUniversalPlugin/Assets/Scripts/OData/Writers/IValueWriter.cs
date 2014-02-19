// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IValueWriter.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IValueWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Provider.Writers
{
	using System;
	//using System.Diagnostics.Contracts;

#if NETFX_CORE
	using System.Reflection;
#endif

	//[ContractClass(typeof(ValueWriterContracts))]
	internal interface IValueWriter
	{
		bool Handles(Type type);

		string Write(object value);
	}

	//[ContractClassFor(typeof(IValueWriter))]
	internal abstract class ValueWriterContracts : IValueWriter
	{
		public bool Handles(Type type)
		{
			//Contract.Requires(type != null);

			throw new NotImplementedException();
		}

		public string Write(object value)
		{
			//Contract.Requires(value != null);

			throw new NotImplementedException();
		}
	}
}