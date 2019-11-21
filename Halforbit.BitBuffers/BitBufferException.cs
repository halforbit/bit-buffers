using System;
using System.Diagnostics;

namespace Lidgren.Network
{
    /// <summary>
    /// Exception thrown in the Lidgren Network Library
    /// </summary>
    public sealed class BitBufferException : Exception
	{
		/// <summary>
		/// NetException constructor
		/// </summary>
		public BitBufferException()
			: base()
		{
		}

		/// <summary>
		/// NetException constructor
		/// </summary>
		public BitBufferException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// NetException constructor
		/// </summary>
		public BitBufferException(string message, Exception inner)
			: base(message, inner)
		{
		}

		/// <summary>
		/// Throws an exception, in DEBUG only, if first parameter is false
		/// </summary>
		[Conditional("DEBUG")]
		public static void Assert(bool isOk, string message)
		{
			if (!isOk)
				throw new BitBufferException(message);
		}

		/// <summary>
		/// Throws an exception, in DEBUG only, if first parameter is false
		/// </summary>
		[Conditional("DEBUG")]
		public static void Assert(bool isOk)
		{
			if (!isOk)
				throw new BitBufferException();
		}
	}
}
