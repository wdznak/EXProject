using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCClient.Models
{
    public class Whale1H
    {
        public int ID { get; set; }
        public int ConnectionID { get; set; }
        public DateTime Time { get; set; }
        public float Market { get; set; }
        public float Limit { get; set; }
    }
}
