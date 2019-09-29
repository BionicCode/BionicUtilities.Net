using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace BionicUtilities.NetStandard.Profiling
{
  public static class Profiler
  {
    public static Action<TimeSpan> LogPrinter { get; set; }

    public static TimeSpan LogTime(Action action)
    {
      var stopwatch = new Stopwatch();
      stopwatch.Start();
      action.Invoke();
      stopwatch.Stop();
      TimeSpan stopwatchElapsed = stopwatch.Elapsed;
      if (Profiler.LogPrinter == null)
      {
        Profiler.LogPrinter = (elapsedTime) =>
          Console.WriteLine($"Elapsed time: {elapsedTime.TotalMilliseconds} [ms]");
      }
      Profiler.LogPrinter?.Invoke(stopwatchElapsed);

      return stopwatchElapsed;
    }

    public static List<TimeSpan> LogTimes(Action action, int runCount)
    {
      if (Profiler.LogPrinter == null)
      {
        Profiler.LogPrinter = (elapsedTime) =>
          Console.WriteLine($"Iteration #{runCount}: Elapsed time: {elapsedTime.TotalMilliseconds} [ms]");
      }
      var stopwatch = new Stopwatch();
      var measuredTimes = new List<TimeSpan>();

      for (; runCount > 0; runCount--)
      {
        stopwatch.Start();
        action.Invoke();
        stopwatch.Stop();
        TimeSpan stopwatchElapsed = stopwatch.Elapsed;
        measuredTimes.Add(stopwatchElapsed);
        Profiler.LogPrinter.Invoke(stopwatchElapsed);
      }

      return measuredTimes;
    }

    public static TimeSpan LogAverageTime(Action action, int runCount)
    {
      var stopwatch = new Stopwatch();
      var measuredTimes = new List<TimeSpan>();

      for (; runCount > 0; runCount--)
      {
        stopwatch.Start();
        action.Invoke();
        stopwatch.Stop();
        TimeSpan stopwatchElapsed = stopwatch.Elapsed;
        measuredTimes.Add(stopwatchElapsed);
      }

      var logAverageTime = new TimeSpan((long) measuredTimes.Average((time) => time.Ticks));
      if (Profiler.LogPrinter == null)
      {
        Profiler.LogPrinter = (elapsedTime) =>
          Console.WriteLine($"Iterations={runCount}; Average elapsed time: {elapsedTime.TotalMilliseconds} [ms]");
      }
      Profiler.LogPrinter.Invoke(logAverageTime);
      return logAverageTime;
    }
  }
}
