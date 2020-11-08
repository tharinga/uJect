using System;

namespace uJect
{
    internal sealed class PrototypeBindingDefinition<T> : BindingDefinition where T : class, ICloneable
    {
        private T _prototype;
        
        public PrototypeBindingDefinition(Type concreteType, T prototype) : base(concreteType)
        {
            SetInstance(prototype);
        }
        
        public override void SetInstance(object instance)
        {
            if(instance is T prototype)
                _prototype = prototype;
            else
                throw new ArgumentException("Prototype must implement ICloneable interface");
        }

        public override object ResolveInstanceToInject(Type type)
        {
            return _prototype.Clone();
        }
    }
}