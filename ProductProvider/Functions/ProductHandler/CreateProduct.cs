using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ProductProvider.Contexts;
using ProductProvider.Entities;

namespace ProductProvider.Functions.ProductHandler;

public class CreateProduct
{
    private readonly ILogger<CreateProduct> _logger;
    private readonly DataContext _context;

    public CreateProduct(ILogger<CreateProduct> logger, DataContext context)
    {
        _logger = logger;
        _context = context;
    }

    [Function("CreateProduct")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        try
        {
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var entity = JsonConvert.DeserializeObject<Product>(body);

            _context.Add(entity);
            await _context.SaveChangesAsync();

            return new OkResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new BadRequestResult();
        }
    }
}
