using System;

namespace CommandPipe.Annotations
{
    /// <summary>
    /// Represents a marker to exclude a class from an assembly read and registry process.
    /// </summary>
    /// <remarks>
    /// Marking a class with this attribute will exclude it from processes that read an assembly
    /// and register its classes as services on the <see cref="IServiceCollection"/>.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class IgnoreOnAssemblyReadAttribute : Attribute
    {
    }
}
