
using System.Diagnostics;
using System.Text.RegularExpressions;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using ParsingService.Models;

public class AmperkotParseService
{
    private readonly HttpClient _httpClient; //TODO спарсить inStock
    public AmperkotParseService()
    {
        _httpClient = new HttpClient();
    }
    public async Task<List<ProductLink>> GetProdictLinkList(string url, string queryMessage)
    {
        var sw = new Stopwatch();
        sw.Start();
        var productLinkList = new List<ProductLink>();

        string html = await _httpClient.GetStringAsync(url + queryMessage + "&page=1");
        var parser = new HtmlParser();
        var doc = parser.ParseDocument(html);
        int totalResults = GetTotalResults(doc);

        var productNodes = doc.QuerySelectorAll(".product-item");
        foreach(var productNode in productNodes)
        {
            var productName = productNode.QuerySelector(".body")?.TextContent.Trim();
            var productUrl = productNode.QuerySelector(".thumbnail_container a")?.GetAttribute("href"); 
            var priceNode = productNode.QuerySelector(".price-current.pull-right");
            var priceText = priceNode?.TextContent;
            
            int priceValue = 0;
            if(priceText != null)
            {
                string cleanedPrice = priceText.Replace("руб.", "").Replace(" ", "").Trim();
                cleanedPrice = Regex.Replace(cleanedPrice, @"\s+", "");
                int.TryParse(cleanedPrice, out priceValue);
            }

            bool productInStock = true;
            var productInStockNode = productNode.QuerySelector("div.ribbon.green")?.TextContent;
            if(productInStockNode == null)
            {
                productInStock = false;
            }
            
            if(!string.IsNullOrEmpty(productName) && !string.IsNullOrEmpty(productUrl))
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
                    SiteName = "Amperkot",
                    Price = priceValue,
                    InStock = productInStock,
                    Count = null,
                    Product = product
                };
                productLinkList.Add(productLink);
            }
        }
        while(productLinkList.Count < totalResults)
        {
            html = await _httpClient.GetStringAsync(url + queryMessage + "&page=1");
            parser = new HtmlParser();
            doc = parser.ParseDocument(html);
            productNodes = doc.QuerySelectorAll(".product-item");

            foreach(var productNode in productNodes)
            {
                var productName = productNode.QuerySelector(".body")?.TextContent.Trim();
                var productUrl = productNode.QuerySelector(".thumbnail_container a")?.GetAttribute("href"); 
                var priceNode = productNode.QuerySelector(".price-current.pull-right");
                var priceText = priceNode?.TextContent;
                int priceValue = 0;
                
                if(priceText != null)
                {
                    string cleanedPrice = priceText.Replace("руб.", "").Replace(" ", "").Trim();
                    cleanedPrice = Regex.Replace(cleanedPrice, @"\s+", "");
                    int.TryParse(cleanedPrice, out priceValue);
                }

                bool productInStock = true;
                var productInStockNode = productNode.QuerySelector("div.ribbon.green")?.TextContent;
                if(productInStockNode == null)
                {
                    productInStock = false;
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
                    var productLink = new ProductLink
                    {
                        Id = Guid.NewGuid().ToString(),
                        Link = productUrl,
                        SiteName = "Amperkot",
                        Price = priceValue,
                        InStock = productInStock,
                        Count = null,
                        Product = product
                    };
                    productLinkList.Add(productLink);
                }
            }
        }

        sw.Stop();
        Console.WriteLine(sw.Elapsed);
        return productLinkList;
    }

    private int GetTotalResults(IDocument document)
    {
        var totalResultsElements = document?.QuerySelector("div.result-counter")?.TextContent
                                            .Trim()
                                            .Replace("Показано 1-21 из ", "");
        return totalResultsElements != null ? int.Parse(totalResultsElements) : 0;
    }

}