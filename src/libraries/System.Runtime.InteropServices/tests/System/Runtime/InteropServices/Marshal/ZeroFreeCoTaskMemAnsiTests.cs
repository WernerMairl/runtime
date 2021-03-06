// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Security;
using Xunit;

namespace System.Runtime.InteropServices.Tests
{
    public class ZeroFreeCoTaskMemAnsiTests
    {
        [Fact]
        public void ZeroFreeCoTaskMemAnsi_ValidPointer_Success()
        {
            using (SecureString secureString = ToSecureString("hello"))
            {
                IntPtr ptr = Marshal.SecureStringToCoTaskMemAnsi(secureString);
                Marshal.ZeroFreeCoTaskMemAnsi(ptr);
            }
        }

        [Fact]
        public void ZeroFreeCoTaskMemAnsi_Zero_Nop()
        {
            Marshal.ZeroFreeCoTaskMemAnsi(IntPtr.Zero);
        }

        private static SecureString ToSecureString(string data)
        {
            var str = new SecureString();
            foreach (char c in data)
            {
                str.AppendChar(c);
            }
            str.MakeReadOnly();
            return str;
        }
    }
}
