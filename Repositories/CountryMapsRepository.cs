using CountryMaps.TerminalsLoader.Data;
using CountryMaps.TerminalsLoader.Entities;
using CountryMaps.TerminalsLoader.Services;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace CountryMaps.TerminalsLoader.Repositories
{
    public class CountryMapsRepository(
        ApplicationDbContext dbContext,
        INotificationService notificationService,
        ILogger<CountryMapsRepository> logger) : ICountryMapsRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;
        private readonly ILogger<CountryMapsRepository> _logger = logger;
        private readonly INotificationService _notificationService = notificationService;
        public async Task Import(List<Office> offices, CancellationToken cancellationToken)
        {
            var trx = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            var deleted = 0;
            try
            {
                deleted = await _dbContext.Offices.ExecuteDeleteAsync(cancellationToken);
                _logger.LogInformation("Удалено {DeletedCount} существующих терминалов из базы данных", deleted);

                await _dbContext.BulkInsertAsync(offices, cancellationToken: cancellationToken);
                _logger.LogInformation("Добавлено {InsertedCount} терминалов в базу данных", offices.Count);

                await trx.CommitAsync(cancellationToken);

                _logger.LogInformation("Импорт терминалов завершен успешно.");

                await _notificationService.SendImportReportAsync(deleted, offices.Count, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при импорте терминалов, откат транзакции");
                await trx.RollbackAsync(cancellationToken);

                await _notificationService.SendErrorAsync(
                    "Ошибка импорта терминалов",
                    "Ошибка при импорте терминалов",
                    ex,
                    cancellationToken);
            }
        }
    }
}
