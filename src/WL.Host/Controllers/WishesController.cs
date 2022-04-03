using System.Text.Json;
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
    private const int MAX_PAGE_SIZE = 3;

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

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> AddWish(Wish wish)
    {
        await _wishRepository.AddWishAsync(wish);
        await _wishRepository.SaveChangesAsync();
        return CreatedAtAction(nameof(GetWish), new { wish.Id }, wish);
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
    public async Task<ActionResult<IEnumerable<WishDto>>> GetWishes(
        [FromQuery] string? name,
        [FromQuery] string? searchQuery,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 2)
    {
        if (pageSize > MAX_PAGE_SIZE)
        {
            pageSize = MAX_PAGE_SIZE;
        }

        var (wishes, metadata) = await _wishRepository.GetWishesAsync(name, searchQuery, pageNumber, pageSize);
        _logger.LogInformation("wishes count is: {count}", wishes.Count());

        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));
        return Ok(_mapper.Map<IEnumerable<WishDto>>(wishes));
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteWish(Guid id)
    {
        _wishRepository.Delete(id);
        await _wishRepository.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateWish(Guid id, UpdateWishDto updateWish)
    {
        var wish = await _wishRepository.GetWishAsync(id);
        _mapper.Map(updateWish, wish);
        await _wishRepository.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult> PatchSomething(Guid id, JsonPatchDocument<UpdateWishDto> patchWish)
    {
        var wish = await _wishRepository.GetWishAsync(id);
        if (wish == null)
        {
            return NotFound();
        }

        var newWish = _mapper.Map<UpdateWishDto>(wish);

        patchWish.ApplyTo(newWish, ModelState);

        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        if (!TryValidateModel(newWish))
        {
            return BadRequest(ModelState);
        }

        _mapper.Map(newWish, wish);
        await _wishRepository.SaveChangesAsync();

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