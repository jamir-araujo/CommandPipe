using System.Threading.Tasks;

namespace System
{
    internal static class ObjectExtensions
    {
        public static Task<T> AsTask<T>(this T val) => Task.FromResult(val);
    }
}
