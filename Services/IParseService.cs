using ParsingService.Models;

public interface IParseService
{
    public Task<List<ProductLink>> GetProdictLinkList(string url, string queryMessage);
}

