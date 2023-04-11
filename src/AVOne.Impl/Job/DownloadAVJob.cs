﻿// Copyright (c) 2023 Weloveloli. All rights reserved.
// See License in the project root for license information.

namespace AVOne.Impl.Job
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using AVOne.Extensions;
    using AVOne.Impl.Data;
    using AVOne.Impl.Json;
    using AVOne.Models.Download;
    using AVOne.Models.Job;
    using AVOne.Providers;
    using AVOne.Providers.Download;

    public class DownloadAVJob : IAVOneJob
    {
        public BaseDownloadableItem? DownloadableItem { get; set; }

        public DownloadOpts? DownloadOpts { get; set; }

        public string? ItemType => DownloadableItem?.GetType().FullName;

        public string? DownloadProvider;

        public override string Type => "DownloadAVJob";

        public override string Key => DownloadableItem?.Key.GetMD5().ToString() ?? string.Empty;

        private string? _name;

        public override string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                {
                    if (DownloadableItem is not null)
                    {
                        _name = DownloadableItem.SaveName;
                    }
                }
                return _name ?? string.Empty;
            }
            set
            {
                _name = value;
            }
        }
        private string? _description;

        public override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    if (DownloadableItem is not null)
                    {
                        _description = DownloadableItem.DisplayName;
                    }
                }
                return _description ?? string.Empty;
            }
            set
            {
                _description = value;
            }
        }

        public long? Speed { get; set; }
        public int? Eta { get; set; }

        public override Task Execute(CancellationToken cancellationToken)
        {
            if (DownloadableItem == null || DownloadOpts == null)
            {
                return Task.CompletedTask;
            }
            var providerManager = ApplicationHost.Resolve<IProviderManager>();
            var providers = providerManager.GetDownloaderProviders(DownloadableItem);
            IDownloaderProvider? downloadProvider;
            if (DownloadProvider == null)
            {
                downloadProvider = providers.FirstOrDefault();
            }
            else
            {
                downloadProvider = providers.Where(e => e.Name == DownloadProvider).FirstOrDefault();
            }

            if (downloadProvider == null)
            {
                throw new Exception("No download provider");
            }
            DownloadOpts.StatusChanged += DownloadOpts_StatusChanged;
            return downloadProvider.CreateTask(DownloadableItem, DownloadOpts, cancellationToken);
        }

        private void DownloadOpts_StatusChanged(object? sender, JobStatusArgs e)
        {
            this.Progress?.Report(e);
        }

        protected override Dictionary<string, string> BuildExtra()
        {
            var extra = new Dictionary<string, string>
            {
                { "ItemType", ItemType! },
                { "DownloadOpts", JsonSerializer.Serialize(DownloadOpts, JsonDefaults.Options) },
                { "Item", JsonSerializer.Serialize(DownloadableItem, JsonDefaults.Options) },
            };
            if (DownloadProvider != null)
            {
                extra.Add("DownloadProvider", DownloadProvider);
            }
            if (Speed.HasValue)
            {
                extra.Add("Speed", Speed.Value.ToString());
            }
            if (Eta.HasValue)
            {
                extra.Add("Eta", Eta.Value.ToString());
            }
            return extra;
        }

        protected override void FromExtra(Dictionary<string, string> extra)
        {
            var itemType = extra["ItemType"];
            var item = extra["Item"];
            if (extra.TryGetValue("DownloadProvider", out var downloadProvider))
            {
                DownloadProvider = downloadProvider;
            }
            if (extra.TryGetValue("DownloadOpts", out var downloadOpts))
            {
                DownloadOpts = JsonSerializer.Deserialize<DownloadOpts>(downloadOpts, JsonDefaults.Options);
            }
            if (extra.TryGetValue("Speed", out var speed))
            {
                Speed = long.Parse(speed.ToString());
            }
            if (extra.TryGetValue("Eta", out var eta))
            {
                Eta = int.Parse(eta.ToString());
            }
            var type = Assembly.GetAssembly(typeof(BaseDownloadableItem))!.GetType(itemType);
            DownloadableItem = JsonSerializer.Deserialize(item, type, JsonDefaults.Options) as BaseDownloadableItem;
        }

        public override void UpdateStatus(JobStatusArgs jobStatusArgs)
        {
            base.UpdateStatus(jobStatusArgs);
            if (jobStatusArgs is DownloadProgressEventArgs progressEventArgs)
            {
                this.Speed = progressEventArgs.Speed;
                this.Eta = progressEventArgs.Eta;
            }
        }
    }
}
