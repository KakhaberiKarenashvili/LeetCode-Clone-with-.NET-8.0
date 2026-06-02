using BuildingBlocks.Common.Enums;
using MainApp.Application.Extensions.Pagination;
using MediatR;

namespace MainApp.Application.Features.Problems.Queries.GetProblems;

public record GetProblemsQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string? Name = null,
    Difficulty? Difficulty = null,
    List<Category>? Categories = null) : IRequest<PagedList<GetProblemsResponse>>;
