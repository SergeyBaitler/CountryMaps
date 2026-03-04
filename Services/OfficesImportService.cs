using System.Text.Json;
using CountryMaps.TerminalsLoader.Data;
using CountryMaps.TerminalsLoader.Entities;
using CountryMaps.TerminalsLoader.Repositories;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace CountryMaps.TerminalsLoader.Services;

public sealed class OfficesImportService(
    ILogger<OfficesImportService> logger,
    ICountryMapsRepository countryMapsRepository,
    IHostEnvironment env,
    INotificationService notificationService) : IOfficesImportService
{
    private readonly ILogger<OfficesImportService> _logger = logger;
    private readonly ICountryMapsRepository _countryMapsRepository = countryMapsRepository;
    private readonly IHostEnvironment _env = env;
    private readonly INotificationService _notificationService = notificationService;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task ImportAsync(CancellationToken cancellationToken)
    {
        var filePath = Path.Combine(_env.ContentRootPath, "files", "terminals.json");

        _logger.LogInformation("Начало импорта терминалов из {FilePath}", filePath);

        if (!File.Exists(filePath))
        {
            _logger.LogWarning("Файл терминалов не был найден {FilePath}", filePath);
            await _notificationService.SendErrorAsync(
                "Ошибка импорта терминалов",
                $"Файл терминалов не был найден {filePath}",
                null,
                cancellationToken);
            return;
        }

        string json;
        try
        {
            json = await File.ReadAllTextAsync(filePath, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка чтения файла терминалов {FilePath}", filePath);
            await _notificationService.SendErrorAsync(
                "Ошибка импорта терминалов",
                $"Ошибка чтения файла терминалов {filePath}",
                ex,
                cancellationToken);
            return;
        }

        List<Office>? offices;
        try
        {
            offices = JsonSerializer.Deserialize<List<Office>>(json, JsonOptions);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Ошибка при десериализации JSON файла терминалов");
            await _notificationService.SendErrorAsync(
                "Ошибка импорта терминалов",
                "Ошибка при десериализации JSON файла терминалов",
                ex,
                cancellationToken);
            return;
        }

        if (offices is null || offices.Count == 0)
        {
            _logger.LogWarning("Файл терминалов пуст");
            await _notificationService.SendErrorAsync(
                "Ошибка импорта терминалов",
                "Файл терминалов пуст",
                null,
                cancellationToken);
            return;
        }

        _logger.LogInformation("Десериализовано {Count} терминалов из JSON файла", offices.Count);
        await _countryMapsRepository.Import(offices, cancellationToken);
    }
}

