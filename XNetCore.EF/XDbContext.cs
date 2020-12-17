using System;
using Microsoft.EntityFrameworkCore;
using XNetCore.STL;

namespace XNetCore.EF
{
    public class XDbContext : DbContext
    {
        private DbConfig config = null;
        public XDbContext(DbConfig config) : base()
        {
            this.config = config;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            new DbContextHelper().OnConfiguring(options, this.config);
        }
    }
}
