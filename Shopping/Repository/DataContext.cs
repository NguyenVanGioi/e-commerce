using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shopping.Models;

namespace Shopping.Repository
{
    public class DataContext : IdentityDbContext<AppUserModel>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<BrandModel> Brands { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<ProductModel> Products { get; set; }
        public DbSet<OrderModel> Orders { get; set; }
        public DbSet<OrderDetailModel> OrderDetails { get; set; }
        public DbSet<CommentModel> Comments { get; set; }
        public DbSet<PostModel> Posts { get; set; }
        public DbSet<TransactStatusModel> TransactStatus { get; set; }
        public DbSet<AddressModel> Address { get; set; }
    }
}
