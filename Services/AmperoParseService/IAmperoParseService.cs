public interface IAmperoParseService
{
    public Task<List<string>> GetProdictListHtml(string url, string queryMessage);
}