using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using WL.Host.Dtos;
using WL.Host.Entities;
using WL.Host.Services;

namespace WL.Host.Controllers;

[ApiController]
// [Authorize(Policy = "MustHaveUsernamePolaroid15")]
[Route("api/[controller]")]
public class WishesController : ControllerBase
{
    private readonly ILogger<WishesController> _logger;
    private readonly IWishRepository _wishRepository;
    private readonly IMapper _mapper;
    private readonly IBlackListService _blackListService;
    private readonly FileExtensionContentTypeProvider _contentTypeProvider;
    private const int MAX_PAGE_SIZE = 3;

    public WishesController(
        ILogger<WishesController> logger,
        IWishRepository wishRepository,
        IMapper mapper,
        IBlackListService blackListService,
        FileExtensionContentTypeProvider contentTypeProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _wishRepository = wishRepository ?? throw new ArgumentNullException(nameof(wishRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _blackListService = blackListService ?? throw new ArgumentNullException(nameof(blackListService));
        _contentTypeProvider = contentTypeProvider ?? throw new ArgumentNullException(nameof(contentTypeProvider));
    }

    /// <summary>
    /// Create new wish.
    /// </summary>
    /// <param name="wish">New wish <see cref="Wish"/>.</param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(WishDto wish)
    {
        var wishEntity = _mapper.Map<Wish>(wish);
        await _wishRepository.AddWishAsync(wishEntity);
        await _wishRepository.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { wishEntity.Id }, wish);
    }

    /// <summary>
    /// Get wish.
    /// </summary>
    /// <param name="id">id of with.</param>
    /// <returns>Wish.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult> Get(Guid id)
    {
        var user = User.Claims.FirstOrDefault(u => u.Type == "id")?.Value;
        
        if (string.IsNullOrEmpty(user))
        {
            return Forbid();
        }
        var wish = await _wishRepository.GetWishAsync(id);
        if (wish == null)
        {
            return NotFound();
        }

        var blackListItems = await _blackListService.GetBlackListItems(id);

        _logger.LogInformation("wish name is: {name}. BlackList count of wish: {count}", wish.Name, blackListItems.Count);

        return Ok(wish);
    }

    /// <summary>
    /// Get pagination wishes
    /// </summary>
    /// <param name="name">Filter by name.</param>
    /// <param name="searchQuery">Parameter to search wishes.</param>
    /// <param name="pageNumber">Page number for pagination.</param>
    /// <param name="pageSize">Page size of pagination.</param>
    /// <returns>An IActionResult</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

    /// <summary>
    /// Delete wish.
    /// </summary>
    /// <param name="id">id of wish.</param>
    [HttpDelete]
    public async Task<IActionResult> Delete(Guid id)
    {
        _wishRepository.Delete(id);
        await _wishRepository.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// Update wish.
    /// </summary>
    /// <param name="id">id of wish.</param>
    /// <param name="updateWish">New wish.</param>
    /// <returns>Wish.</returns>
    [HttpPut]
    public async Task<IActionResult> Update(Guid id, Wish updateWish)
    {
        var wish = await _wishRepository.GetWishAsync(id);
        _mapper.Map(updateWish, wish);
        await _wishRepository.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// Update part of old wish.
    /// </summary>
    /// <param name="id">id of wish.</param>
    /// <param name="patchWish">Instructions for update.</param>
    /// <returns></returns>
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