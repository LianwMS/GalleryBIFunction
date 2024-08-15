using Microsoft.Extensions.Logging;

namespace GelleryBI.Tests
{
    public class TestConsoleLogge : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            string message = state == null ? null : state.ToString();
            if (exception == null)
            {
                Console.WriteLine($"{logLevel} {message}");
            }
            else
            {
                Console.WriteLine($"{logLevel} {message} Exception: {exception.Message}");
            }
        }
    }
}
