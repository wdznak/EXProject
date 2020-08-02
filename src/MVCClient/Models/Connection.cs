using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MVCClient.Models
{
    public class Connection
    {
        public int ID { get; set; }
        [StringLength(15)]
        public string Symbol { get; set; }
        public string Exchange { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }

        public ICollection<Whale1H> Whale1H { get; set; }
    }
}
