using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace N2ImageAgentGithub
{
    public class Utility
    {

        /// <summary>
        /// Get Image Info from File.
        /// </summary>
        /// <param name="localImagePath"></param>
        /// <param name="id"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static ImageInfo GetImageInfo(string localImagePath, string id, string tag)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException("id");
            if (string.IsNullOrEmpty(localImagePath)) throw new ArgumentNullException("localImagePath");
            var source = Image.FromFile(localImagePath);

            var imgInfo = new ImageInfo();
            imgInfo.Id = id;
            imgInfo.Width = source.Width;
            imgInfo.Height = source.Height;
            imgInfo.Extension = ImageUtility.GetImageFormat(source).ToString().ToLower();
            imgInfo.Tag = tag;

            return imgInfo;
        }
    }
}
