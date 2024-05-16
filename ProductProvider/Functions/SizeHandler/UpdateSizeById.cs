using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using ProductProvider.Contexts;
using ProductProvider.Entities;
using System.Text.Json;

namespace ProductProvider.Functions.SizeHandler;

public class UpdateSizeById
{
    private readonly ILogger<UpdateSizeById> _logger;
    private readonly DataContext _context;

    public UpdateSizeById(ILogger<UpdateSizeById> logger, DataContext context)
    {
        _logger = logger;
        _context = context;
    }

    [Function("UpdateSizeById")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route = "sizes/{id}")] HttpRequest req, string id)
    {
        try
        {
            var existingItem = await _context.Sizes.FindAsync(id);
            if (existingItem == null)
            {
                return new NotFoundResult();
            }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var updatedItem = JsonSerializer.Deserialize<Size>(requestBody, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            if (updatedItem == null)
            {
                return new BadRequestResult();
            }

            existingItem.SizeName = updatedItem.SizeName;

            _context.Sizes.Update(existingItem);
            await _context.SaveChangesAsync();

            return new OkObjectResult(existingItem);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}
