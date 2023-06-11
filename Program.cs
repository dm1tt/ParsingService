
using ParsingService.ChipDipParseService;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
var a = new ChipDipParseService();
System.Console.WriteLine(a.GetProdictListHtml("микроконтроллеры"));
app.Run();
