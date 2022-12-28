using System;
using System.Threading.Tasks;
using Microsoft.Windows.ApplicationModel.Resources;
using Windows.Storage;

namespace WinRTXamlToolkit.IO
{
    /// <summary>
    /// Contains a helper method for getting a file with the applicable scale qualifier.
    /// </summary>
    public static class ScaledImageFile
    {
        /// <summary>
        /// Used to retrieve a StorageFile that uses qualifiers in the naming convention.
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public static async Task<StorageFile> GetAsync(string relativePath)
        {
            string resourceKey = string.Format("Files/{0}", relativePath);
            var mainResourceMap = new Microsoft.Windows.ApplicationModel.Resources.ResourceManager().MainResourceMap;

            var foundResource = mainResourceMap.TryGetValue(resourceKey);
            if (foundResource == null)
            {
                return null;
            }

            return await StorageFile.GetFileFromPathAsync(foundResource.ValueAsString);
        }
    }
}
