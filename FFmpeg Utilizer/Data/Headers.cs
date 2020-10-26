using System.Collections.Generic;
using System.Linq;

namespace FFmpeg_Utilizer.Data
{
    public class Headers
    {
        public List<string> headers = new List<string>();

        public void AddHeaders(string[] _headers) => headers = _headers.ToList();
    }
}