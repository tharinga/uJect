using UnityEngine;

public class InjectionTargetWithConcreteDependency : MonoBehaviour
{
    private InjectionSource _injectionSource;

    public void Inject(InjectionSource injectionSource)
    {
        _injectionSource = injectionSource;
    }

    public string Value => _injectionSource.Value;
}

public class InjectionTargetWithInterfaceDependency : MonoBehaviour
{
    private IInjectionSource _injectionSource;

    public void Inject(IInjectionSource injectionSource)
    {
        _injectionSource = injectionSource;
    }

    public string Value => _injectionSource.Value;
}

public class AlternateInjectionTargetWithInterfaceDependency : MonoBehaviour
{
    private IInjectionSource _injectionSource;

    public void Inject(IInjectionSource injectionSource)
    {
        _injectionSource = injectionSource;
    }

    public string Value => _injectionSource.Value;
}

public class InjectionTargetWithPocoDependency : MonoBehaviour
{
    private PocoInjectionSource _injectionSource;

    public void Inject(PocoInjectionSource injectionSource)
    {
        _injectionSource = injectionSource;
    }

    public string Value => _injectionSource.Value;
}
