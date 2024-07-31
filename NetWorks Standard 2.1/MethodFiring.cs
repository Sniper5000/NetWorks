using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NetWorks_Library
{
    /// <summary>
    /// Method Firing allows executing methods from received data.
    /// This is the instanced type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MethodFiring<T>
    {
        private readonly Dictionary<int, MethodInfo> methodIdMap = new Dictionary<int, MethodInfo>();

        public MethodFiring()
        {
            foreach(var method in typeof(T).GetMethods())
            {
                var attribute = method.GetCustomAttribute<MethodFiringId>();
                if(attribute == null) continue;
                methodIdMap.Add(attribute.Identifier, method);
            }
        }
        
        public bool FireMethod(int methodId, T receiver, object?[]? args = null)
        {
            if(methodIdMap.TryGetValue(methodId, out var method))
            {
                method.Invoke(receiver, args);
                return true;
            }

            return false;
        }
    }

    //Attributes

    [AttributeUsage(AttributeTargets.Method)]
    public class MethodFiringId : Attribute
    {
        public int Identifier { get; set; }

        public MethodFiringId(int Identifier)
        {
            this.Identifier = Identifier;
        }
    }
}
