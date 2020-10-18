using System;

namespace uJect
{
    public class ConcreteBindingDefinition : BindingDefinition
    {
        public ConcreteBindingDefinition(Type concreteType) 
            : base(concreteType)
        {
        }

        public override object ResolveInstanceToInject(Type type)
            => _instance;
    }
}