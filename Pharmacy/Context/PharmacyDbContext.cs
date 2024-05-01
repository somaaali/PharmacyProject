
namespace Pharmacy.Context
{
	public class PharmacyDbContext :IdentityDbContext<ApplicationUser>
	{
		public PharmacyDbContext(DbContextOptions<PharmacyDbContext> options) : base(options)
		{
		}

		public DbSet<Medicine> medicines { get; set; }
		public DbSet<Request> requests { get; set; }
		public DbSet<SearchHistory> searchHistory { get; set; }

		public DbSet<Category> Categories { get; set; }
	}	
}
