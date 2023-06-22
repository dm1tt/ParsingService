
using System.Text.RegularExpressions;
using AngleSharp.Html.Parser;
using ParsingService.Models;

public class AmperkotParseService
{
    private readonly HttpClient _httpClient;
    public AmperkotParseService()
    {
        _httpClient = new HttpClient();
    }
    public async Task<List<Product>> GetProdictLinkList(string url, string queryMessage)
    {
        var productList = new List<Product>();

        string html = await _httpClient.GetStringAsync(url + queryMessage);
        var parser = new HtmlParser();
        var doc = parser.ParseDocument(html);

        var productNodes = doc.QuerySelectorAll(".product-item");
        foreach(var productNode in productNodes)
        {
            var productName = productNode.QuerySelector(".body")?.TextContent.Trim();
            var priceNode = productNode.QuerySelector(".price-current.pull-right");
            var priceText = priceNode?.TextContent.Trim();
            int priceValue = 0;
            
            if(priceText != null)
            {
                priceValue = Convert.ToInt32(priceText?.Replace("руб.", "").Trim());
            }
            
            if(!string.IsNullOrEmpty(productName))
            {
                var product = new Product
                {
                    ProductId = Guid.NewGuid().ToString(),
                    Name = productName,
                    Description = null,
                    Image = null,
                    ProductType = null
                };
                productList.Add(product);
            }
        }
        return productList;
    }
}