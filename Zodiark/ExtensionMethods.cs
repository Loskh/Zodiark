using System;
using System.Diagnostics;
using System.Reflection;

namespace Zodiark
{
	public static class ExtensionMethods
	{
		public static string ToHex(this IntPtr p) => string.Format("0x{0:X}", (ulong)p);
    }
}
