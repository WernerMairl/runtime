// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Xunit;

namespace Microsoft.Extensions.Logging.Test
{
    public class LogValuesFormatterTest
    {
        [Fact]
        public void ShoulNotModifyArguments()
        {
            object[] arguments = new object[] { null, null };
            var formatter = new LogValuesFormatter(string.Empty);
            formatter.Format(arguments);
            Assert.Null(arguments[0]);
            Assert.Null(arguments[1]);
        }

        [Fact]
        public void ShouldHandleNullArgument()
        {
            object[] arguments = null;
            var formatter = new LogValuesFormatter(string.Empty);
            var result = formatter.Format(arguments);
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void ShouldHandleEmptyArgumentArray()
        {
            object[] arguments = new object[] { };
            var formatter = new LogValuesFormatter(string.Empty);
            var result = formatter.Format(arguments);
            Assert.Equal(string.Empty, result);
        }
    }
}
