// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Xunit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.SpanTests
{
    public static partial class SpanTests
    {
        [Fact]
        public static void Empty()
        {
            Span<int> empty = Span<int>.Empty;
            Assert.True(empty.IsEmpty);
            Assert.Equal(0, empty.Length);
            unsafe
            {
                ref int expected = ref Unsafe.AsRef<int>(null);
                ref int actual = ref MemoryMarshal.GetReference(empty);
                Assert.True(Unsafe.AreSame(ref expected, ref actual));
            }
        }
    }
}
