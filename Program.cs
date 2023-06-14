
using ParsingService.ChipDipParseService;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var a = new ChipDipParseService();
var baseUrl = builder.Configuration.GetValue<string>("ChipDipGlobalSearchBaseUrl");
app.MapGet("/", async () => 
{
    var result = await a.GetProdictListHtml(baseUrl!, "микроконтроллеры");

    return result;
}
);
app.Run();
