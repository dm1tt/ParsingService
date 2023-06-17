
using System.Text.RegularExpressions;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using ParsingService.Models;

public class AmperoParseService : IAmperoParseService
{
    private HttpClient _httpClient;
    public AmperoParseService()
    {
        _httpClient = new HttpClient();   
    }
    public async Task<List<Product>> GetProdictListHtml(string url, string queryMessage)
    {
        var productList = new List<Product>();

        string html = await _httpClient.GetStringAsync(url + queryMessage);
        var parser = new HtmlParser();
        var doc = parser.ParseDocument(html);

        int totalResults = GetTotalResults(doc);

        var productNodes = doc.QuerySelectorAll(".mse2-row");

        foreach (var productNode in productNodes)
        {
            var productName = productNode.QuerySelector(".search-link")?.TextContent.Trim();
            var priceNode = productNode.QuerySelector("div.col-md-10.col-sm-8.col-xs-8.bottommargin-xs");
            var priceText = priceNode?.TextContent;
            if (priceText != null)
            {
                var priceMatch = Regex.Match(priceText, @"Цена:\s*(\d+)");
                if (priceMatch.Success)
                {
                    var priceValue = priceMatch.Groups[1].Value;
                }
            }

            var productLink = productNode.QuerySelector("a.search-link")?.GetAttribute("href");

            if (!string.IsNullOrEmpty(productName) && !string.IsNullOrEmpty(productLink))
            {
                var product = new Product
                {
                    ProductId = Guid.NewGuid().ToString(),
                    Name = productName,
                    Description = null,
                    Image = null,
                    Link = productLink,
                    ProductType = null
                };

                productList.Add(product);
            }
        }

        int numberOfPage = 2;
        
        while(productList.Count < totalResults)
        {
            html = await _httpClient.GetStringAsync(url + queryMessage + $"&page={numberOfPage}");
            doc = parser.ParseDocument(html);
            productNodes = doc.QuerySelectorAll(".mse2-row");

            foreach (var productNode in productNodes)
            {
                var productName = productNode.QuerySelector(".search-link")?.TextContent.Trim();
                var productLink = productNode.QuerySelector("a.search-link")?.GetAttribute("href");

                if (!string.IsNullOrEmpty(productName) && !string.IsNullOrEmpty(productLink))
                {
                    var product = new Product
                    {
                        ProductId = Guid.NewGuid().ToString(),
                        Name = productName,
                        Description = null,
                        Image = null,
                        Link = productLink,
                        ProductType = null
                    };

                    productList.Add(product);
                }
            }

            numberOfPage++;
        }
        
        List<Product> products = productList.Select(product => new Product
        {
            ProductId = product.ProductId,
            Name = product.Name,
            Description = null,
            Image = null,
            Link = product.Link,
            ProductType = null
        }).ToList();

        return products;
    }

    private int GetTotalResults(IDocument document)
    {
        var totalResultsElements = document.QuerySelector("#mse2_total");
        return totalResultsElements != null ? int.Parse(totalResultsElements.TextContent) : 0;
    }
}