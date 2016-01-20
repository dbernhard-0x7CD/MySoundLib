using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySoundLib
{
    public class Playlist
    {
        public string Name { get; set; }
        public int Id;
        int currentIndex { get; set; }
        List<int> songIds;

        public void Next()
        {
            currentIndex++;
        }
    }
}
