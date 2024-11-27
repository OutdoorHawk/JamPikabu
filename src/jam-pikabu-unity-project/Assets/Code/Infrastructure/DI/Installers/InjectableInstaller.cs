using System;
using System.Reflection;
using Zenject;

namespace Code.Infrastructure.DI.Installers
{
    public class InjectableInstaller
    {
        public static void Install(DiContainer diContainer)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    TryBindType(diContainer, type);
                }
            }
        }

        private static void TryBindType(DiContainer diContainer, Type type)
        {
            if (type.GetCustomAttribute(typeof(InjectableAttribute), false) is not InjectableAttribute attrib)
                return;
            
            FromBinderNonGeneric binder;

            if (attrib.LinkedTypes == null)
                binder = diContainer.Bind(type);
            else
                binder = diContainer.Bind(attrib.LinkedTypes).To(type);
            
            if (attrib.asSingle)
                binder.AsSingle();

            if (attrib.nonLazy)
                binder.NonLazy();

            if (attrib.asTransient)
                binder.AsTransient();
        }
    }
}