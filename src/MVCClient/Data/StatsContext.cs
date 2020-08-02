using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MVCClient.Models;

namespace MVCClient.Data
{
    public class StatsContext : DbContext
    {
        public DbSet<Connection> Connections { get; set; }
        public DbSet<Whale1H> Whale1Hs { get; set; }
        public StatsContext(DbContextOptions<StatsContext> options)
            : base(options)
        {
               
        }
    }
}
