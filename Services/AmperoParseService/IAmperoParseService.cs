using ParsingService.Models;

public interface IAmperoParseService
{
    public Task<List<Product>> GetProdictListHtml(string url, string queryMessage);
}

