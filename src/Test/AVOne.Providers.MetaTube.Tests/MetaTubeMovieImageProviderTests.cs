﻿// Copyright (c) 2023 Weloveloli. All rights reserved.
// Licensed under the Apache V2.0 License.

namespace AVOne.Providers.MetaTube.Tests
{
    using Xunit;
    using AutoFixture;
    using Microsoft.Extensions.Logging;
    using Moq;
    using AVOne.Models.Item;
    using AVOne.Providers.Metatube;
    using AVOne.Configuration;

    public class MetaTubeMovieImageProviderTests : BaseTestCase
    {
        private readonly MetaTubeMovieImageProvider _provider;
        private readonly TestApplicationConfigs config;
        private readonly string metaTubeServerUrl;
        private readonly bool disableHttpTest;

        public MetaTubeMovieImageProviderTests()
        {
            metaTubeServerUrl = Environment.GetEnvironmentVariable("MetaTubeServerUrl");
            disableHttpTest = bool.Parse(Environment.GetEnvironmentVariable("disableHttpTest") ?? "false");
            config = new TestApplicationConfigs();
            config.MetaTube.Server = metaTubeServerUrl;
            var mockManager = fixture.Freeze<Mock<IConfigurationManager>>();
            mockManager.Setup(m => m.CommonConfiguration).Returns(config);
            var logMock = fixture.Freeze<Mock<ILogger<MetatubeMovieMetaDataProvider>>>();
            fixture.Register((IConfigurationManager manager) => new MetatubeApiClient(new HttpClient(), manager));
            _provider = fixture.Build<MetaTubeMovieImageProvider>().Create();
        }
        [SkippableFact]
        public async Task MetaTubeMovieImageProviderTest()
        {
            Skip.If(string.IsNullOrEmpty(metaTubeServerUrl) || disableHttpTest);

            Skip.If(string.IsNullOrEmpty(metaTubeServerUrl) || disableHttpTest);
            var data = await _provider.GetImages(new PornMovie
            {
                Name = "stars-507",
                ProviderIds = new Dictionary<string, string>
                {
                    { "MetaTube","AVWIKI:STARS-507"}
                }

            }, default);

            Assert.NotNull(data);
            Assert.True(data.Count() >= 1);
        }
    }
}
