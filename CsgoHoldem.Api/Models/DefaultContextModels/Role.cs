using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CsgoHoldem.Api.Models.DefaultContextModels
{
    [Table(nameof(Role))]
    public class Role
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR(45)")]
        public string RoleName { get; set; }

        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Unique<Role>(r => r.RoleName);
        }
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(new Role() { Id = 1, RoleName = "User"});
            modelBuilder.Entity<Role>().HasData(new Role() {Id = 2, RoleName = "Admin"});
        }
    }
}