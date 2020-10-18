using System;
using System.Collections.Generic;

namespace uJect
{
    public class ConditionalBindingDefinition : BindingDefinition
    {
        private Dictionary<Type, Type> _conditionalBindings = new Dictionary<Type, Type>();
        private Dictionary<Type, object> _instances = new Dictionary<Type, object>();
        public ConditionalBindingDefinition(Type abstractType, Type concreteType, Type targetType) 
            : base(abstractType)
        {
            _conditionalBindings[concreteType] = targetType;
        }

        public void AddDefinition(Type concreteType, Type targetType)
        {
            _conditionalBindings[concreteType] = targetType;
        }
        
        public override void SetInstance(object instance)
        {
            var type = _conditionalBindings[instance.GetType()];
            _instances[type] = instance;
        }

        public override object ResolveInstanceToInject(Type type)
        {
            return _instances[type];
        }
    }
}