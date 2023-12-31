using System.Diagnostics;
using System.Text.RegularExpressions;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using ParsingService.Models;

public class AmperoParseService : IParseService //TODO пофиксить парсинг цены. Когда ценга > 1000 парсит только первую цифру
{
    
    private readonly HttpClient _httpClient;
    public AmperoParseService()
    {
        _httpClient = new HttpClient();   
    }
    public async Task<List<ProductLink>> GetProdictLinkListAsync(string url, string queryMessage)
    {
        var sw = new Stopwatch();
        sw.Start();
        var productLinkList = new List<ProductLink>();

        string html = await _httpClient.GetStringAsync(url + "search?query=" + queryMessage + "&limit=100");
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
                var priceMatch = Regex.Match(priceText, @"Цена:\s*(\d+(\s\d{3})*)\s*руб\.");
                if (priceMatch.Success)
                {
                    priceValue = Convert.ToInt32(priceMatch.Groups[1].Value.Replace(" ", ""));
                }
            }

            int productCount = 0;

            var productCountText = productNode.QuerySelector("span.value")?.TextContent;
            if(productCountText != null)
            {
                var productCountMatch = Regex.Match(productCountText, @"в наличии \s*(\d+)");
                if(productCountMatch.Success)
                {
                    productCount = Convert.ToInt32(productCountMatch.Groups[1].Value);
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
                bool productInStock = false;
                if(productCount != 0)
                {
                    productInStock = true;
                }
                var productLink = new ProductLink
                {
                    Id = Guid.NewGuid().ToString(),
                    Link = productUrl,
                    SiteName = "Ampero",
                    Price = priceValue,
                    InStock = productInStock,
                    Count = productCount,
                    Product = product
                };

                productLinkList.Add(productLink);
            }
        }

        int numberOfPage = 2;
        
        while(productLinkList.Count < totalResults)
        {
            html = await _httpClient.GetStringAsync(url + "search?query=" + queryMessage + $"&page={numberOfPage}&limit=100");
            doc = parser.ParseDocument(html);
            productNodes = doc.QuerySelectorAll(".mse2-row");

            foreach (var productNode in productNodes)
            {
                var productName = productNode.QuerySelector(".search-link")?.TextContent.Trim();
                var productUrl = productNode.QuerySelector("a.search-link")?.GetAttribute("href");

                var priceNode = productNode.QuerySelector("div.col-md-10.col-sm-8.col-xs-8.bottommargin-xs");
                var priceText = priceNode?.TextContent;
                int priceValue = 0;
                if (priceText != null)
                {
                    var priceMatch = Regex.Match(priceText, @"Цена:\s*(\d+(\s\d{3})*)\s*руб\.");
                    if (priceMatch.Success)
                    {
                        priceValue = Convert.ToInt32(priceMatch.Groups[1].Value.Replace(" ", ""));
                    }
                }

                int productCount = 0;
                var productCountText = productNode.QuerySelector("span.value")?.TextContent;
                if(productCountText != null)
                {
                    var productCountMatch = Regex.Match(productCountText, @"в наличии \s*(\d+)");
                    if(productCountMatch.Success)
                    {
                        productCount = Convert.ToInt32(productCountMatch.Groups[1].Value);
                    }
                }


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

                    bool productInStock = false;
                    if(productCount != 0)
                    {
                        productInStock = true;
                    }

                    var productLink = new ProductLink
                    {
                        Id = Guid.NewGuid().ToString(),
                        Link = productUrl,
                        SiteName = "Ampero",
                        Price = priceValue,
                        InStock = productInStock,
                        Count = productCount,
                        Product = product
                    };

                    productLinkList.Add(productLink);
                }
            }

            numberOfPage++;
        }
        sw.Stop();
        Console.WriteLine(sw.Elapsed);
        return productLinkList;
    }

    private int GetTotalResults(IDocument document)
    {
        var totalResultsElements = document.QuerySelector("#mse2_total");
        return totalResultsElements != null ? int.Parse(totalResultsElements.TextContent) : 0;
    }

    public async Task<ProductLink> GetOneProductLinkAsync(string baseUrl, string productUrl)
    {
        string html = await _httpClient.GetStringAsync(baseUrl + productUrl);
        return null!;
    }
}