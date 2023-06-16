using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;

public class AmperoParseService : IAmperoParseService
{
    private HttpClient _httpClient;
    public AmperoParseService()
    {
        _httpClient = new HttpClient();   
    }
    public async Task<List<string>> GetProdictListHtml(string url, string queryMessage)
    {
        var productList = new List<string>();
        string html = await _httpClient.GetStringAsync(url + queryMessage);

        var parser = new HtmlParser();
        var doc = parser.ParseDocument(html);

        int totalResults = GetTotalResults(doc);
        var productLinks = doc.QuerySelectorAll("a.search-link").Select(link => link.Text());
        productList.AddRange(productLinks);

        int numberOfPage = 2;
        
        while(productList.Count < totalResults)
        {
            html = await _httpClient.GetStringAsync(url + queryMessage + $"&page={numberOfPage}");
            doc = parser.ParseDocument(html);
            productLinks = doc.QuerySelectorAll("a.search-link").Select(link => link.Text());
        
            productList.AddRange(productLinks);
            numberOfPage++;
        }
        
        return productList;
    }

    private int GetTotalResults(IDocument document)
    {
        var totalResultsElements = document.QuerySelector("#mse2_total");
        return totalResultsElements != null ? int.Parse(totalResultsElements.TextContent) : 0;
    }
}