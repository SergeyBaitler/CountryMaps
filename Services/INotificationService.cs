namespace CountryMaps.TerminalsLoader.Services
{
    public interface INotificationService
    {
        Task SendImportReportAsync(int deletedCount, int insertedCount, CancellationToken cancellationToken);
        Task SendErrorAsync(string subject, string message, Exception? exception, CancellationToken cancellationToken);
    }
}
