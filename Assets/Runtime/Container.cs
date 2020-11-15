using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace uJect
{
    using static BindingFlags;
    
    public class Container : MonoBehaviour
    {
        private readonly Dictionary<Type, BindingDefinition> _bindings = new Dictionary<Type, BindingDefinition>();
        private HashSet<Type> _instancesToCollect = new HashSet<Type>();  
        
        public void ResolveDependencies()
        {
            var monoBehaviors = Resources.FindObjectsOfTypeAll<MonoBehaviour>();

            CollectInstancesFromHierarchy(monoBehaviors);
            InjectDependencies(monoBehaviors);
        }

        private void CollectInstancesFromHierarchy(MonoBehaviour[] behaviors)
        {
            foreach (var behaviour in behaviors)
            {
                if (gameObject.scene != behaviour.gameObject.scene)
                {
                    continue;
                }
                
                var type = behaviour.GetType();
                BindHierarchyInstance(type, behaviour);

                var interfaces = type.GetInterfaces();

                foreach (var @interface in interfaces)
                {
                    BindHierarchyInstance(@interface, behaviour);
                }
            }
        }
        
        bool HasInjectAttribute(MemberInfo memberInfo)
        {
            return Attribute.GetCustomAttribute(memberInfo, typeof(InjectAttribute), false) != null;
        }

        private void BindHierarchyInstance(Type type, MonoBehaviour instance)
        {
            if (_instancesToCollect.Contains(type))
            {
                BindInstance(type, instance);
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
                var methods = target.GetType().GetMethods(Instance | Public);
                if (methods.Length == 0)
                {
                    continue;
                }

                foreach (var method in methods)
                {
                    if (!HasInjectAttribute(method)) continue;
                    
                    var parameters = method.GetParameters();
                    foreach (var parameter in parameters)
                    {
                        if (_bindings.TryGetValue(parameter.ParameterType, out var binding))
                        {
                            parameterList.Add(binding.ResolveInstanceToInject(target.GetType()));
                        }
                    }

                    method.Invoke(target, parameterList.ToArray());
                }

                
                parameterList.Clear();
            }
        }

        public BindingBuilder<T> Bind<T>()
        {
            return BindingBuilder<T>.Bind<T>(this);
        }

        public class BindingBuilder<T>
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
            private object _instance;


            public static BindingBuilder<T> Bind<T>(Container container)
                => new BindingBuilder<T>(container, typeof(T));

            public BindingBuilder<T> To<TConcrete>()
            {
                _concreteType = typeof(TConcrete);
                return this;
            }

            public BindingBuilder<T> WhenInjectedInto<TTarget>()
            {
                _conditionalTargetType = typeof(TTarget);
                return this;
            }

            public BindingBuilder<T> FromInstance<TInstance>(TInstance instance)
            {
                _instance = instance;
                return this;
            }
            public BindingBuilder<T> FromComponentInHierarchy()
            {
                _container._instancesToCollect.Add(_type);
                return this;
            }
            
            public void AsSingle()
            {
                if (IsConditionalBinding)
                {
                    BindConditional();
                }
                else
                {
                    Bind();
                }
                SetInstance();
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

            private void Bind()
            {
                _container._bindings[_type] = new ConcreteBindingDefinition(_type);
            }

            private void SetInstance()
            {
                if (_instance != null)
                {
                    _container._bindings[_type].SetInstance(_instance);
                }
            }

            public void FromPrototype<TProto>(TProto prototype) where TProto : class, ICloneable
            {
                _container._bindings[_type] = new PrototypeBindingDefinition<TProto>(_type, prototype);
            }
        }

    }
}
