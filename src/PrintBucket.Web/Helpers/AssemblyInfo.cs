using System.Reflection;

namespace PrintBucket.Web.Helpers
{
    public static class AssemblyInfo
    {
        public static string Version =>
            (Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly())
                .GetName()
                .Version?
                .ToString() ?? "0.0.0";
    }
}