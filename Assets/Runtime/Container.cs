using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace uJect
{
    using static BindingFlags;
    
    public class Container : MonoBehaviour
    {
        private readonly Dictionary<Type, BindingDefinition> _bindings = new Dictionary<Type, BindingDefinition>();

        public void ResolveDependencies()
        {
            var monoBehaviors = Resources.FindObjectsOfTypeAll<MonoBehaviour>();

            CollectDependencyInstances(monoBehaviors);
            InjectDependencies(monoBehaviors);
        }

        void CollectDependencyInstances(MonoBehaviour[] behaviors)
        {
            foreach (var behaviour in behaviors)
            {
                var type = behaviour.GetType();
                BindInstance(type, behaviour);

                var interfaces = type.GetInterfaces();

                foreach (var @interface in interfaces)
                {
                    BindInstance(@interface, behaviour);
                }
            }
        }

        private void BindInstance(Type type, MonoBehaviour instance)
        {
            if (_bindings.ContainsKey(type))
            {
                _bindings[type].SetInstance(instance);
            }
        }

        private void InjectDependencies(MonoBehaviour[] targets)
        {
            var parameterList = new List<object>(8);

            foreach (var target in targets)
            {
                var method = target.GetType().GetMethod("Inject", Instance | Public);
                if (method == null)
                {
                    continue;
                }

                var parameters = method.GetParameters();
                foreach (var parameter in parameters)
                {
                    if (_bindings.TryGetValue(parameter.ParameterType, out var binding))
                    {
                        parameterList.Add(binding.ResolveInstanceToInject(target.GetType()));
                    }
                }

                method.Invoke(target, parameterList.ToArray());
                parameterList.Clear();
            }
        }

        public BindingBuilder Bind<T>()
        {
            return BindingBuilder.Bind<T>(this);
        }

        public class BindingBuilder
        {
            private BindingBuilder(Container container, Type type)
            {
                _container = container;
                _type = type;
            }

            private readonly Container _container;
            private readonly Type _type;
            private Type _concreteType;
            private Type _conditionalTargetType;


            public static BindingBuilder Bind<T>(Container container)
                => new BindingBuilder(container, typeof(T));

            public BindingBuilder To<T>()
            {
                _concreteType = typeof(T);
                return this;
            }

            public BindingBuilder WhenInjectedInto<T>()
            {
                _conditionalTargetType = typeof(T);
                return this;
            }

            public void AsSingle()
            {
                if (IsConditionalBinding)
                {
                    BindConditional();
                }
                else if(IsAbstractBinding)
                {
                    BindAbstract();
                }
                else
                {
                    BindConcrete();
                }
            }

            private bool IsConditionalBinding
                => _conditionalTargetType != null;

            private bool IsAbstractBinding
                => _concreteType != null;
            

            private void BindConditional()
            {
                if (!_container._bindings.TryGetValue(_type, out var binding))
                {
                    _container._bindings[_type] = new ConditionalBindingDefinition(_type, _concreteType, _conditionalTargetType);
                }

                if (binding is ConditionalBindingDefinition definition)
                {
                    definition.AddDefinition(_concreteType, _conditionalTargetType);
                }
            }

            private void BindAbstract()
            {
                _container._bindings[_type] =  new ConcreteBindingDefinition(_type);
            }
            
            private void BindConcrete()
            {
                _container._bindings[_type] = new ConcreteBindingDefinition(_type);
            }
        }

    }
}
