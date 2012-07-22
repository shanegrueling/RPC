using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ILCallTest
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class RPCController : Attribute
    {
        public RPCController(string name) { this.name = name;}
        public string name;
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class RPCMethod : Attribute
    {
        public RPCMethod(string name) { this.name = name;}
        public string name;
    }

    [RPCController("Test")]
    class TestController
    {
        [RPCMethod("callable")]
        public int callable()
        {
            return 1;
        }
        
        [RPCMethod("multiply")]
        public int multiply(int one, int two)
        {
            return one*two;
        }

        public string notCallableFromOutside(int a)
        {
            return "This should never return";
        }

        [RPCMethod("hello")]
        public string multiply(string world)
        {
            return "Hello "+world;
        }
    }

    class TestClass
    {
        public string hello = "hello";
        public string world = "world";

        public override string ToString()
        {
            return this.hello + " " + this.world;
        }
    }

    [RPCController("Test2")]
    class Test2Controller
    {
        [RPCMethod("classAsParameter")]
        public string classAsParameter(TestClass test)
        {
            return test.hello+" "+test.world;
        }

        [RPCMethod("classAsReturnValue")]
        public TestClass classAsReturnValue(string world)
        {
            TestClass t = new TestClass();
            t.world = world;
            return t;
        }

        [RPCMethod("returnAnArray")]
        public int[] returnAnArray()
        {
            return new int[] { 10, 12, 13};
        }

        [RPCMethod("arrayAsParameter")]
        public int[] arrayAsParameter(int[] array)
        {
            for(int i = 0; i < array.Count(); ++i)
            {
                array[i] *= 2;
            }
            return array;
        }

        [RPCMethod("classArrayAsParameter")]
        public TestClass[] classArrayAsParameter(TestClass[] array)
        {
            for (int i = 0; i < array.Count(); ++i)
            {
                array[i].world = i.ToString();
            }
            return array;
        }
    }
}
