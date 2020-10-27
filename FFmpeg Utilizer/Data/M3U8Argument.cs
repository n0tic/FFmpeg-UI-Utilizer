using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg_Utilizer.Data
{
    public class M3U8Argument
    {
        public string name;
        public string url;

        public M3U8Argument(string name, string url)
        {
            this.name = name;
            this.url = url;
        }
    }
}
