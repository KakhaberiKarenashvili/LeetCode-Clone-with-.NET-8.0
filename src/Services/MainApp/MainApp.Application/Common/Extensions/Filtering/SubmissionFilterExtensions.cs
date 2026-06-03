using BuildingBlocks.Common.Enums;
using MainApp.Domain.Entity;

namespace MainApp.Application.Common.Extensions.Filtering;

public static class SubmissionFilterExtensions
{
    public static IQueryable<Submissions> FilterByStatus(this IQueryable<Submissions> query, Status? status)
    {
        if (status == null)
            return query;

        return query.Where(s => s.Status == status);
    }

    public static IQueryable<Submissions> FilterByLanguage(this IQueryable<Submissions> query, Language? language)
    {
        if (language == null)
            return query;

        return query.Where(s => s.Language == language);
    }

    public static IQueryable<Submissions> ApplyFilter(this IQueryable<Submissions> query, Status? status, Language? language)
    {
        return query
            .FilterByLanguage(language)
            .FilterByStatus(status);
    }
}
