﻿// Copyright (c) 2023 Weloveloli. All rights reserved.
// See License in the project root for license information.

namespace AVOne.Impl.Registrator
{
    using System.Net;
    using System.Net.Http.Headers;
    using AVOne.Abstraction;
    using AVOne.Configuration;
    using AVOne.Constants;
    using AVOne.Impl.Data;
    using AVOne.Impl.IO;
    using AVOne.Impl.Job;
    using AVOne.Impl.Library;
    using AVOne.Impl.Providers;
    using AVOne.Impl.Updates;
    using AVOne.IO;
    using AVOne.Library;
    using AVOne.Models.Item;
    using AVOne.Providers;
    using AVOne.Providers.Download;
    using AVOne.Providers.Extractor;
    using AVOne.Providers.Metadata;
    using AVOne.Resolvers;
    using AVOne.Updates;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public class ImplRegistrator : IServiceRegistrator
    {
        public void RegisterServices(IServiceCollection serviceCollection)
        {
            // ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
            serviceCollection.AddHttpClient(HttpClientNames.Default, (client) =>
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/94.0.4606.81 Safari/537.36 Edg/94.0.992.50");
                client.DefaultRequestHeaders.AcceptLanguage.Clear();
                client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("zh-CN"));
                client.DefaultRequestVersion = HttpVersion.Version20;
            })
            .ConfigurePrimaryHttpMessageHandler((builder) =>
            {
                var configManager = builder.GetService<IConfigurationManager>();
                var proxy = configManager.CommonConfiguration.ProviderConfig.Proxy;
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (m, c, ch, e) => { return true; },
                    AutomaticDecompression = DecompressionMethods.All
                };
                if (!string.IsNullOrEmpty(proxy) && proxy.StartsWith("http://") || proxy.StartsWith("https://"))
                {
                    handler.Proxy = new WebProxy(proxy);
                    handler.UseProxy = true;
                }

                return handler;

            });

            serviceCollection.AddHttpClient(HttpClientNames.Download, (client) =>
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/94.0.4606.81 Safari/537.36 Edg/94.0.992.50");
            }).ConfigurePrimaryHttpMessageHandler((builder) =>
            {
                var configManager = builder.GetService<IConfigurationManager>();
                var proxy = configManager.CommonConfiguration.DownloadConfig.Proxy;

                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (m, c, ch, e) => { return true; },
                    AutomaticDecompression = DecompressionMethods.All
                };
                if (!string.IsNullOrEmpty(proxy) && proxy.StartsWith("http://") || proxy.StartsWith("https://"))
                {
                    handler.Proxy = new WebProxy(proxy);
                    handler.UseProxy = true;
                }

                return handler;

            })
                .ConfigureHttpMessageHandlerBuilder(sp => new SocketsHttpHandler
                {
                    // Connect Timeout.
                    ConnectTimeout = TimeSpan.FromSeconds(30),
                    // TCP Keep Alive.
                    KeepAlivePingPolicy = HttpKeepAlivePingPolicy.Always,
                    KeepAlivePingDelay = TimeSpan.FromSeconds(30),
                    KeepAlivePingTimeout = TimeSpan.FromSeconds(30),

                    // Connection Pooling.
                    PooledConnectionLifetime = TimeSpan.FromMinutes(10),
                    PooledConnectionIdleTimeout = TimeSpan.FromSeconds(90),
                });
            serviceCollection.AddSingleton<IProviderManager, ProviderManager>();
            serviceCollection.AddSingleton<ILibraryManager, LibraryManager>();
            serviceCollection.AddSingleton<IInstallationManager, InstallationManager>();
            serviceCollection.AddSingleton<IFileSystem, ManagedFileSystem>();
            serviceCollection.AddSingleton<IDirectoryService, DirectoryService>();
            serviceCollection.AddSingleton(sp =>
            {
                return ApplicationDbContext.Create(sp.GetService<IApplicationPaths>());
            })
            .AddSingleton<JobRepository>()
            .AddSingleton<IJobManager, JobManager>();
        }

        public void PostBuildService(IApplicationHost host)
        {
            BaseItem.FileSystem = host.Resolve<IFileSystem>();
            BaseItem.Logger = host.Resolve<ILogger<BaseItem>>();
            BaseItem.ConfigurationManager = host.Resolve<IConfigurationManager>();
            IAVOneJob.JobManager = host.Resolve<IJobManager>();
            IAVOneJob.ApplicationHost = host.Resolve<IApplicationHost>();

            host.Resolve<IProviderManager>().AddParts(
                host.GetExports<IImageProvider>(),
                host.GetExports<IMetadataProvider>(),
                host.GetExports<INamingOptionProvider>(),
                host.GetExports<IVideoResolverProvider>(),
                host.GetExports<IMetadataSaverProvider>(),
                host.GetExports<IImageSaverProvider>(),
                host.GetExports<IDownloaderProvider>(),
                host.GetExports<IMediaExtractorProvider>()
                );

            host.Resolve<ILibraryManager>().AddParts(
                host.GetExports<IResolverIgnoreRule>(),
                host.GetExports<IItemResolver>());

            host.Resolve<IConfigurationManager>()
                .AddParts(host.GetExports<IConfigurationFactory>());
        }
    }
}
