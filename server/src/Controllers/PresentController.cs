using a.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreApi.DTOs;
using StoreApi.Interfaces;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
namespace a.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class PresentController : ControllerBase
    {
        private readonly IPresentService _presentService;
        private readonly ILogger<PresentController> _logger;
        private readonly IDistributedCache _cache;

        public PresentController(
            IPresentService presentService,
            ILogger<PresentController> logger,IDistributedCache cache)
        {
            _presentService = presentService;
            _logger = logger;
            _cache = cache;
        }
        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<PresentDto>>> GetAllPaged([FromQuery] PaginationParams paginationParams)
        {
            string cacheKey = "all-products";
            var cachedData  = await _cache.GetStringAsync(cacheKey);
            if(!string.IsNullOrEmpty(cachedData))
            {
                _logger.LogInformation("Data retrieved from cache.");
                var productsFromCache = JsonSerializer.Deserialize<PagedResult<PresentDto>>(cachedData);
                return Ok(productsFromCache);
            }

            _logger.LogInformation("Data retrieved from database.");

            var presents = await _presentService.GetAllPresentsPagedAsync(paginationParams);


            var cacheOptions = new DistributedCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromSeconds(300));
            var serializedData = JsonSerializer.Serialize(presents);
        await _cache.SetStringAsync(cacheKey, serializedData, cacheOptions);

            return Ok(presents);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PresentDto>> getById(int id)
        {
            var present = await _presentService.GetPresentByIdAsync(id);
            if (present == null)
                return NotFound(new { massage = $"present with UserId: {id} not found" });

            return Ok(present);
        }
        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<PresentDto>>> GetByCategory(int categoryId)
        {
            var presents = await _presentService.GetPresentsByCategoryAsync(categoryId);
            return Ok(presents);
        }
        //create present dto
        //[Authorize(Roles = "Donor")]
        [HttpPost]
        public async Task<ActionResult<PresentDto>> Create([FromBody] CreatePresentDto presentDto)
        {
            try
            {
                var present = await _presentService.CreatePresentAsync(presentDto);
                return CreatedAtAction(nameof(getById), new { id = present.Id }, present);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { massage = ex.Message });
            }
        }
        //[Authorize(Roles = "Admin,Donor")]

        [HttpPut("{id}")]
        public async Task<ActionResult<PresentDto>> Update(int id, [FromBody] UpdatePresentDto presentDto)
        {

            try
            {
                var present = await _presentService.UpdatePresentAsync(id, presentDto);
                if (present == null)
                {
                    return NotFound(new { message = $"Product with ID {id} not found." });
                }

                return Ok(present);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

      //  [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<PresentDto>> Delete(int id)
        {

            var result = await _presentService.DeletePresentAsync(id);

            if (!result)
            {
                return NotFound(new { message = $"Present with ID {id} not found." });
            }

            return NoContent();
        }

    }
}