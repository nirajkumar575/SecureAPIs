using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SecureAPIs.Repository.LocationRepository
{
    public class LocationService : ILocationService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _countriesEndpoint;
        private readonly string _statesEndpoint;
        private readonly string _citiesEndpoint;
        public LocationService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["CountryApi:BaseUrl"] ?? "";
            _countriesEndpoint = configuration["CountryApi:CountriesEndpoint"] ?? "positions";
            _statesEndpoint = configuration["CountryApi:StatesEndpoint"] ?? "states";
            _citiesEndpoint = configuration["CountryApi:CitiesEndpoint"] ?? "state/cities";
        }

        public async Task<string> GetCountriesAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}{_countriesEndpoint}");
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetStatesAsync(string country)
        {
            var body = JsonSerializer.Serialize(new { country });
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}{_statesEndpoint}", content);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetCitiesAsync(string country, string state)
        {
            var body = JsonSerializer.Serialize(new { country, state });
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}{_citiesEndpoint}", content);
            return await response.Content.ReadAsStringAsync();
        }
    }

}
