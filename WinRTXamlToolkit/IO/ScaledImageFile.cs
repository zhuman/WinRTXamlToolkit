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

            if (!mainResourceMap.ContainsKey(resourceKey))
            {
                return null;
            }

            // ResourceContext.GetForCurrentView() makes it get the version of the resource for the scale used in the current view/screen
            return await mainResourceMap[resourceKey].Resolve(/*
                TODO ResourceContext.GetForCurrentView and ResourceContext.GetForViewIndependentUse do not exist in Windows App SDK
                Use your ResourceManager instance to create a ResourceContext as below. If you already have a ResourceManager instance,
                replace the new instance created below with correct instance.
                Read: https://docs.microsoft.com/en-us/windows/apps/windows-app-sdk/migrate-to-windows-app-sdk/guides/mrtcore
            */new Microsoft.Windows.ApplicationModel.Resources.ResourceManager().CreateResourceContext()).GetValueAsFileAsync();
        }
    }
}
