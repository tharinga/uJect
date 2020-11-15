using System;

namespace uJect
{
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property)]
    public class InjectAttribute : Attribute
    {
    }
}