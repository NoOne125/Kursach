using System;
using System.Collections.Generic;

namespace API
{
    public class UserDbRepository
    {
        public string chat_id { get; set; }
        public List<string> name_pl { get; set; }
        public List<string> name_ro { get; set; }
        public List<List<string>> playlists { get; set; }
        public List<List<string>> routes { get; set; }

    }
}
