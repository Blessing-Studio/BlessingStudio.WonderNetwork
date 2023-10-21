using BlessingStudio.WonderNetwork.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BlessingStudio.WonderNetwork.Utils
{
    public static class ReflectionUtils
    {
        public static object Deserilize(Type type, ISerilizer serilizer, byte[] data)
        {
            Type serilizerType = serilizer.GetType();
            if (serilizerType.GenericTypeArguments.Length == 1)
            {
                Type genericType = serilizerType.GenericTypeArguments[0];
                if(type == genericType || type.IsSubclassOf(genericType))
                {
                    MethodInfo methodInfo = serilizerType.GetMethod("Deserilize")!;
                    return methodInfo.Invoke(serilizer, new object[] { data })!;
                }
                throw new InvalidOperationException("Type is not the type or subclass in serilize");
            }
            throw new InvalidOperationException("Serilizer Error");
        }
    }
}
