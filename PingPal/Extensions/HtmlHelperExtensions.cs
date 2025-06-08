using System.Linq.Expressions;
using System.Text;
using PingPal.Common.Extensions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using PingPal.Models;

namespace PingPal.Extensions;

public static class HtmlHelperExtensions
{
    public static HtmlString DisplaySortColumnTitle<TModel, TResult>(
        this IHtmlHelper<TModel> htmlHelper,
        HttpContext context,
        string? currentSortBy,
        string sortPropertyName,
        Expression<Func<TModel, TResult>> expression,
        bool isDefault = false)
    {
        (currentSortBy, var sortDescPropertyName, var nextSortPropertyName) =
            CreateSortPropertyNames(currentSortBy, sortPropertyName, isDefault);

        var nextUrl = CreateUrlWithChangedSort(context, nextSortPropertyName);

        var stringBuilder = new StringBuilder()
            .Append("<a href=\"")
            .Append(nextUrl)
            .Append("\" class=\"link-dark\">")
            .Append(htmlHelper.DisplayNameFor(expression))
            .Append("</a>");

        if (currentSortBy == sortPropertyName)
            stringBuilder.Append(" <i class=\"fa fa-sort-asc\" style=\"vertical-align: middle; margin-bottom: -5px;\"></i>");
        else if (currentSortBy == sortDescPropertyName)
            stringBuilder.Append(" <i class=\"fa fa-sort-desc\" style=\"vertical-align: middle; margin-top: -5px;\"></i>");

        return new HtmlString(stringBuilder.ToString());
    }

    private static (string? CurrentSortBy, string SortDescPropertyName, string NextSortPropertyName)
        CreateSortPropertyNames(string? currentSortBy, string sortPropertyName, bool isDefault)
    {
        var sortDescPropertyName = $"{sortPropertyName}{Constants.DescSuffix}";

        if (currentSortBy.IsNullOrEmpty() && isDefault)
            currentSortBy = sortPropertyName;

        var nextSortPropertyName =
            currentSortBy == sortPropertyName
                ? sortDescPropertyName
                : isDefault
                    ? string.Empty
                    : sortPropertyName;

        return (currentSortBy, sortDescPropertyName, nextSortPropertyName);
    }

    private static string CreateUrlWithChangedSort(HttpContext context, string nextSortPropertyName)
    {
        var path = context.Request.Path;
        var parameters = context.Request.Query.ToList();

        parameters.RemoveAll(pair => pair.Key == "Page");
        parameters.RemoveAll(pair => pair.Key == "SortBy");

        if (nextSortPropertyName.IsSignificant())
            parameters.Add(KeyValuePair.Create("SortBy", new StringValues(nextSortPropertyName)));

        return QueryHelpers.AddQueryString(path, parameters);
    }

    public static HtmlString DisplayPagination<TModel>(
        this IHtmlHelper<TModel> htmlHelper,
        HttpContext context,
        SortingPaginationModelBase model)
    {
        var stringBuilder = new StringBuilder();
        var pageValue = model.Page ?? Constants.FirstPage;
        var totalCountValue = model.TotalCount ?? 0;
        var totalPagesValue = model.TotalPages ?? Constants.FirstPage;

        stringBuilder
            .Append("<div style=\"margin-top: 8px;\">")
            .Append("Количество: ")
            .Append(totalCountValue)
            .Append("</div>")
            .AppendLine();
        stringBuilder
            .Append("<div style=\"margin-top: 4px;\">")
            .AppendLine();
        stringBuilder
            .Append("<a href=\"")
            .Append(CreateUrlWithChangedPage(context, pageValue - 1))
            .Append("\" class=\"btn btn-sm btn-outline-primary")
            .Append(model.HasPrevPage ? string.Empty : " disabled")
            .Append("\"><i class=\"fa fa-chevron-left\"></i> Назад</a>")
            .AppendLine();
        stringBuilder
            .Append("Страница ")
            .Append(pageValue)
            .Append(" из ")
            .Append(totalPagesValue)
            .AppendLine();
        stringBuilder
            .Append("<a href=\"")
            .Append(CreateUrlWithChangedPage(context, pageValue + 1))
            .Append("\" class=\"btn btn-sm btn-outline-primary")
            .Append(model.HasNextPage ? string.Empty : " disabled")
            .Append("\">Вперед <i class=\"fa fa-chevron-right\"></i></a>")
            .AppendLine();
        stringBuilder
            .Append("</div>")
            .AppendLine();

        return new HtmlString(stringBuilder.ToString());
    }

    private static string CreateUrlWithChangedPage(HttpContext context, int pageValue)
    {
        var path = context.Request.Path;
        var parameters = context.Request.Query.ToList();

        parameters.RemoveAll(pair => pair.Key == "Page");

        if (pageValue > Constants.FirstPage)
            parameters.Add(KeyValuePair.Create("Page", new StringValues(pageValue.ToString())));

        return QueryHelpers.AddQueryString(path, parameters);
    }
}