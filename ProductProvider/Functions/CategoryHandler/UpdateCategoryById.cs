using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using ProductProvider.Contexts;
using ProductProvider.Entities;
using System.Text.Json;

namespace ProductProvider.Functions.CategoryHandler;

public class UpdateCategoryById
{
    private readonly ILogger<UpdateCategoryById> _logger;
    private readonly DataContext _context;

    public UpdateCategoryById(ILogger<UpdateCategoryById> logger, DataContext context)
    {
        _logger = logger;
        _context = context;
    }

    [Function("UpdateCategoryById")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route = "categories/{id}")] HttpRequest req, string id)
    {
        var existingItem = await _context.Categories.FindAsync(id);
        if (existingItem == null)
        {
            return new NotFoundResult();
        }

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var updatedItem = JsonSerializer.Deserialize<Category>(requestBody, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        if (updatedItem == null)
        {
            return new BadRequestResult();
        }
        existingItem.CategoryName = updatedItem.CategoryName;

        _context.Categories.Update(existingItem);
        await _context.SaveChangesAsync();

        return new OkObjectResult(existingItem);
    }
}
