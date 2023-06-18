using ParsingService.Models;

public interface IAmperoParseService
{
    public Task<List<ProductLink>> GetProdictListHtml(string url, string queryMessage);
}

