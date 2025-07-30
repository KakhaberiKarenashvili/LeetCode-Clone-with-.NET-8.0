using BuildingBlocks.Common.Dtos;
using BuildingBlocks.Common.Enums;
using FakeItEasy;
using FluentAssertions;
using MainApp.Application.Dto.Request;
using MainApp.Application.Services;
using MainApp.Domain.Entity;
using MainApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MainApp.Test.Application.Service.Tests;

public class AdminServiceTests
{
    private readonly AdminService _adminService;
    private readonly AppDbContext _appDbContext;

    public AdminServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _appDbContext = new AppDbContext(options);
        _adminService = new AdminService(_appDbContext);
    }
    

    public void Dispose()
    {
        _appDbContext.Dispose();
    }
}
