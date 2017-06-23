using Microsoft.EntityFrameworkCore;


namespace PersonalFinances.Models
{
    public class PFContext:DbContext
    {
        public DbSet<Currency> Currency { get; set; }
        public DbSet<Purse> Purse { get; set; }
        public DbSet<Costs> Costs { get; set; }
        public DbSet<CostCategories> CostCategories { get; set; }
        public DbSet<Income> Income { get; set; }
        public DbSet<SourceOfIncome> SourceOfIncome { get; set; }
        public DbSet<Displacement> Displacement { get; set; }
        public DbSet<Accumulation> Accumulation { get; set; }
        public DbSet<AccumulationOperation> AccumulationOperation { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=PersonalFinancesDB_Test14.db");
        }
    }
}
