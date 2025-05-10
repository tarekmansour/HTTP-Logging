using Microsoft.AspNetCore.Mvc;

namespace Logging.API.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class CountriesController(ILogger<CountriesController> logger) : ControllerBase
{
    private static readonly List<Country> _countries = new()
    {
        new Country("FR", "France", "Europe"),
        new Country("US", "United States", "North America"),
        new Country("JP", "Japan", "Asia"),
        new Country("BR", "Brazil", "South America")
    };

    private readonly ILogger<CountriesController> _logger = logger;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Country>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<Country>> GetAllCountries()
    {
        _logger.LogInformation("Getting all countries");
        return Ok(_countries);
    }

    [HttpGet("{code}")]
    [ProducesResponseType(typeof(Country), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Country> GetCountryByCode(string code)
    {
        _logger.LogInformation("Looking up country with code: {CountryCode}", code);

        if (string.IsNullOrWhiteSpace(code) || code.Length != 2)
        {
            _logger.LogWarning("Invalid country code provided: {CountryCode}", code);
            return BadRequest("Country code must be 2 letters.");
        }

        var country = _countries.FirstOrDefault(c => c.Alpha2Code.Equals(code, StringComparison.OrdinalIgnoreCase));
        if (country is null)
        {
            _logger.LogWarning("Country not found for code: {CountryCode}", code);
            return NotFound($"No country found with code '{code}'.");
        }

        _logger.LogInformation("Found country: {@Country}", country);
        return Ok(country);
    }

    [HttpGet("region/{region}")]
    [ProducesResponseType(typeof(IEnumerable<Country>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<IEnumerable<Country>> GetCountriesByRegion(string region)
    {
        _logger.LogInformation("Filtering countries by region: {Region}", region);

        var filtered = _countries
            .Where(c => c.Region.Equals(region, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (!filtered.Any())
        {
            _logger.LogWarning("No countries found for region: {Region}", region);
            return NotFound($"No countries found in region '{region}'.");
        }

        _logger.LogInformation("Found {Count} countries in region {Region}", filtered.Count, region);
        return Ok(filtered);
    }
}
