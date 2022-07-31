public class SampleService
{
    private readonly GdalService _gdalService;

    public SampleService(GdalService gdalService)
    {
        _gdalService = gdalService;
    }

    public void ReadShapefile(string path)
    {
        _gdalService.Register("big5");
        var source = _gdalService.Load(path);
        var shps = _gdalService.Read(source);
        Console.WriteLine(shps[0].Wkt);
        _gdalService.Dispose(source);
    }

    public void CreateShapefile(string path)
    {
        _gdalService.Register("");
        
        var newShp = new Shp()
        {
            Id = 2, 
            Name = "第二塊土地",
            Area = 123.4,
            Wkt = "POLYGON((120.64645795607646 24.181909471753926,120.65184383177836 24.181909471753926,120.65184383177836 24.178092338284888,120.64645795607646 24.178092338284888,120.64645795607646 24.181909471753926))"
        };
        _gdalService.Create(path, "WGS84", newShp);
    }
}