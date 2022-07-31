using System.Runtime.InteropServices;
using System.Text;
using MaxRev.Gdal.Core;
using OSGeo.GDAL;
using OSGeo.OGR;
using OSGeo.OSR;

public class GdalService
{
    public void Register(string encoding)
    {
        GdalBase.ConfigureAll();
        Gdal.SetConfigOption("SHAPE_ENCODING", encoding);
        Gdal.AllRegister();
        Ogr.RegisterAll();
    }

    public void Dispose(DataSource source)
    {
        source.Dispose();
    }

    public DataSource Load(string path)
    {
        return Ogr.Open(path, (int)EnumShapefile.ReadOnly);
    }

    public List<Shp> Read(DataSource source)
    {
        var shps = new List<Shp>();
        var layerCount = source.GetLayerCount();
        for (var i = 0; i < layerCount; i++)
        {
            // 讀取圖層 Layer 資訊
            var layer = source.GetLayerByIndex(i);
            var fieldCount = layer.GetLayerDefn().GetFieldCount();
            for (var j = 0; j < fieldCount; j++)
            {
                var field = layer.GetLayerDefn().GetFieldDefn(j);
                var fieldName = GetFieldDfnName(field);
            }
            
            
            // 讀取要素 Feature
            Feature feature;
            while ((feature = layer.GetNextFeature()) != null)
            {
                var id = feature.GetFieldAsInteger((int)EnumShapefileColumn.Id);
                var name = feature.GetFieldAsString((int)EnumShapefileColumn.Name);
                var area = feature.GetFieldAsDouble((int)EnumShapefileColumn.Area);
                var geometry = feature.GetGeometryRef();
                geometry.ExportToWkt(out var wkt);
                
                var shp = new Shp()
                {
                    Id = id,
                    Name = name,
                    Area = area,
                    Wkt = wkt
                };
                shps.Add(shp);
            }
        }
        
        return shps;
    }

    public void Create(string path, string spatialReferenceSystem, Shp shp)
    {
        var driver = Ogr.GetDriverByName("ESRI Shapefile");
        var source = driver.CreateDataSource(path, null);
        var spatialReference = new SpatialReference("");
        spatialReference.SetWellKnownGeogCS(spatialReferenceSystem);
        var layer = source.CreateLayer("layer one", spatialReference, wkbGeometryType.wkbPolygon,
            new[] { "ENCODING=BIG5" });

        var fieldId = new FieldDefn("Id", FieldType.OFTInteger);
        layer.CreateField(fieldId, (int)EnumShapefileColumn.Id);
        var fieldName = new FieldDefn("土地名稱", FieldType.OFTString);
        layer.CreateField(fieldName, (int)EnumShapefileColumn.Name);
        var fieldArea = new FieldDefn("土地面積", FieldType.OFTString);
        layer.CreateField(fieldArea, (int)EnumShapefileColumn.Area);

        var field = layer.GetLayerDefn();
        var feature = new Feature(field);
        feature.SetField((int)EnumShapefileColumn.Id, shp.Id);
        feature.SetFieldBinaryFromHexString((int)EnumShapefileColumn.Name, GetHexString(shp.Name));
        feature.SetField((int)EnumShapefileColumn.Area, shp.Area);
        var geometry = Geometry.CreateFromWkt(shp.Wkt);
        feature.SetGeometry(geometry);
        layer.CreateFeature(feature);
    }

    private static string GetHexString(string origin)
    {
        var bytes = Encoding.GetEncoding("UTF8").GetBytes(origin);
        return Convert.ToHexString(bytes);
    }

    [DllImport("gdal/x64/gdal305.dll", EntryPoint = "OGR_Fld_GetNameRef", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr OGR_Fld_GetNameRef(HandleRef handle);

    private string GetFieldDfnName(FieldDefn fieldDefn)
    {
        var handle = FieldDefn.getCPtr(fieldDefn);
        var ptr = OGR_Fld_GetNameRef(handle);
        return Utf8BytesToString(ptr);
    }

    private string Utf8BytesToString(IntPtr ptr)
    {
        if (ptr == IntPtr.Zero)
            return "";

        var ms = new MemoryStream();
        byte b;
        var ofs = 0;
        while ((b = Marshal.ReadByte(ptr, ofs++)) != 0)
        {
            ms.WriteByte(b);
        }

        return Encoding.UTF8.GetString(ms.ToArray());
    }
}

public class Shp
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double Area { get; set; }
    public string Wkt { get; set; }
}