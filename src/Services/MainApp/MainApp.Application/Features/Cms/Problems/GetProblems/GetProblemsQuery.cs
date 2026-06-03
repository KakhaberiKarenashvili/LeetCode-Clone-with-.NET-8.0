using BuildingBlocks.Common.Enums;
using MainApp.Application.Common.Extensions.Pagination;
using MediatR;

namespace MainApp.Application.Features.Cms.Problems.GetProblems;

public record GetProblemsQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string? Name = null,
    Difficulty? Difficulty = null,
    List<Category>? Categories = null) : IRequest<PagedList<GetProblemsResponse>>;
