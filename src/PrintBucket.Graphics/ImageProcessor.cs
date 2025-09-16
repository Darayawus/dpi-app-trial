using NetVips;

namespace PrintBucket.Graphics
{
    public class ImageProcessor
    {
        public static void Initialization()
        {
            ModuleInitializer.Initialize();
            if (ModuleInitializer.VipsInitialized)
            {
                Serilog.Log.Debug($"VipsInitialized OK");
            }
            else
            {
                Serilog.Log.Debug($"VipsInitialized ERROR");
            }
        }

        public static bool IsNetVipsAvailable()
        {
            try
            {
                return ModuleInitializer.VipsInitialized;
            }
            catch
            {
                return false;
            }
        }

        public static string GetNetVipsVersion()
        {
            return NetVips.NetVips.Version(0).ToString();
        }
    }
}