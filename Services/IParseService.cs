using ParsingService.Models;

public interface IParseService
{
    public Task<List<ProductLink>> GetProdictLinkListAsync(string url, string queryMessage);
    public Task<ProductLink> GetOneProductLinkAsync(string baseUrl, string productUrl);
}

