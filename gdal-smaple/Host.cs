using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace gdal_smaple;

public class Host
{
    public IHost Build()
    {
        var builder = new HostBuilder();
        builder.ConfigureServices((_, services) =>
        {
            services.AddSingleton<AppService>();
            services.AddSingleton<GdalService>();
            services.AddSingleton<SampleService>();
        });
        return builder.Build();
    }
}