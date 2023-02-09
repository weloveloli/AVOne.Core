﻿// Copyright (c) 2023 Weloveloli. All rights reserved.
// Licensed under the Apache V2.0 License.

#nullable disable

namespace AVOne.Models.Item
{
    using System.Text.Json.Serialization;
    using AVOne.Abstraction;
    using AVOne.Helper;
    using AVOne.Models.Info;

    public abstract class BaseItem : IHasProviderIds, IHasLookupInfo<ItemLookupInfo>
    {
        /// <summary>
        /// Gets or sets the provider ids.
        /// </summary>
        /// <value>The provider ids.</value>
        [JsonIgnore]
        public Dictionary<string, string> ProviderIds { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [JsonIgnore]
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the OriginalTitle.
        /// </summary>
        /// <value>The OriginalTitle.</value>
        [JsonIgnore]
        public string OriginalTitle { get; set; }

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>The path.</value>
        [JsonIgnore]
        public virtual string Path { get; set; }

        public DateTime DateLastSaved { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        /// <value>The date created.</value>
        [JsonIgnore]
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the date modified.
        /// </summary>
        /// <value>The date modified.</value>
        [JsonIgnore]
        public DateTime DateModified { get; set; }

        [JsonIgnore]
        public string Tagline { get; set; }

        public abstract ItemLookupInfo GetLookupInfo();

        /// <summary>
        /// Gets or sets the name of the sort.
        /// </summary>
        /// <value>The name of the sort.</value>
        [JsonIgnore]
        public string SortName { get; set; }

        public string ForcedSortName { get; set; }

        /// <summary>
        /// Gets or sets the official rating.
        /// </summary>
        /// <value>The official rating.</value>
        [JsonIgnore]
        public string OfficialRating { get; set; }

        /// <summary>
        /// Gets or sets the critic rating.
        /// </summary>
        /// <value>The critic rating.</value>
        [JsonIgnore]
        public float? CriticRating { get; set; }

        /// <summary>
        /// Gets or sets the custom rating.
        /// </summary>
        /// <value>The custom rating.</value>
        [JsonIgnore]
        public string CustomRating { get; set; }

        /// <summary>
        /// Gets or sets the overview.
        /// </summary>
        /// <value>The overview.</value>
        [JsonIgnore]
        public string Overview { get; set; }

        /// <summary>
        /// Gets or sets the studios.
        /// </summary>
        /// <value>The studios.</value>
        [JsonIgnore]
        public string[] Studios { get; set; }

        /// <summary>
        /// Gets or sets the genres.
        /// </summary>
        /// <value>The genres.</value>
        [JsonIgnore]
        public string[] Genres { get; set; }

        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        /// <value>The tags.</value>
        [JsonIgnore]
        public string[] Tags { get; set; }

        [JsonIgnore]
        public string PreferredMetadataLanguage { get; set; }
        public string PreferredMetadataCountryCode { get; set; }
        public string[] ProductionLocations { get; set; }

        /// <summary>
        /// Gets or sets the remote trailers.
        /// </summary>
        /// <value>The remote trailers.</value>
        public IReadOnlyList<MediaUrl> RemoteTrailers { get; set; }

        /// <summary>
        /// Gets or sets the production year.
        /// </summary>
        /// <value>The production year.</value>
        [JsonIgnore]
        public int? ProductionYear { get; set; }

        /// <summary>
        /// Gets or sets the home page URL.
        /// </summary>
        /// <value>The home page URL.</value>
        [JsonIgnore]
        public string HomePageUrl { get; set; }

        /// <summary>
        /// Gets or sets the community rating.
        /// </summary>
        /// <value>The community rating.</value>
        [JsonIgnore]
        public float? CommunityRating { get; set; }

        /// <summary>
        /// Gets or sets the date that the item first debuted. For movies this could be premiere date, episodes would be first aired.
        /// </summary>
        /// <value>The premiere date.</value>
        [JsonIgnore]
        public DateTime? PremiereDate { get; set; }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        /// <value>The end date.</value>
        [JsonIgnore]
        public DateTime? EndDate { get; set; }

        public bool SupportsLocalMetadata { get; set; } = true;
        /// <summary>
        /// Gets or sets the run time ticks.
        /// </summary>
        /// <value>The run time ticks.</value>
        [JsonIgnore]
        public long? RunTimeTicks { get; set; }
        [JsonIgnore]
        public virtual ItemImageInfo[] ImageInfos { get; set; }
        public List<PersonInfo> People { get; set; }

        public void AddPerson(PersonInfo p)
        {
            People ??= new List<PersonInfo>();

            PeopleHelper.AddPerson(People, p);
        }
    }
}
