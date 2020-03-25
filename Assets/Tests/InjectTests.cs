using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class InjectTests
    {
        [Test]
        public void Inject_Monobehavior_From_Scene_NotNull()
        {
            var target = new GameObject("Target").AddComponent<InjectionTarget>();
            var source = new GameObject("Source").AddComponent<InjectionSource>();
            var container = new GameObject("Container").AddComponent<Container>();
            
            container.Bind<InjectionSource>();
            
            Assert.That(target.Value, Is.EqualTo(10));
        }
    }
}
