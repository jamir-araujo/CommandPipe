namespace System
{
    internal static class TypeExtensions
    {
        public static bool IsPublicConcreteClass(this Type type)
        {
            return (type.IsPublic || type.IsNestedPublic) && type.IsClass && !type.IsAbstract;
        }
    }
}
