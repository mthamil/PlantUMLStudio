using System;
using System.Collections.Generic;
using SharpEssentials.Collections;

namespace PlantUmlStudio.Core.Dependencies.Update
{
    /// <summary>
    /// Compares strings that are in a version format, ie. {major}.{minor}.{build}.
    /// </summary>
    public class NormalizingVersionComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            Version xVersion;
            Version yVersion;
            if (Version.TryParse(Normalize(x), out xVersion) && Version.TryParse(Normalize(y), out yVersion))
                return xVersion == yVersion;

            return false;
        }

        private static string Normalize(string version)
        {
            var components = version.Split('.');
            var normalized = new string[4];
            for (var i = 0; i < normalized.Length; i++)
            {
                normalized[i] = components.Length <= i || String.IsNullOrEmpty(components[i])
                                    ? "0" 
                                    : components[i];
            }

            return normalized.ToDelimitedString(".");
        }

        public int GetHashCode(string obj)
        {
            Version version;
            return Version.TryParse(Normalize(obj), out version) 
                ? version.GetHashCode() 
                : 0;
        }
    }
}