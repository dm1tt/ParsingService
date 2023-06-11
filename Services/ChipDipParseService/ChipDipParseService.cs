using System.Net;

namespace ParsingService.ChipDipParseService;
public class ChipDipParseService : IChipDipParseService 
{
    private readonly string _chipDipGlobalSearchBaseUrl = "https://www.chipdip.ru/search?searchtext="; //TODO перенести в жсон
    public string GetProdictListHtml(string url) //TODO доделать. спарсить хтмл
    {
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        HttpClient client = new HttpClient();
        var searchHtml = client.GetStringAsync(_chipDipGlobalSearchBaseUrl + url).Result;
        return searchHtml;
    }

}