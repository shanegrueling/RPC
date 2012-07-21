using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using System.Diagnostics;

namespace ILCallTest
{
    class Program
    {
        class Test
        {
            public string blub(int a)
            {
                return "Der Parameter ist" + a.ToString();
            }
        }

        private delegate TReturn OneParameter<TReturn, TParameter0>
        (TParameter0 p0);

        static void Main(string[] args)
        {
            RPC rpc = new RPC();

            Console.WriteLine(rpc.route("Test", "hello", new object[] { "World" }));
            Console.WriteLine(rpc.route("Test", "multiply", new object[] { 10, 20 }));
            Console.WriteLine(rpc.route("Test", "callable", new object[] {  }));
            Console.WriteLine(rpc.route("Test2", "classAsParameter", new object[] { new TestClass() }));
            Console.WriteLine(rpc.route("Test2", "classAsReturnValue", new object[] { "blub" }));
        }

        static void oldMain(string[] args)
        {
            Console.WriteLine(new Test().blub(2));
            MethodInfo mi = typeof(Test).GetMethod("blub", BindingFlags.Instance | BindingFlags.Public);

            Type[] methodArgs = { typeof(Test), typeof(int) };

            DynamicMethod ilCall = new DynamicMethod(
            "",
            typeof(string),
            methodArgs,
            typeof(Test));      

            ILGenerator generator = ilCall.GetILGenerator();

            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Callvirt, mi);
            generator.Emit(OpCodes.Ret);

            OneParameter<string, int> invoke = (OneParameter<string, int>)
            ilCall.CreateDelegate(
                typeof(OneParameter<string, int>),
                new Test()
            );

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            for (int i = 0; i < 100000; ++i)
            {
                invoke(i);
            }

            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine(elapsedTime, "RunTime");


            Stopwatch stopWatch2 = new Stopwatch();
            stopWatch2.Start();

            Test t = new Test();
            for (int i = 0; i < 100000; ++i)
            {
                mi.Invoke(t, new object[] { i });
            }

            stopWatch2.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts2 = stopWatch2.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime2 = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts2.Hours, ts2.Minutes, ts2.Seconds,
                ts2.Milliseconds / 10);
            Console.WriteLine(elapsedTime2, "RunTime2");

            Stopwatch stopWatch3 = new Stopwatch();
            stopWatch3.Start();

            for (int i = 0; i < 100000; ++i)
            {
                t.blub(i);
            }

            stopWatch3.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts3 = stopWatch3.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime3 = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts3.Hours, ts3.Minutes, ts3.Seconds,
                ts3.Milliseconds / 10);
            Console.WriteLine(elapsedTime3, "RunTime3");
        }
    }
}
