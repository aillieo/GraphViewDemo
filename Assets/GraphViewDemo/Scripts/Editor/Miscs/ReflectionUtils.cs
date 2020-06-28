using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AillieoUtils.GraphViewDemo.Editor
{
    public static class ReflectionUtils
    {
        private static readonly Dictionary<string, string> cachedAssemblyQualifiedNameByTypeName = new Dictionary<string, string>();

        public static Type GetType(string typeName)
        {
            Type type = Type.GetType(typeName);
            if (type != null)
            {
                return type;
            }
            string assemblyQualifiedName;
            if (!cachedAssemblyQualifiedNameByTypeName.TryGetValue(typeName, out assemblyQualifiedName))
            {
                foreach (var ass in AppDomain.CurrentDomain.GetAssemblies())
                {
                    type = ass.GetType(typeName);
                    if (type != null)
                    {
                        cachedAssemblyQualifiedNameByTypeName.Add(typeName, type.AssemblyQualifiedName);
                        return type;
                    }
                }
                cachedAssemblyQualifiedNameByTypeName.Add(typeName, null);
            }

            if (!string.IsNullOrEmpty(assemblyQualifiedName))
            {
                return Type.GetType(assemblyQualifiedName);
            }
            UnityEngine.Debug.LogError("Get type failed: " + typeName);
            return null;
        }
    }

}
