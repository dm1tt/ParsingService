
using System.Text.RegularExpressions;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using ParsingService.Models;

public class AmperoParseService : IAmperoParseService //TODO обсудить что такое count и зачем оно нужно
{
    private HttpClient _httpClient;
    public AmperoParseService()
    {
        _httpClient = new HttpClient();   
    }
    public async Task<List<ProductLink>> GetProdictListHtml(string url, string queryMessage)
    {
        var productLinkList = new List<ProductLink>();

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
            int priceValue = 0;
            if (priceText != null)
            {
                var priceMatch = Regex.Match(priceText, @"Цена:\s*(\d+)");
                if (priceMatch.Success)
                {
                    priceValue = Convert.ToInt32(priceMatch.Groups[1].Value);
                }
            }

            var productUrl = productNode.QuerySelector("a.search-link")?.GetAttribute("href");

            if (!string.IsNullOrEmpty(productName) && !string.IsNullOrEmpty(productUrl))
            {
                var product = new Product
                {
                    ProductId = Guid.NewGuid().ToString(),
                    Name = productName,
                    Description = null,
                    Image = null,
                    ProductType = null
                };
                var productLink = new ProductLink
                {
                    Id = Guid.NewGuid().ToString(),
                    Link = productUrl,
                    SiteName = "Ampero",
                    Price = priceValue,
                    InStock = null,
                    Count = totalResults,
                    Product = product
                };

                productLinkList.Add(productLink);
            }
        }

        int numberOfPage = 2;
        
        while(productLinkList.Count < totalResults)
        {
            html = await _httpClient.GetStringAsync(url + queryMessage + $"&page={numberOfPage}");
            doc = parser.ParseDocument(html);
            productNodes = doc.QuerySelectorAll(".mse2-row");

            foreach (var productNode in productNodes)
            {
                var productName = productNode.QuerySelector(".search-link")?.TextContent.Trim();

                var priceNode = productNode.QuerySelector("div.col-md-10.col-sm-8.col-xs-8.bottommargin-xs");
                var priceText = priceNode?.TextContent;
                int priceValue = 0;
                if (priceText != null)
                {
                    var priceMatch = Regex.Match(priceText, @"Цена:\s*(\d+)");
                    if (priceMatch.Success)
                    {
                        priceValue = Convert.ToInt32(priceMatch.Groups[1].Value);
                    }
                }

                var productUrl = productNode.QuerySelector("a.search-link")?.GetAttribute("href");

                if (!string.IsNullOrEmpty(productName) && !string.IsNullOrEmpty(productUrl))
                {
                    var product = new Product
                    {
                        ProductId = Guid.NewGuid().ToString(),
                        Name = productName,
                        Description = null,
                        Image = null,
                        ProductType = null
                    };

                    var productLink = new ProductLink
                    {
                        Id = Guid.NewGuid().ToString(),
                        Link = productUrl,
                        SiteName = "Ampero",
                        Price = priceValue,
                        InStock = null,
                        Count = totalResults,
                        Product = product
                    };

                    productLinkList.Add(productLink);
                }
            }

            numberOfPage++;
        }
        
        

        return productLinkList;
    }

    private int GetTotalResults(IDocument document)
    {
        var totalResultsElements = document.QuerySelector("#mse2_total");
        return totalResultsElements != null ? int.Parse(totalResultsElements.TextContent) : 0;
    }
}