using FakeUserDataGenerator.Models.UsersData;
using Microsoft.EntityFrameworkCore;


namespace FakeUserDataGenerator.Data
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {
        }
        public DbSet<Name> Names { get; set; }
        public DbSet<Surname> Surnames { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Street> Streets { get; set; }
        public DbSet<Building> Buildings { get; set; }
        public DbSet<Apartment> Apartments { get; set; }
        public DbSet<Mobile> Mobiles { get; set; }
    }



}
