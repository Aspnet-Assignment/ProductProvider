using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductProvider.Contexts;

namespace ProductProvider.Functions.CategoryHandler;

public class GetAllCategories
{
    private readonly ILogger<GetAllCategories> _logger;
    private readonly DataContext _context;

    public GetAllCategories(ILogger<GetAllCategories> logger, DataContext context)
    {
        _logger = logger;
        _context = context;
    }

    [Function("GetAllCategories")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "categories/all")] HttpRequest req)
    {
        try
        {
            var items = await _context.Categories.ToListAsync();
            return new OkObjectResult(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new NotFoundResult();
        }
    }
}
