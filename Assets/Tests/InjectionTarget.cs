using uJect;
using UnityEngine;

public class InjectionTargetWithConcreteDependency : MonoBehaviour
{
    private InjectionSource _injectionSource;

    [Inject]
    public void Inject(InjectionSource injectionSource)
    {
        _injectionSource = injectionSource;
    }

    public string Value => _injectionSource.Value;
}

public class InjectionTargetWithInterfaceDependency : MonoBehaviour
{
    private IInjectionSource _injectionSource;

    [Inject]
    public void Inject(IInjectionSource injectionSource)
    {
        _injectionSource = injectionSource;
    }

    public string Value => _injectionSource.Value;
}

public class AlternateInjectionTargetWithInterfaceDependency : MonoBehaviour
{
    private IInjectionSource _injectionSource;

    [Inject]
    public void Inject(IInjectionSource injectionSource)
    {
        _injectionSource = injectionSource;
    }

    public string Value => _injectionSource.Value;
}

public class InjectionTargetWithPocoDependency : MonoBehaviour
{
    private PocoInjectionSource _injectionSource;

    [Inject]
    public void Inject(PocoInjectionSource injectionSource)
    {
        _injectionSource = injectionSource;
    }

    public string Value => _injectionSource.Value;
}

public class InjectionTargetWithPrototypeDependency : MonoBehaviour
{
    private PrototypeInjectionSource _injectionSource;

    [Inject]
    public void Inject(PrototypeInjectionSource injectionSource)
    {
        _injectionSource = injectionSource;
    }

    public string Value => _injectionSource.Value;
    public PrototypeInjectionSource Dependency => _injectionSource;
}