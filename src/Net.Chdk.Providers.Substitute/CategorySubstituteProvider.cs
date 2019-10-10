﻿using Microsoft.Extensions.Logging;
using Net.Chdk.Meta.Model.Address;
using Net.Chdk.Model.Software;
using Net.Chdk.Providers.Firmware;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Net.Chdk.Providers.Substitute
{
    sealed class CategorySubstituteProvider : DataProvider<Dictionary<string, AddressPlatformData>>, ICategorySubstituteProvider
    {
        private IFirmwareProvider FirmwareProvider { get; }
        private string CategoryName { get; }

        public CategorySubstituteProvider(IFirmwareProvider firmwareProvider, string categoryName, ILogger logger)
            : base(logger)
        {
            FirmwareProvider = firmwareProvider;
            CategoryName = categoryName;
        }

        public IDictionary<string, object>? GetSubstitutes(SoftwareInfo software)
        {
            if (software.Model == null || software.Camera == null)
                return null;

            var name = software.Model.Name;
            if (name == null)
                return null;

            var platform = software.Camera.Platform;
            var revision = software.Camera.Revision;
            if (platform == null || revision == null)
                return null;

            var revisionStr = GetRevisionString(revision);
            if (revisionStr == null)
                return null;

            var subs = new Dictionary<string, object>
            {
                ["model"] = name
            };

            if (!Data.TryGetValue(platform, out AddressPlatformData platformData))
            {
                subs["platforms"] = Data.Select(kvp => kvp.Key);
                return subs;
            }

            subs["platform"] = platform;
            subs["platform_id"] = GetHexString(platformData.Id);
            subs["platform_id_address"] = GetHexString(platformData.IdAddress);
            subs["model_id"] = GetHexString(software.Model.Id);

            if (platformData.Revisions == null)
            {
                subs["error"] = "Download_InvalidFormat_Text";
                return subs;
            }

            if (!platformData.Revisions.TryGetValue(revision, out AddressRevisionData revisionData))
            {
                subs["revisions"] = platformData.Revisions.Select(kvp => kvp.Key);
                return subs;
            }

            subs["revision"] = revision;
            subs["revision_str"] = revisionStr;
            subs["revision_str_address"] = GetHexString(revisionData.RevisionAddress);
            subs["palette_buffer_ptr"] = GetHexString(revisionData.PaletteBufferPtr);
            subs["active_palette_buffer"] = GetHexString(revisionData.ActivePaletteBuffer);
            subs["palette_to_zero"] = revisionData.PaletteToZero;

            return subs;
        }

        protected override string GetFilePath()
        {
            return Path.Combine(Directories.Data, Directories.Category, CategoryName, "addresses.json");
        }

        private string? GetRevisionString(string revision)
        {
            return FirmwareProvider.GetRevisionString(revision, CategoryName);
        }

        private static string GetHexString<T>(T value)
        {
            return $"0x{value:x}";
        }
    }
}
