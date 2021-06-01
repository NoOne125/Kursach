using System;
using System.Collections.Generic;
using System.Text;

namespace Bot
{
    public class Model_Det
    {
        public Main result { get; set; }
    }

    public class Main
    {
        public Opening_hours_2 opening_hours { get; set; }
        public string international_phone_number { get; set; }
        public string website { get; set; }
        public Review[] reviews { get; set; }
        public string name { get; set; }
        public double rating { get; set; }
        public string vicinity { get; set; }
        public string place_id { get; set; }
    }

    public class Review
    {
        public string author_name { get; set; }
        public int rating { get; set; }
        public string relative_time_description { get; set; }
        public string text { get; set; }
    }

    public class Opening_hours_2
    {
        public bool open_now { get; set; }
        public string[] weekday_text { get; set; }
    }
}
