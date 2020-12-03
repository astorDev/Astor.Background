using System.Reflection;
using System.Threading.Tasks;

namespace Astor.Background
{
    public static class MethodInfoExtensions
    {
        public static async Task<object> InvokeAsync(this MethodInfo method, object instance, params object[] args)
        {
            if (!method.IsAsync())
            {
                return method.Invoke(instance, args);
            }
            
            if (method.ReturnType.IsGenericType)
            {
                return (object) await (dynamic) method.Invoke(instance, args);
            }

            await (Task) method.Invoke(instance, args);
            return null;

        }

        public static bool IsAsync(this MethodInfo method)
        {
            return method.ReturnType.GetMethod(nameof(Task.GetAwaiter)) != null;
        }
    }
}