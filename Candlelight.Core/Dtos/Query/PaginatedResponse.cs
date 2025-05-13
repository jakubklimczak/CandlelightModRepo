namespace Candlelight.Core.Dtos.Query;

public class PaginatedResponse<T>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    // ReSharper disable once UseCollectionExpression - reason: generic
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
}
