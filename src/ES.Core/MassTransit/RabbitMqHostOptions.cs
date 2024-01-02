namespace ES.Core.MassTransit;

public class RabbitMqHostOptions
{
    /// <summary>
    /// RabbitMQ host uri `rabbitmq://localhost`
    /// </summary>
    public required Uri Host { get; set; }

    /// <summary>
    /// Username credential
    /// </summary>
    public required string Username { get; set; }

    /// <summary>
    /// Password credential
    /// </summary>
    public required string Password { get; set; }

    /// <summary>
    /// Enable batch publishing of messages
    /// </summary>
    public bool BatchPublish { get; set; } = true;

    /// <summary>
    /// Timeout to wait for messages to send in a batch
    /// </summary>
    public TimeSpan BatchPublishTimeout { get; set; } = TimeSpan.Zero;
}
