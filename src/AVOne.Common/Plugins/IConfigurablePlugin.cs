﻿// Copyright (c) 2023 Weloveloli. All rights reserved.
// See License in the project root for license information.

namespace AVOne.Common.Plugins
{
    public interface IConfigurablePlugin
    {
        public IEnumerable<IPluginConfigField> GetConfiguration();

        public void SetConfiguration(IEnumerable<IPluginConfigField> fields);
    }
}
