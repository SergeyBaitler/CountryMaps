using CountryMaps.TerminalsLoader.Entities;

namespace CountryMaps.TerminalsLoader.Repositories
{
    public interface ICountryMapsRepository
    {
        Task Import(List<Office> offices, CancellationToken cancellationToken);
    }
}
