using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API
{
    public class Return_model
    {
        List<Reading_Model> results;

        public Return_model(Reading_Model k)
        {
            results.Add(k);
        }

    }
}
