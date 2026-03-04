using CountryMaps.TerminalsLoader.Services;

namespace CountryMaps.TerminalsLoader.Hosted;

public sealed class OfficesImportBackgroundService(
    ILogger<OfficesImportBackgroundService> logger,
    IOfficesImportService officesImportService,
    INotificationService notificationService,
    IConfiguration configuration) : BackgroundService
{
    private readonly ILogger<OfficesImportBackgroundService> _logger = logger;
    private readonly IOfficesImportService _officesImportService = officesImportService;
    private readonly INotificationService _notificationService = notificationService;
    private readonly TimeSpan _runTimeMsk = ParseRunTime(configuration);

    private static readonly TimeZoneInfo MoscowTimeZone =
        TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Старт импорт терминалов");

        while (!stoppingToken.IsCancellationRequested)
        {
            var delay = GetDelayUntilNextRun();
            _logger.LogInformation(
                "Следующий импорт терминалов запланирован через {Delay} (время запуска в {RunTimeMsk} MSK)",
                delay,
                _runTimeMsk);

            try
            {
                await Task.Delay(delay, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }

            try
            {
                await _officesImportService.ImportAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при импорте терминалов");

                try
                {
                    await _notificationService.SendErrorAsync(
                        "Ошибка импорта терминалов",
                        "Неожиданная ошибка возникла при импорте терминалов",
                        ex,
                        stoppingToken);
                }
                catch (Exception notificationEx)
                {
                    _logger.LogError(notificationEx, "Failed to send background service error notification");
                }
            }
        }

        _logger.LogInformation("Сервис импорта терминалов остановлен");
    }

    private TimeSpan GetDelayUntilNextRun()
    {
        var nowUtc = DateTimeOffset.UtcNow;
        var nowMsk = TimeZoneInfo.ConvertTime(nowUtc, MoscowTimeZone);

        var nextRunMsk = new DateTimeOffset(
            nowMsk.Year,
            nowMsk.Month,
            nowMsk.Day,
            _runTimeMsk.Hours,
            _runTimeMsk.Minutes,
            _runTimeMsk.Seconds,
            nowMsk.Offset);

        if (nowMsk >= nextRunMsk)
        {
            nextRunMsk = nextRunMsk.AddDays(1);
        }

        var nextRunUtc = TimeZoneInfo.ConvertTime(nextRunMsk, TimeZoneInfo.Utc);
        var delay = nextRunUtc - nowUtc;

        return delay < TimeSpan.Zero ? TimeSpan.Zero : delay;
    }

    private static TimeSpan ParseRunTime(IConfiguration configuration)
    {
        var defaultTime = new TimeSpan(2, 0, 0); // 02:00 MSK по умолчанию

        var timeString = configuration
            .GetSection("OfficesImport")
            .GetValue<string>("RunTimeMsk");

        if (string.IsNullOrWhiteSpace(timeString))
        {
            return defaultTime;
        }

        if (TimeSpan.TryParse(timeString, out var time))
        {
            return time;
        }

        return defaultTime;
    }
}

