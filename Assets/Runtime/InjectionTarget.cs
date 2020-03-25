using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InjectionTarget : MonoBehaviour
{
    private InjectionSource _injectionSource;

    private void Inject(InjectionSource injectionSource)
    {
        _injectionSource = injectionSource;
    }

    public int Value => _injectionSource.Value;
}

