using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using uJect;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace Tests
{
    public class InjectTests
    {
        private Container _container;
        private List<MonoBehaviour> _destroyables = new List<MonoBehaviour>();
        
        TTarget ArrangeTarget<TTarget>() 
            where TTarget : MonoBehaviour 
        {
            var target = new GameObject("Target").AddComponent<TTarget>();
            _destroyables.Add(target);
            return target;
        }
        
        TTarget ArrangeSourceAndTarget<TSource, TTarget>() 
            where TTarget : MonoBehaviour 
            where TSource : MonoBehaviour 
        {
            var target = new GameObject("Target").AddComponent<TTarget>();
            var source = new GameObject("Source").AddComponent<TSource>();
            _destroyables.Add(target);
            _destroyables.Add(source);
            return target;
        }

        [SetUp]
        public void SetUp()
        {
            _container = new GameObject("Container").AddComponent<Container>();
        }
        
        [Test]
        public void Inject_MonoBehaviour_From_Scene_Into_Target_With_Concrete_Dependency()
        {
            var target = ArrangeSourceAndTarget<InjectionSource, InjectionTargetWithConcreteDependency>();
            
            _container.Bind<InjectionSource>().FromComponentInHierarchy().AsSingle();
            _container.ResolveDependencies();
            
            Assert.That(target.Value, Is.EqualTo("A"));
        }
        
        [Test]
        public void Inject_MonoBehaviour_From_Scene_Into_Target_With_Interface_Dependency()
        {
            var target = ArrangeSourceAndTarget<InjectionSource, InjectionTargetWithInterfaceDependency>();
            
            _container.Bind<IInjectionSource>().To<InjectionSource>().FromComponentInHierarchy().AsSingle();
            _container.ResolveDependencies();
            
            Assert.That(target.Value, Is.EqualTo("A"));
        }
        
        [Test]
        public void Inject_Multiple_MonoBehaviours_Into_Multiple_Targets()
        {
            var target1 = ArrangeSourceAndTarget<InjectionSource, InjectionTargetWithConcreteDependency>();
            var target2 = ArrangeSourceAndTarget<InjectionSource, InjectionTargetWithInterfaceDependency>();
            
            _container.Bind<InjectionSource>().FromComponentInHierarchy().AsSingle();
            _container.Bind<IInjectionSource>().To<InjectionSource>().FromComponentInHierarchy().AsSingle();
            _container.ResolveDependencies();
            
            Assert.That(target1.Value, Is.EqualTo("A"));
            Assert.That(target2.Value, Is.EqualTo("A"));
        }
        
        [Test]
        public void Inject_Conditional_Into_Specified_Target()
        {
            var target1 = ArrangeSourceAndTarget<InjectionSource, InjectionTargetWithInterfaceDependency>();
            var target2 = ArrangeSourceAndTarget<AlternateInjectionSource, AlternateInjectionTargetWithInterfaceDependency>();
            
            _container.Bind<IInjectionSource>().To<InjectionSource>().WhenInjectedInto<InjectionTargetWithInterfaceDependency>().FromComponentInHierarchy().AsSingle();
            _container.Bind<IInjectionSource>().To<AlternateInjectionSource>().WhenInjectedInto<AlternateInjectionTargetWithInterfaceDependency>().FromComponentInHierarchy().AsSingle();
            _container.ResolveDependencies();
            
            Assert.That(target1.Value, Is.EqualTo("A"));
            Assert.That(target2.Value, Is.EqualTo("B"));
        }

        [Test]
        public void Inject_From_Instance_Into_Target_With_Concrete_Dependency()
        {
            var target = ArrangeTarget<InjectionTargetWithPocoDependency>();
            
            _container.Bind<PocoInjectionSource>().FromInstance(new PocoInjectionSource()).AsSingle();
            _container.ResolveDependencies();
            
            Assert.That(target.Value, Is.EqualTo("C"));
        }
        
        [Test]
        public void Inject_From_Instance_Into_Target_With_Interface_Dependency()
        {
            var target = ArrangeTarget<InjectionTargetWithInterfaceDependency>();
            
            _container.Bind<IInjectionSource>().To<PocoInjectionSource>().FromInstance(new PocoInjectionSource()).AsSingle();
            _container.ResolveDependencies();
            
            Assert.That(target.Value, Is.EqualTo("C"));
        }

        [Test]
        public void Inject_From_Prototype()
        {
            var target = ArrangeTarget<InjectionTargetWithPrototypeDependency>();
            var prototype = new PrototypeInjectionSource();

            _container.Bind<PrototypeInjectionSource>().FromPrototype(prototype);
            _container.ResolveDependencies();
            
            Assert.That(target.Value, Is.EqualTo("D"));
            Assert.That(target.Dependency, Is.Not.SameAs(prototype));
        }
        
        [UnityTearDown]
        public IEnumerator TearDown()
        {
            foreach (var monoBehaviour in _destroyables)
            {
                Object.DestroyImmediate(monoBehaviour.gameObject);
            }
            Object.DestroyImmediate(_container.gameObject);
            _destroyables.Clear();
            yield return null;
        }
    }
}
