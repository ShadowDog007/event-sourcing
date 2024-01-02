using FluentValidation;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ES.Core.MassTransit;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEventSourcingMassTransit(this IServiceCollection services)
    {
        services.AddSingleton<IConsumeContextProvider, ConsumeContextProvider>();

        services.AddMassTransit(mt =>
        {
            mt.SetEndpointNameFormatter(new EventSourcingEndpointNameFormatter());

            mt.UsingRabbitMq((ctx, mq) =>
            {
                // TODO - Add exceptions to this
                mq.UseMessageRetry(r => r.Immediate(1)
                    .Ignore<ValidationException>());
                //mq.UseCircuitBreaker(r =>
                //{
                //    ctx.GetRequiredService<IConfiguration>().Bind(r);
                //    r.Ignore<ValidationException>();
                //});
                mq.UseServiceScope(ctx);
                mq.UseInMemoryOutbox(ctx, o => o.ConcurrentMessageDelivery = true);
                mq.UseConsumeFilter(typeof(TransactionScopeFilter<>), ctx);
                mq.UseConsumeFilter(typeof(ConsumeContextScopeFilter<>), ctx);

                var optionsMonitor = ctx.GetRequiredService<IOptionsMonitor<RabbitMqHostOptions>>();
                mq.Host(optionsMonitor.CurrentValue.Host, opt =>
                {
                    var config = optionsMonitor.CurrentValue;
                    opt.Username(config.Username);
                    opt.Password(config.Password);
                    
                    if (config.BatchPublish)
                    {
                        opt.ConfigureBatchPublish(b =>
                        {
                            b.Timeout = config.BatchPublishTimeout;
                        });
                    }
                });

                mq.ConfigureEndpoints(ctx);
            });
        });

        services.AddOptions<RabbitMqHostOptions>()
            .BindConfiguration($"MassTransit:{nameof(RabbitMqHostOptions)}")
            .Validate(o => o.Host is { Scheme: "rabbitmq" }, $"MassTransit:{nameof(RabbitMqHostOptions)}:{nameof(RabbitMqHostOptions.Host)} must use `rabbitmq://` scheme")
            .Validate(o => !string.IsNullOrEmpty(o.Host.Host), $"MassTransit:{nameof(RabbitMqHostOptions)}:{nameof(RabbitMqHostOptions.Host)} must have a non-empty hostname")
            .Validate(o => !string.IsNullOrEmpty(o.Username), $"MassTransit:{nameof(RabbitMqHostOptions)}:{nameof(RabbitMqHostOptions.Username)} must have a non-empty value")
            .Validate(o => !string.IsNullOrEmpty(o.Password), $"MassTransit:{nameof(RabbitMqHostOptions)}:{nameof(RabbitMqHostOptions.Password)} must have a non-empty value")
            .ValidateOnStart();

        return services;
    }
}
