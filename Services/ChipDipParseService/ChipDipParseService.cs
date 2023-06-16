using System.Net;
using AngleSharp;
using AngleSharp.Dom;

namespace ParsingService.ChipDipParseService;
public class ChipDipParseService : IChipDipParseService 
{
    private HttpClient _httpClient;
    public ChipDipParseService()
    {
        _httpClient = new HttpClient();
    }

    public async Task<List<string>> GetProdictListHtml(string baseUrl, string queryMessage)
    {
        var config = Configuration.Default;
        var testList = new List<string>();

        int numberOfPage = 1;
        while(true)
        {
            string searchHtml = String.Empty;
            try
            {
                searchHtml = _httpClient.GetStringAsync(baseUrl + queryMessage + $"&page={numberOfPage}").Result;

                using var context = BrowsingContext.New(config);
                using var doc = await context.OpenAsync(req => req.Content(searchHtml));
                var els = doc.QuerySelectorAll("a")
                            .Where(item => item.ClassName != null && item.ClassName
                            .Contains("link link_dark no-visited group-header font-m"));
                
                if(els == null)
                {
                    
                    break;
                }

                testList.AddRange(els.Select(el => el.Text()));
                
                numberOfPage++;
            }
            catch(Exception e)
            {
                if(e.Message.Contains("429"))
                {
                    break;
                }
                else throw new Exception(e.Message);
            }

        }

        return testList;
    }

}