using System;
using UnityEngine.Scripting;

namespace Code.Infrastructure.DI.Installers
{
    [Preserve]
    [AttributeUsage(AttributeTargets.Class)]
    public class InjectableAttribute : PreserveAttribute
    {
        public readonly bool asSingle;
        public readonly bool nonLazy;
        public readonly bool asTransient;
        public readonly Type[] LinkedTypes;

        public InjectableAttribute(bool nonLazy = false)
        {
            asTransient = true;
            this.nonLazy = nonLazy;
        }

        public InjectableAttribute(params Type[] types)
        {
            LinkedTypes = types;
            asSingle = true;
        }

        public InjectableAttribute(bool asTransient, params Type[] types)
        {
            this.asTransient = asTransient;
            LinkedTypes = types;
        }

        public InjectableAttribute(bool asSingle, bool nonLazy, params Type[] types)
        {
            this.asSingle = asSingle;
            this.nonLazy = nonLazy;
            LinkedTypes = types;
        }
    }
}