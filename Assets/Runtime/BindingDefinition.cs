using System;

namespace uJect
{
    public abstract class BindingDefinition
    {
        protected BindingDefinition(Type concreteType)
        {
            _concreteType = concreteType;
        }

        protected object _instance;
        protected readonly Type _concreteType;

        public virtual void SetInstance(object instance)
        {
            _instance = instance;
        }

        public abstract object ResolveInstanceToInject(Type type);
    }
}