using ArvatoAssessment.Common;
using ArvatoAssessment.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArvatoAssessment.DataAcess.Entity
{
    class RequestContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(DBConnection.ConnectionString);
        }

        public DbSet<RequestLog> RequestLogs { get; set; }
    }
}
