using Microsoft.EntityFrameworkCore;
using ParsingServise.DataBasesContext;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var a = new AmperkotParseService();
var baseUrl = builder.Configuration.GetValue<string>("BaseUrls:AmperkotGlobalSearchBaseUrl");

builder.Services.AddDbContext<MsSqlContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MsSql")));

app.MapGet("/", async () => 
{
    var result = await a.GetProdictLinkListAsync(baseUrl!, "arduino");

    return result;
}
);
app.Run();
\