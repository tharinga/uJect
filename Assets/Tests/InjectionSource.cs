using UnityEngine;

public class InjectionSource : MonoBehaviour, IInjectionSource
{
    public string Value => "A";
}

public class AlternateInjectionSource : MonoBehaviour, IInjectionSource
{
    public string Value => "B";
}