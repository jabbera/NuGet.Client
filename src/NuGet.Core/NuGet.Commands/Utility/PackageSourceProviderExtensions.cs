﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using NuGet.Configuration;

namespace NuGet.Commands
{
    public static class PackageSourceProviderExtensions
    {
        public static PackageSource ResolveSource(IEnumerable<PackageSource> availableSources, string source)
        {
            var resolvedSource = availableSources.FirstOrDefault(
                f => f.Source.Equals(source, StringComparison.OrdinalIgnoreCase) ||
                    f.Name.Equals(source, StringComparison.OrdinalIgnoreCase));

            if (resolvedSource == null)
            {
                ValidateSource(source);
                return new PackageSource(source);
            }
            else
            {
                return resolvedSource;
            }
        }

        public static string ResolveAndValidateSource(this IPackageSourceProvider sourceProvider, string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return null;
            }

            var sources = sourceProvider.LoadPackageSources().Where(s => s.IsEnabled);
            var result = ResolveSource(sources, source);
            ValidateSource(result.Source);
            return result.Source;
        }

        private static void ValidateSource(string source)
        {
            Uri result;
            if (!Uri.TryCreate(source, UriKind.Absolute, out result) && !Directory.Exists(source))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Strings.InvalidSource, source));
            }
        }
    }
}
