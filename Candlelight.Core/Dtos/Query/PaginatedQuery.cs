namespace Candlelight.Core.Dtos.Query;

public class PaginatedQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
