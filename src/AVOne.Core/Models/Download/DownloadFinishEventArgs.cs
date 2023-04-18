﻿// Copyright (c) 2023 Weloveloli. All rights reserved.
// See License in the project root for license information.

#nullable disable

namespace AVOne.Models.Download
{
    using AVOne.Models.Job;

    public class DownloadFinishEventArgs : JobStatusArgs
    {
        public string FinalFilePath { get; set; }
    }
}
