using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CsgoHoldem.Api.Models
{
    [Table(nameof(SeedMigration))]
    public class SeedMigration
    {
        [Key]
        public string MigrationId { get; set; }
    }
}