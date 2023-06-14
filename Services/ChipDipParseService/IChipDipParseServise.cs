namespace ParsingService.ChipDipParseService;
public interface IChipDipParseService
{
    public Task<List<string>> GetProdictListHtml(string queryMessage ,string url);
}