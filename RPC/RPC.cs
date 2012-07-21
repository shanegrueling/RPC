using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

namespace ILCallTest
{
    class RPC
    {
        class ControllerInfo
        {
            public Type type;
            public object instance;
            public Dictionary<string, OneParameter<object, object[]>> methods;
        }

        private Dictionary<string, ControllerInfo> controller = new Dictionary<string, ControllerInfo>();

        private delegate TReturn OneParameter<TReturn, TParameter0>(TParameter0 p0);

        public RPC()
        {
            var typesWithMyAttribute =
                from a in AppDomain.CurrentDomain.GetAssemblies()
                from t in a.GetTypes()
                let attributes = t.GetCustomAttributes(typeof(RPCController), true)
                where attributes != null && attributes.Length > 0
                select new { Type = t, Attributes = attributes.Cast<RPCController>() };

            foreach (var controllerInfo in typesWithMyAttribute)
            {
                addController((Type)controllerInfo.Type, (RPCController)(controllerInfo.Attributes.First()));                
            }
        }

        public void addController(Type t, RPCController attribute)
        {
            var methodsWithAttribute = 
                from m in t.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                let attributes = m.GetCustomAttributes(typeof(RPCMethod), true)
                where attributes != null && attributes.Length > 0
                select new { Type = m, Attributes = attributes.Cast<RPCMethod>() };

            
            ControllerInfo ci = new ControllerInfo();
            ci.type = t;
            ci.instance = Activator.CreateInstance(t);
            ci.methods = new Dictionary<string, OneParameter<object, object[]>>();

            this.controller.Add(attribute.name, ci);

            foreach (var methodeInfo in methodsWithAttribute)
            {
                addMethode(attribute.name, (MethodInfo)methodeInfo.Type, (RPCMethod)(methodeInfo.Attributes.First()));                
            }
        }

        public void addMethode(string controller, MethodInfo methodInfo, RPCMethod attribute)
        {
            // Create a dynamic method to write our own function           
            DynamicMethod call = new DynamicMethod(
                "",
                typeof(object),
                new Type[]{ methodInfo.DeclaringType, typeof(object[]) },
                methodInfo.DeclaringType
            );

            ILGenerator generator = call.GetILGenerator();
            
            //First get all parameters out of the object[]
            //and cast/unbox them to the right type.
            List<LocalBuilder> lbList = new List<LocalBuilder>();
            foreach (ParameterInfo parameter in methodInfo.GetParameters())
            {
                LocalBuilder lb = generator.DeclareLocal(parameter.ParameterType);
                generator.Emit(OpCodes.Ldarg_1);
                generator.Emit(OpCodes.Ldc_I4, parameter.Position);
                generator.Emit(OpCodes.Ldelem, typeof(object));
                if (parameter.ParameterType.IsClass)
                {
                    generator.Emit(OpCodes.Castclass, parameter.ParameterType);
                }
                else
                {
                    generator.Emit(OpCodes.Unbox_Any, parameter.ParameterType);
                }
                generator.Emit(OpCodes.Stloc, lb);
                lbList.Add(lb);
            }
            //Now call the method
            generator.Emit(OpCodes.Ldarg_0);
            foreach (LocalBuilder lb in lbList)
            {
                generator.Emit(OpCodes.Ldloc, lb);
            }
            generator.Emit(OpCodes.Callvirt, methodInfo);
            
            //Return the result
            if (!methodInfo.ReturnType.IsClass)
            {
                //built-ins must be boxed to become an object
                generator.Emit(OpCodes.Box, methodInfo.ReturnType);
            }
            generator.Emit(OpCodes.Ret);

            //Save the built method under its route
            this.controller[controller].methods.Add(
                attribute.name,
                (OneParameter<object, object[]>)call.CreateDelegate(
                    typeof(OneParameter<object, object[]>),
                    this.controller[controller].instance
                )
            );
        }

        public object route(string controller, string method, object[] parameter)
        {
            return (this.controller[controller].methods[method])(parameter);
        }
    }
}
