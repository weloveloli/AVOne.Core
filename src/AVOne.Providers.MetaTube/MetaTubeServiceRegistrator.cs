﻿// Copyright (c) 2023 Weloveloli. All rights reserved.
// Licensed under the Apache V2.0 License.

namespace AVOne.Providers.MetaTube
{
    using AVOne.Abstraction;
    using AVOne.Providers.Metatube;
    using Microsoft.Extensions.DependencyInjection;

    public class MetaTubeServiceRegistrator : IServiceRegistrator
    {
        public void RegisterServices(IServiceCollection serviceCollection)
        {
            _ = serviceCollection
                .AddHttpClient<MetatubeApiClient>()
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
                    PooledConnectionIdleTimeout = TimeSpan.FromSeconds(90)
                });
        }

        public void RegisterStatic(IApplicationHost host)
        {

        }
    }
}
