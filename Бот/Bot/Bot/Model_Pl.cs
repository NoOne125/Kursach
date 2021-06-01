using System;
using System.Collections.Generic;
using System.Text;

namespace Bot
{
    class Model_Pl
    {
        public int ind;
        public bool route_ch;
        public bool route;
        public bool open;
        public bool detals;
        public bool delete;
        public bool delete_pl;
        public bool add_pl;
        public List<string> playlists;
        public string playlist;
        public List<Place> places;

        public Model_Pl(int ind)
        {
            this.ind = ind;
        }
    }
}
