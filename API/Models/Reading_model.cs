using System.Collections.Generic;

namespace API
{
    public class List_Read_Models
    {
        public List<Reading_Model> results { get; set; }
    }

    public class Reading_Model
    {
        public Geometry geometry { get; set; }
        public string name { get; set; }
        public Opening_hours opening_hours { get; set; }
        public double rating { get; set; }
        public string vicinity { get; set; }
        public string place_id { get; set; }
    }

    public class Geometry
    {
        public Location location { get; set; }
    }

    public class Opening_hours
    {
        public bool open_now { get; set; }
        public Opening_hours(bool open_now)
        {
            this.open_now = open_now;
        }
    }

    public class Location
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }
}
