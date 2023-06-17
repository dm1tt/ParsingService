var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var a = new AmperoParseService();
var baseUrl = builder.Configuration.GetValue<string>("BaseUrls:AmperoGlobalSearchBaseUrl");
app.MapGet("/", async () => 
{
    var result = await a.GetProdictListHtml(baseUrl!, "ATMEGA8A-PU");

    return result;
}
);
app.Run();
