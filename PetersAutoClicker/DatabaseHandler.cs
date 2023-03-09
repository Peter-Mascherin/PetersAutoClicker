using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PetersAutoClicker
{
    public class DatabaseHandler : DbContext
    {
        public DbSet<HotkeyEntity> Hotkeys { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True");
        }
    }


    
}
