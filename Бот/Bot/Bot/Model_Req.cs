using System;
using System.Collections.Generic;
using System.Text;

namespace Bot
{
    class Model_Req
    {
        public bool sort_ch = false;
        public bool start = false;
        public bool detals = false;
        public bool add = false;
        public bool filter_ch = false;

        public bool open_now = false;
        public bool time_work = false;
        public bool site = false;
        public bool telef = false;
        public bool count_com = false;

        public double[] location;
        public int radius;
        public bool sort;
        public string type;
        public int index;
        public Model_List current_list;

        public Model_Req(double x, double y)
        {
            sort_ch = false;
            start = false;
            detals = false;
            radius = 0;
            sort = false;
            type = "";
            index = 0;
            current_list = new Model_List();
            location = new double[2] { x, y };
        }
    }
}
