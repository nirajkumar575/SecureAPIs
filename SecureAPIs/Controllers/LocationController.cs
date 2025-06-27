using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SecureAPIs.Models;
using SecureAPIs.Repository.LocationRepository;

namespace SecureAPIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    [EnableRateLimiting("fixed")]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet("countries")]
        public async Task<IActionResult> GetCountries()
        {
            var result = await _locationService.GetCountriesAsync();
            return Content(result, "application/json");
        }

        [HttpPost("states")]
        public async Task<IActionResult> GetStates([FromBody] StateRequest request)
        {
            var result = await _locationService.GetStatesAsync(request.Country);
            return Content(result, "application/json");
        }

        [HttpPost("cities")]
        public async Task<IActionResult> GetCities([FromBody] CityRequest request)
        {
            var result = await _locationService.GetCitiesAsync(request.Country, request.State);
            return Content(result, "application/json");
        }
    }

}
