﻿using Files.Common;
using Files.Extensions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Files.Helpers
{
    public static class UIHelpers
    {
        public static bool IsAnyContentDialogOpen()
        {
            var openedPopups = VisualTreeHelper.GetOpenPopups(Window.Current);
            return openedPopups.Any(popup => popup.Child is ContentDialog);
        }

        public static async Task<IList<IconFileInfo>> LoadSelectedIconsAsync(string filePath, IList<int> indexes, NamedPipeAsAppServiceConnection connection, int iconSize = 48, bool rawDataOnly = true)
        {
            if (connection != null)
            {
                var value = new ValueSet();
                value.Add("Arguments", "GetSelectedIconsFromDLL");
                value.Add("iconFile", filePath);
                value.Add("requestedIconSize", iconSize);
                value.Add("iconIndexes", JsonConvert.SerializeObject(indexes));
                var (status, response) = await connection.SendMessageForResponseAsync(value);
                if (status == AppServiceResponseStatus.Success)
                {
                    var icons = JsonConvert.DeserializeObject<IList<IconFileInfo>>((string)response["IconInfos"]);

                    if (icons != null && !rawDataOnly)
                    {
                        foreach (IconFileInfo iFInfo in icons)
                        {
                            await iFInfo.LoadImageFromModelString();
                        }
                    }

                    return icons;
                }
            }
            return null;
        }
    }
}