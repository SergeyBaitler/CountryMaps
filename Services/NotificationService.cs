using System.Net;
using System.Net.Mail;

namespace CountryMaps.TerminalsLoader.Services;


public sealed class NotificationService(IConfiguration configuration, ILogger<NotificationService> logger)
    : INotificationService
{
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<NotificationService> _logger = logger;

    public async Task SendImportReportAsync(int deletedCount, int insertedCount, CancellationToken cancellationToken)
    {
        var subject = "Импорт терминалов завершон";
        var body =
            $"Импорт терминалов завершон успешно.{Environment.NewLine}Удалено: {deletedCount}{Environment.NewLine}Импортировано: {insertedCount}";

        await SendEmailAsync(subject, body, cancellationToken);
    }

    public async Task SendErrorAsync(string subject, string message, Exception? exception, CancellationToken cancellationToken)
    {
        var body = message;
        if (exception is not null)
        {
            body += Environment.NewLine + Environment.NewLine + exception;
        }

        await SendEmailAsync(subject, body, cancellationToken);
    }

    private async Task SendEmailAsync(string subject, string body, CancellationToken cancellationToken)
    {
        //TODO: Удалить эту строку после конфигурации smtp
        _logger.LogInformation($"Отправка письма с темой {subject} и сообщением {body}");
        return;

        var section = _configuration.GetSection("Notification:Smtp");
        var host = section.GetValue<string>("Host");
        var port = section.GetValue<int?>("Port") ?? 587;
        var enableSsl = section.GetValue<bool?>("EnableSsl") ?? true;
        var from = section.GetValue<string>("From");
        var to = section.GetValue<string>("To");
        var user = section.GetValue<string>("User");
        var password = section.GetValue<string>("Password");

        if (string.IsNullOrWhiteSpace(host) ||
            string.IsNullOrWhiteSpace(from) ||
            string.IsNullOrWhiteSpace(to))
        {
            _logger.LogWarning("Уведомление: Настройки SMTP не полностью сконфигурированы. Электронное письмо не будет отправлено.");
            return;
        }

        using var message = new MailMessage(from, to)
        {
            Subject = subject,
            Body = body
        };

        using var client = new SmtpClient(host, port)
        {
            EnableSsl = enableSsl
        };

        if (!string.IsNullOrWhiteSpace(user))
        {
            client.Credentials = new NetworkCredential(user, password);
        }

        try
        {
            using var registration = cancellationToken.Register(client.SendAsyncCancel);
            await client.SendMailAsync(message, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при отправке письма с темой {Subject}", subject);
        }
    }
}

