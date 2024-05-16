using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using ProductProvider.Contexts;
using ProductProvider.Entities;
using System.Text.Json;

namespace ProductProvider.Functions.ColorHandler;

public class UpdateColorById
{
    private readonly ILogger<UpdateColorById> _logger;
    private readonly DataContext _context;

    public UpdateColorById(ILogger<UpdateColorById> logger, DataContext context)
    {
        _logger = logger;
        _context = context;
    }

    [Function("UpdateColorById")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route = "colors/{id}")] HttpRequest req, string id)
    {
        try
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var updatedItem = JsonSerializer.Deserialize<Color>(requestBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (updatedItem == null || updatedItem.Id != id)
            {
                return new BadRequestResult();
            }

            var existingItem = await _context.Colors.FindAsync(id);
            if (existingItem == null)
            {
                return new NotFoundResult();
            }

            existingItem.ColorName = updatedItem.ColorName;



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