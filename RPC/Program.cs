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
        static void Main(string[] args)
        {
            RPC rpc = new RPC();

            Console.WriteLine(rpc.route("Test", "hello", new object[] { "World" }));
            Console.WriteLine(rpc.route("Test", "multiply", new object[] { 10, 20 }));
            Console.WriteLine(rpc.route("Test", "callable", new object[] { }));
            Console.WriteLine(rpc.route("Test2", "classAsParameter", new object[] { new TestClass() }));
            Console.WriteLine(rpc.route("Test2", "classAsReturnValue", new object[] { "blub" }));
            foreach(object a in (int[])rpc.route("Test2", "returnAnArray", new object[] { }))
            {
                Console.WriteLine(a);
            }

            foreach (object a in (int[])rpc.route("Test2", "arrayAsParameter", new object[] { new object[] {1, 2, 3, 4, 5} }))
            {
                Console.WriteLine(a);
            }

            foreach (object a in (TestClass[])rpc.route("Test2", "classArrayAsParameter", new object[] { new object[] { new TestClass(), new TestClass() } }))
            {
                Console.WriteLine(a);
            }

            Console.ReadKey();
        }
    }
}
