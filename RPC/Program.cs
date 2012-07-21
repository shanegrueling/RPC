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
        }
    }
}
