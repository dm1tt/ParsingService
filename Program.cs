var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var a = new AmperkotParseService();
var baseUrl = builder.Configuration.GetValue<string>("BaseUrls:AmperkotGlobalSearchBaseUrl");
app.MapGet("/", async () => 
{
    var result = await a.GetProdictLinkList(baseUrl!, "микроконтроллеры");

    return result;
}
);
app.Run();
