#pragma warning disable CS8618
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace PingPal.Models;

public class SortingPaginationModelBase
{
    public string? SortBy { get; set; }

    public int? Page { get; set; }
    public int? TotalCount { get; set; }

    public int? TotalPages => TotalCount.HasValue
        ? (int) Math.Ceiling(TotalCount.Value / (double) Constants.PageSize)
        : null;

    public bool HasPrevPage => Page is > Constants.FirstPage;
    public bool HasNextPage => Page.HasValue && Page < TotalPages;
}