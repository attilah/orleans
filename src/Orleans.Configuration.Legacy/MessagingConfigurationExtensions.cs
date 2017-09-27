using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Orleans.Runtime.Configuration;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Orleans.Configuration.Legacy
{
    public static class MessagingConfigurationExtensions
    {
        public static IServiceCollection AddLegacySiloConfigurationSupport(this IServiceCollection services)
        {
            // Global configuration

            services.Configure<GlobalConfiguration, SiloMessagingOptions>((config, options) =>
            {
                CopyCommonMessagingOptions(config, options);

                options.SiloSenderQueues = config.SiloSenderQueues;
                options.GatewaySenderQueues = config.GatewaySenderQueues;
                options.MaxForwardCount = config.MaxForwardCount;
                options.ClientDropTimeout = config.ClientDropTimeout;
            });

            services.Configure<GlobalConfiguration, SerializationProviderOptions>((config, options) =>
            {
                options.SerializationProviders = config.SerializationProviders;
                options.FallbackSerializationProvider = config.FallbackSerializationProvider;
            });
            
            // Node configuration

            // Messaging configuration

            return services;
        }

        public static IServiceCollection AddLegacyClientConfigurationSupport(this IServiceCollection services)
        {
            // Global configuration

            services.Configure<GlobalConfiguration, ClientMessagingOptions>((config, options) =>
            {
                CopyCommonMessagingOptions(config, options);

                options.ClientSenderBuckets = config.ClientSenderBuckets;
            });

            // Node configuration

            // Messaging configuration

            return services;
        }

        private static void CopyCommonMessagingOptions(GlobalConfiguration config, MessagingOptions options)
        {
            options.OpenConnectionTimeout = config.OpenConnectionTimeout;
            options.ResponseTimeout = config.ResponseTimeout;
            options.MaxResendCount = config.MaxResendCount;
            options.ResendOnTimeout = config.ResendOnTimeout;
            options.MaxSocketAge = config.MaxSocketAge;
            options.DropExpiredMessages = config.DropExpiredMessages;
            options.BufferPoolBufferSize = config.BufferPoolBufferSize;
            options.BufferPoolMaxSize = config.BufferPoolMaxSize;
            options.BufferPoolPreallocationSize = config.BufferPoolPreallocationSize;
        }

        private static void Configure<TConfiguration, TOptions>(this IServiceCollection services, Action<TConfiguration, TOptions> configureOptions) where TOptions : class
        {
            services.AddSingleton<IConfigureOptions<TOptions>>(sp =>
            {
                var configuration = sp.GetRequiredService<TConfiguration>();

                return new ConfigureOptions<TOptions>(options => configureOptions(configuration, options));
            });
        }
    }
}
