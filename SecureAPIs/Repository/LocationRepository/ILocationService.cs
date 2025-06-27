namespace SecureAPIs.Repository.LocationRepository
{
    public interface ILocationService
    {
        Task<string> GetCountriesAsync();
        Task<string> GetStatesAsync(string country);
        Task<string> GetCitiesAsync(string country, string state);
    }
}
