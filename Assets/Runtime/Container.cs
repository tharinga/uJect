using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Container : MonoBehaviour
{
    private Dictionary<Type, object> _singletonInstances = new Dictionary<Type, object>();
    private HashSet<Type> _singletonBindingTypes = new HashSet<Type>();
    
    void ScanScene()
    {
        var monoBehaviors = Resources.FindObjectsOfTypeAll<MonoBehaviour>();

        CollectDependencyInstances(monoBehaviors);
        InjectDependencies(monoBehaviors);
    }

    void CollectDependencyInstances(MonoBehaviour[] behaviors)
    {
        foreach (var behaviour in behaviors)
        {
            if (_singletonBindingTypes.Contains(behaviour.GetType()))
            {
                _singletonInstances[behaviour.GetType()] = behaviour;
            }
        }
    }
    
    void InjectDependencies(MonoBehaviour[] behaviors)
    {
        var parameterList = new List<object>(8);

        foreach (var behaviour in behaviors)
        {
            var method = behaviour.GetType().GetMethod("Inject", BindingFlags.Instance | BindingFlags.NonPublic);
            if (method != null)
            {
                var parameters = method.GetParameters();
                foreach (var parameter in parameters)
                {
                    if (_singletonInstances.TryGetValue(parameter.ParameterType, out var parameterInstance))
                    {
                        parameterList.Add(parameterInstance);
                    }
                }

                method.Invoke(behaviour, parameterList.ToArray());
            }
            parameterList.Clear();
        }
    }

    public void Bind<T>()
    {
        _singletonBindingTypes.Add(typeof(T));
        ScanScene();

    }
  
}
