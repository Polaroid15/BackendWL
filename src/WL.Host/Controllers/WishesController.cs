using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using WL.Host.Dtos;
using WL.Host.Entities;
using WL.Host.Services;

namespace WL.Host.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WishesController : ControllerBase
{
    private readonly ILogger<WishesController> _logger;
    private readonly IWishRepository _wishRepository;
    private readonly IMapper _mapper;
    private readonly FileExtensionContentTypeProvider _contentTypeProvider;

    public WishesController(
        ILogger<WishesController> logger,
        IWishRepository wishRepository,
        IMapper mapper,
        FileExtensionContentTypeProvider contentTypeProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _wishRepository = wishRepository ?? throw new ArgumentNullException(nameof(wishRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _contentTypeProvider = contentTypeProvider ?? throw new ArgumentNullException(nameof(contentTypeProvider));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetWish(Guid id)
    {
        var wish = await _wishRepository.GetWishAsync(id);
        if (wish == null)
        {
            return NotFound();
        }
        _logger.LogInformation("wish name is: {name}", wish.Name);

        return Ok(wish);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WishDto>>> GetWishes()
    {
        var wishes = await _wishRepository.GetWishesAsync();
        _logger.LogInformation("wishes count is: {count}", wishes.Count());

        return Ok(_mapper.Map<IEnumerable<WishDto>>(wishes));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult> AddWish(Wish wish)
    {
        await _wishRepository.AddWishAsync(wish);
        return CreatedAtAction(nameof(GetWish), new { wish.Id}, wish);
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteWish(Guid id)
    {
        await _wishRepository.DeleteAsync(id);
        return NoContent();
    }

    [HttpPatch("[action]")]
    public async Task<ActionResult> PatchSomething(Guid id, JsonPatchDocument<Wish> patchWish)
    {
        var wish = await _wishRepository.GetWishAsync(id);
        if (wish == null)
        {
            return NotFound();
        }

        var newWish = new Wish()
        {
            Id = id,
            Name = wish.Name,
            Description = wish.Description,
        };
        
        patchWish.ApplyTo(newWish, ModelState);

        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        if (!TryValidateModel(newWish))
        {
            return BadRequest(ModelState);
        }

        wish.Name = newWish.Name;
        wish.Description = newWish.Description;
        
        return NoContent();
    }

    [HttpGet("[action]")]
    public IActionResult GetFile()
    {
        var filePath = "Profile.pdf";

        if (!System.IO.File.Exists(filePath))
        {
            return NotFound();
        }

        if (!_contentTypeProvider.TryGetContentType(filePath, out var contentType))
        {
            contentType = "application/octet-stream";
        }

        var bytes = System.IO.File.ReadAllBytes(filePath);
        return File(bytes, contentType, Path.GetFileName(filePath));
    }
}