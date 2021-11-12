using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Reimbursement_Web_System.Models
{
    public class ReimbursementContext : DbContext
    {
        public ReimbursementContext() : base(System.Configuration.ConfigurationManager.ConnectionStrings["Database"].ConnectionString) //get connection string from webc.onfig
        {
            Database.SetInitializer<ReimbursementContext>(null);
        }
        //initialize the tables
        public DbSet<User> User { get; set; }
        public DbSet<Ticket> Ticket { get; set; }
        public DbSet<Reimbursement> Reimbursement { get; set; }
        public DbSet<Media> Media { get; set; }
        public DbSet<Notification> Notification { get; set; }
    }
}