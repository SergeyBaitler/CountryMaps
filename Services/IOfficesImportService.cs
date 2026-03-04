namespace CountryMaps.TerminalsLoader.Services
{
    public interface IOfficesImportService
    {
        Task ImportAsync(CancellationToken cancellationToken);
    }
}
