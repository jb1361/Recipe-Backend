using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using CsgoHoldem.Api.Models.Context;
using Microsoft.EntityFrameworkCore;

namespace CsgoHoldem.Api.Models.DefaultContextModels
{
    [Table(nameof(User))]
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR(45)")]
        public string Email { get; set; }
        
        [Required]
        public string Password { get; set; }
        
        public int RoleId { get; set; }
        [ForeignKey(nameof(RoleId))]
        public Role Role { get; set; }

        [NotMapped] public String RoleName => Role.RoleName;

        public string PasswordResetToken { get; set; }
        public DateTime? ResetTokenValidDate { get; set; }
        public DateTime? ArchivedAt { get; set; }

        [NotMapped]
        public string Token { get; set; }

        // This should be used whenever user data is being retrieved that should NOT expose sensitive data.
        // Password being null is the main thing that needs to be set null no matter who is retrieving data.
        public void SetNulls()
        {
            if (Email.Length == 0) {
                Email = null;
            }
            Password = null;
            PasswordResetToken = null;
            ResetTokenValidDate = null;
        }
        
        public async Task SetRole(DefaultContext context)
        {
            var role = await context.Roles.FindAsync(RoleId);
            Role = role;
        }
        
        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Unique<User>(u => u.Email);
        }
        
        public bool IsArchived()
        {
            return ArchivedAt != null;
        }
    }
}