using System.Runtime.InteropServices;

namespace gdal_smaple;

public class External
{
    [DllImport("gdal305.dll", EntryPoint = "OGR_F_GetFieldAsString", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr OGR_F_GetFieldAsString(HandleRef handle, int index); 
}