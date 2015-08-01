using System;
using System.Diagnostics;

namespace Upgrader
{
    internal class Tracer
    {
        public static TraceSource _source;

        public Tracer()
        {
            _source = new TraceSource("Upgrader", SourceLevels.All);
        }

        public void Trace(string message)
        {
            _source.TraceEvent(TraceEventType.Information, 0, $"{DateTime.Now} -> {message}");
            _source.Flush();
        }
    }
}