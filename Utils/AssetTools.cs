using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace BananaWatch.Utils
{
    class AssetTools
    {
        private static string FormatPath(string path)
        {
            return path.Replace("/", ".").Replace("\\", ".");
        }

        public static AssetBundle LoadAssetBundle(string path)
        {
            path = FormatPath(path);
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);

            if (stream == null)
            {
                return null;
            }

            AssetBundle bundle = AssetBundle.LoadFromStream(stream);
            if (bundle == null)
            {
                return null;
            }

            stream.Close();
            return bundle;
        }

    }
}
