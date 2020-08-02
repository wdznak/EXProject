using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVCClient.Models
{
    public class BinanceConnectionModel
    {
        public string Address { get; set; }
        public string Description { get; set; }
        [Display(Name = "Depth address")]
        public string DepthAddress { get; set; }
        public string Symbol { get; set; }
        public bool Store { get; set; }
    }
}
