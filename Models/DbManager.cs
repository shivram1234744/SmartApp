using Microsoft.EntityFrameworkCore;

namespace TrustPlus.Models
{
    public class DbManager : DbContext
    {
        public DbManager(DbContextOptions<DbManager> options)
            : base(options)
        {
        }

        public DbSet<LoginModel> User_mst { get; set; }

        public DbSet<Mst_VerificationCheck> Mst_VerificationChecks { get; set; }

        public DbSet<ClientDtl> Client_Dtl { get; set; }

        public DbSet<Verification> Verification_Status { get; set; }

        public DbSet<EmployeeMst>EmployeeMst { get; set; }
        // =========================================
        // Trigger Configuration
        // =========================================
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Verification>()
                .ToTable("tbl_verification", tb =>
                {
                    tb.HasTrigger("trg_UpdateClientStatus");
                });
        }

    }
}