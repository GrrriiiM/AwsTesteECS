using System;

namespace TesteECS
{
    [System.AttributeUsage(System.AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    sealed class InjectableAttribute : System.Attribute
    {

    }
}