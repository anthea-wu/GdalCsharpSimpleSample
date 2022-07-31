using System.Text;
using OSGeo.OSR;

public class AppService
{
    private readonly GdalService _gdalService;
    private readonly SampleService _sampleService;

    public AppService(GdalService gdalService, SampleService sampleService)
    {
        _gdalService = gdalService;
        _sampleService = sampleService;
    }

    public void Run()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        
        _sampleService.ReadShapefile("files/simple/test.shp");
        _sampleService.CreateShapefile("files/simple/create.shp");
    }
}