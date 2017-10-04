using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Orleans.Runtime.Configuration;
using System;
using System.IO;

namespace Orleans.Configuration
{
    public static class LegacyConfigurationExtensions
    {
        public static IServiceCollection AddLegacyClusterConfigurationSupport(this IServiceCollection services)
        {
            // Global configuration

            services.Configure<ClusterConfiguration, SiloMessagingOptions>((configuration, options) =>
            {
                CopyCommonMessagingOptions(configuration.Globals, options);

                options.SiloSenderQueues = configuration.Globals.SiloSenderQueues;
                options.GatewaySenderQueues = configuration.Globals.GatewaySenderQueues;
                options.MaxForwardCount = configuration.Globals.MaxForwardCount;
                options.ClientDropTimeout = configuration.Globals.ClientDropTimeout;
            });

            services.Configure<ClusterConfiguration, SerializationProviderOptions>((configuration, options) =>
            {
                options.SerializationProviders = configuration.Globals.SerializationProviders;
                options.FallbackSerializationProvider = configuration.Globals.FallbackSerializationProvider;
            });

            return services;
        }

        public static IServiceCollection AddLegacyNodeConfigurationSupport(this IServiceCollection services)
        {
            services.Configure<NodeConfiguration, TraceOptions>((configuration, options) =>
            {
                options.TraceFileName = configuration.TraceFileName;
                options.TraceFilePattern = configuration.TraceFilePattern;
            });

            services.PostConfigureTraceOptions("Client");

            return services;
        }

        public static IServiceCollection AddLegacyClientConfigurationSupport(this IServiceCollection services)
        {
            // Global configuration

            services.Configure<ClientConfiguration, ClientMessagingOptions>((configuration, options) =>
            {
                CopyCommonMessagingOptions(configuration, options);

                options.ClientSenderBuckets = configuration.ClientSenderBuckets;
            });

            services.Configure<ClientConfiguration, SerializationProviderOptions>((configuration, options) =>
            {
                options.SerializationProviders = configuration.SerializationProviders;
                options.FallbackSerializationProvider = configuration.FallbackSerializationProvider;
            });

            services.Configure<ClientConfiguration, TraceOptions>((configuration, options) =>
            {
                //options.TraceFileName = configuration.TraceFileName;
                //options.TraceFilePattern = configuration.TraceFilePattern;
            });

            //TODO Figure out how to get Silo.Name here, host name and pass it on
            services.PostConfigureTraceOptions("Silo", null);

            return services;
        }

        private static void CopyCommonMessagingOptions(IMessagingConfiguration configuration, MessagingOptions options)
        {
            options.OpenConnectionTimeout = configuration.OpenConnectionTimeout;
            options.ResponseTimeout = configuration.ResponseTimeout;
            options.MaxResendCount = configuration.MaxResendCount;
            options.ResendOnTimeout = configuration.ResendOnTimeout;
            options.MaxSocketAge = configuration.MaxSocketAge;
            options.DropExpiredMessages = configuration.DropExpiredMessages;
            options.BufferPoolBufferSize = configuration.BufferPoolBufferSize;
            options.BufferPoolMaxSize = configuration.BufferPoolMaxSize;
            options.BufferPoolPreallocationSize = configuration.BufferPoolPreallocationSize;
        }

        private static void Configure<TConfiguration, TOptions>(this IServiceCollection services, Action<TConfiguration, TOptions> configureOptions) where TOptions : class
        {
            services.AddSingleton<IConfigureOptions<TOptions>>(serviceProvider =>
            {
                var configuration = serviceProvider.GetRequiredService<TConfiguration>();

                return new ConfigureOptions<TOptions>(options => configureOptions(configuration, options));
            });
        }

        private static void PostConfigureTraceOptions(this IServiceCollection services, string nodeName, string hostName = null)
        {
            if (String.IsNullOrEmpty(nodeName))
            {
                throw new ArgumentNullException(nameof(nodeName));
            }

            services.PostConfigure<TraceOptions>(options =>
            {
                const string dateFormat = "yyyy-MM-dd-HH.mm.ss.fffZ";

                // If the user explicitly set it to null from default value we've to honor it here. 
                // If we'd set the default value here, we can't detect if the user set it to null or it was the default value of the string.
                if (String.IsNullOrWhiteSpace(options.TraceFilePattern)
                    || options.TraceFilePattern.Equals("false", StringComparison.OrdinalIgnoreCase)
                    || options.TraceFilePattern.Equals("none", StringComparison.OrdinalIgnoreCase))
                {
                    options.TraceFileName = null;
                }
                else if (String.IsNullOrEmpty(options.TraceFileName))
                {
                    options.TraceFileName = null; // Normalize to null
                }
                else
                {
                    // Set trace filename only if it is not set
                    if (String.IsNullOrWhiteSpace(options.TraceFileName))
                    {
                        var traceFileDirectory = Path.GetDirectoryName(options.TraceFilePattern);

                        if (!String.IsNullOrEmpty(traceFileDirectory) && !Directory.Exists(traceFileDirectory))
                        {
                            throw new InvalidOperationException($"Trace file pattern: \"{options.TraceFilePattern}\" contains a directory reference to a non-existant directory. If a directory is part of the pattern it must be exists.");
                        }


                        options.TraceFileName = String.Format(options.TraceFilePattern, nodeName, DateTime.UtcNow.ToString(dateFormat), hostName);
                    }
                }
            });
        }
    }
}
