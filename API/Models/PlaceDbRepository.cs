using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API
{
    public class PlaceDbRepository
    {
        public string place_id { get; set; }
        public List<double> location { get; set; }
        public string name { get; set; }
        public double rating {get; set;}
        public string vicinity { get; set; }
    }
}
