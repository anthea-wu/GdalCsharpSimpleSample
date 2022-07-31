using gdal_smaple;
using Microsoft.Extensions.DependencyInjection;

var host = new Host().Build();
var app = host.Services.GetService<AppService>();
app.Run();