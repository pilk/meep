//#define ENABLE_PROFILER

using System;
using System.Collections;
using System.Collections.Generic;

namespace Utilities
{
    // Smaller memory footprint by using struct
#if ENABLE_PROFILER
	public struct Benchmark : System.IDisposable
#else
    public struct Benchmark : System.IDisposable
#endif
    {
        public static Benchmark Start(string name)
        {
#if ENABLE_PROFILER
            Utilities.Profiler.Start(name);
			return new Benchmark();
#else
            return null;
#endif
        }

        public void Dispose()
        {
            Utilities.Profiler.Stop();
        }
    }

    static public class Profiler
    {
        public struct ProfilerInstance
        {
            public string ID;
            public DateTime timeStamp;
        };


        static private Stack<ProfilerInstance> m_instances = new Stack<ProfilerInstance>();
        static private object m_timeStampLock = new object();

        static public void Start(string ID)
        {
            lock (m_timeStampLock)
            {
                ProfilerInstance instance = new ProfilerInstance();
                instance.ID = ID;
                instance.timeStamp = DateTime.UtcNow;
                m_instances.Push(instance);
            }
        }

        static public void Stop()
        {
            lock (m_timeStampLock)
            {
                ProfilerInstance instance = m_instances.Pop();
                Console.WriteLine("<P> " + GetIndent ( m_instances.Count ) + instance.ID + " took " + (int)(DateTime.UtcNow - instance.timeStamp).TotalMilliseconds + " ms");
            }
        }

        static private string GetIndent( int count )
        {
            string text = "";
            for (int i = 0; i < count; i++)
                text += "--";
            return text;
        }
    };
}