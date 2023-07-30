using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;

namespace MediaDesktop.UI.ViewModels
{
    /// <summary>
    /// Provides methods for common ui operations.
    /// </summary>
    public class CommonUIMethods
    {
        public static void DragOverEventHandlerForFileDrop(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = global::Windows.ApplicationModel.DataTransfer.DataPackageOperation.Copy;
        }

        public static async Task TextBoxGetDropFilePathEventHandler(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                IReadOnlyList<IStorageItem> items = await e.DataView.GetStorageItemsAsync();
                if (items.Count > 0)
                {
                    (sender as TextBox).SetValue(TextBox.TextProperty, items[0].Path);
                }
            }
        }
    }
}
