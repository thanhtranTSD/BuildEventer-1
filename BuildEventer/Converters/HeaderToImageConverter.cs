/*
<License>
Copyright 2015 Virtium Technology
Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at
    http ://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
</License>
 */

using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

namespace BuildEventer.Converters
{
    #region HeaderToImageConverter
    public class HeaderToImageConverter : MarkupExtension, IValueConverter
    {
        private static HeaderToImageConverter s_HeaderToImageConverter;

        public HeaderToImageConverter() { }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (null == s_HeaderToImageConverter)
            {
                s_HeaderToImageConverter = new HeaderToImageConverter();
            }
            return s_HeaderToImageConverter;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string path = value as string;

            BitmapSource icon = GetIconDll(path);

            return icon;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back");
        }

        private BitmapSource GetIconDll(string fileName)
        {
            BitmapSource myIcon = null;

            Boolean validDrive = false;
            foreach (DriveInfo D in System.IO.DriveInfo.GetDrives())
            {
                if (fileName == D.Name)
                {
                    validDrive = true;
                }
            }

            if ((true == File.Exists(fileName)) || (true == Directory.Exists(fileName)) || (true == validDrive))
            {
                using (System.Drawing.Icon sysIcon = ShellIcon.GetIcon(fileName))
                {
                    try
                    {
                        myIcon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                                        sysIcon.Handle,
                                        System.Windows.Int32Rect.Empty,
                                        System.Windows.Media.Imaging.BitmapSizeOptions.FromWidthAndHeight(34, 34));
                    }
                    catch
                    {
                        myIcon = null;
                    }
                }
            }
            return myIcon;
        }

        private class ShellIcon
        {
            public ShellIcon() { }

            [StructLayout(LayoutKind.Sequential)]
            private struct SHFILEINFO
            {
                public IntPtr hIcon;
                public IntPtr iIcon;
                public uint dwAttributes;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
                public string szDisplayName;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
                public string szTypeName;
            };

            public static Icon GetIcon(string fileName)
            {
                // The handle to the system image list
                IntPtr hImgSmall;
                SHFILEINFO shinfo = new SHFILEINFO();

                uint fileAttributes = 0;
                uint flags = Win32.SHGFI_ICON | Win32.SHGFI_SMALLICON;
                hImgSmall = Win32.SHGetFileInfo(fileName, fileAttributes, ref shinfo, (uint)Marshal.SizeOf(shinfo), flags);

                // The icon is returned in the hIcon member of the shinfo struct
                Icon icon = (Icon)Icon.FromHandle(shinfo.hIcon).Clone();
                Win32.DestroyIcon(shinfo.hIcon);

                return icon;
            }

            private class Win32
            {
                [DllImport("shell32.dll")]
                public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);
                [DllImport("User32.dll")]
                public static extern int DestroyIcon(IntPtr hIcon);

                public const uint SHGFI_ICON = 0x100;
                // Small icon
                public const uint SHGFI_SMALLICON = 0x1;
            }
        }
    }
    #endregion
}
