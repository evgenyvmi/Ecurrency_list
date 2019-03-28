using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Ecurrency
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public string LogoUrl { get; set; }
        public float Capitalization { get; set; }
        public float Price { get; set; }
        public float Percent_change_1h { get; set; }
        public float Percent_change_24h { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
