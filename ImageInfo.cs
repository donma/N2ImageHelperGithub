using System;
using System.Collections.Generic;
using System.Text;

namespace N2ImageAgentGithub
{
    public class ImageBlobResult
    {
        public string Url { get; set; }
        public string Para { get; set; }

        public DateTime ExpireTime { get; set; }

        public string FullUrl
        {
            get
            {
                return Url + Para;
            }
        }
    }
    public class ImageInfo
    {
        public string Id { set; get; }
        public int Width { set; get; }
        public int Height { set; get; }
        public string Extension { set; get; }
        public string Tag { get; set; }
    }
}
