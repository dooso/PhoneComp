using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using PhoneComp.Models;

namespace PhoneComp.DAL
{
    public class PhoneCompContext:DbContext
    {
        public DbSet<UsedWorker> UsedWorkers { get; set; }
        public DbSet<Suspects> Suspectses { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<CallRecord> CallRecords { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //移除复数约定
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            
        }
    }
}