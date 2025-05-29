using Microsoft.AspNetCore.Mvc;

namespace Logging.API.Controllers;

[ApiController]
[Route("api/[controller]")]
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

    private readonly ILogger<CountriesController> _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger instance cannot be null.");

    [HttpPost]
    [ProducesResponseType(typeof(Country), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public ActionResult<Country> CreateCountry([FromBody] Country country)
    {
        if (country is null)
            throw new ArgumentNullException(nameof(country), "Country data is required.");

        if (string.IsNullOrWhiteSpace(country.Alpha2Code) || country.Alpha2Code.Length != 2)
            throw new ArgumentException("Alpha2Code must be exactly 2 characters.", nameof(country));

        if (string.IsNullOrWhiteSpace(country.Name))
            throw new ArgumentException("Country name is required.", nameof(country));

        if (string.IsNullOrWhiteSpace(country.Region))
            throw new ArgumentException("Region is required.", nameof(country));

        if (_countries.Exists(c => c.Alpha2Code.Equals(country.Alpha2Code, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"A country with code '{country.Alpha2Code}' already exists.");
        }

        // Simulate adding it to the list (not persisted)
        _countries.Add(country);
        _logger.LogInformation("Country created successfully: {@Country}", country);

        return CreatedAtAction(nameof(GetCountryByCode), new { code = country.Alpha2Code }, country);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Country>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<Country>> GetAllCountries()
    {
        _logger.LogInformation("Getting all countries");
        return Ok(_countries);
    }

    [HttpGet("{code}")]
    [ProducesResponseType(typeof(Country), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Country> GetCountryByCode(string code)
    {
        _logger.LogInformation("Searching for country with code: {Code}", code);

        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentNullException(nameof(code), "Country code cannot be null or empty.");
        }

        if (code.Length != 2)
        {
            throw new ArgumentException("Country code must be exactly 2 characters long.", nameof(code));
        }

        var country = _countries.Find(c => c.Alpha2Code.Equals(code, StringComparison.OrdinalIgnoreCase));

        if (country is null)
        {
            return NotFound($"No country found with code '{code}'.");
        }

        return Ok(country);
    }

    [HttpGet("region/{region}")]
    [ProducesResponseType(typeof(IEnumerable<Country>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<IEnumerable<Country>> GetCountriesByRegion(string region)
    {
        _logger.LogInformation("Filtering countries by region: {Region}", region);

        if (region is null)
            throw new ArgumentNullException(nameof(region), "Region cannot be null.");

        var filtered = _countries
            .Where(c => c.Region.Equals(region, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (filtered.Count == 0)
        {
            _logger.LogWarning("No countries found for region: {Region}", region);
            return NotFound($"No countries found in region '{region}'.");
        }

        return Ok(filtered);
    }
}