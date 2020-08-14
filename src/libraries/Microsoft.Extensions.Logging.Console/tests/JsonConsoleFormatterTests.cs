// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.Test.Console;
using Xunit;

namespace Microsoft.Extensions.Logging.Console.Test
{
    public class JsonConsoleFormatterTests : ConsoleFormatterTests
    {
        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsThreadingSupported))]
        public void NoLogScope_DoesNotWriteAnyScopeContentToOutput_Json()
        {
            // Arrange
            var t = ConsoleFormatterTests.SetUp(
                new ConsoleLoggerOptions { FormatterName = ConsoleFormatterNames.Json },
                new SimpleConsoleFormatterOptions { IncludeScopes = true },
                new ConsoleFormatterOptions { IncludeScopes = true },
                new JsonConsoleFormatterOptions {
                    IncludeScopes = true,
                    JsonWriterOptions = new JsonWriterOptions() { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping } 
                });
            var logger = t.Logger;
            var sink = t.Sink;

            // Act
            using (logger.BeginScope("Scope with named parameter {namedParameter}", 123))
            using (logger.BeginScope("SimpleScope"))
                logger.Log(LogLevel.Warning, 0, "Message with {args}", 73, _defaultFormatter);

            // Assert
            Assert.Equal(1, sink.Writes.Count);
            var write = sink.Writes[0];
            Assert.Equal(TestConsole.DefaultBackgroundColor, write.BackgroundColor);
            Assert.Equal(TestConsole.DefaultForegroundColor, write.ForegroundColor);
            Assert.Contains("Message with {args}", write.Message);
            Assert.Contains("73", write.Message);
            Assert.Contains("{OriginalFormat}", write.Message);
            Assert.Contains("namedParameter", write.Message);
            Assert.Contains("123", write.Message);
            Assert.Contains("SimpleScope", write.Message);
        }

        private static void EnsureStackTrace(params Exception[] exceptions)
        {
            if (exceptions == null) return;

            foreach (Exception exception in exceptions)
            {
                if (string.IsNullOrEmpty(exception.StackTrace))
                {
                    try
                    {
                        throw exception;
                    }
                    catch
                    { }
                }
                Assert.False(string.IsNullOrEmpty(exception.StackTrace));
            }
        }

        private string GetJson(Exception exception)
        {
            if (exception == null) throw new ArgumentNullException(nameof(exception));
            JsonConsoleFormatterOptions jsonOptions = new JsonConsoleFormatterOptions();
            var jsonMonitor = new TestFormatterOptionsMonitor<JsonConsoleFormatterOptions>(jsonOptions);
            var jsonFormatter = new JsonConsoleFormatter(jsonMonitor);
            Func<string, Exception, string> exceptionFormatter = (state, exception) => state.ToString();
            LogEntry<string> entry = new LogEntry<string>(LogLevel.Error, string.Empty, new EventId(), string.Empty, exception, exceptionFormatter);
            StringBuilder output = new StringBuilder();
            using (TextWriter writer = new StringWriter(output))
            {
                jsonFormatter.Write<string>(entry, null, writer);
            }
            return output.ToString();
        }

        [Fact]
        public void ShouldContainInnerException()
        {
            Exception rootException = new Exception("root", new Exception("inner"));
            EnsureStackTrace(rootException, rootException.InnerException);
            string json = GetJson(rootException);
            Assert.Contains(rootException.Message, json);
            Assert.Contains(rootException.InnerException.Message, json);
        }

        [Fact]
        public void ShouldContainAggregateExceptions()
        {
            AggregateException rootException = new AggregateException("aggregate", new Exception("leaf1"), new Exception("leaf2"), new Exception("leaf3"));
            EnsureStackTrace(rootException);
            EnsureStackTrace(rootException.InnerExceptions.ToArray());
            string json = GetJson(rootException);
            Assert.Contains(rootException.Message, json);
            rootException.InnerExceptions.ToList().ForEach((inner) => Assert.Contains(inner.Message, json));
        }
    }
}
