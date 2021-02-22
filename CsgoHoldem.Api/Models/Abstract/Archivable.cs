using System;

namespace CsgoHoldem.Api.Models.Abstract
{
    public interface IArchivable
    {
        public DateTime? ArchivedAt { get; set; }
    }
    
    public abstract class Archivable : IArchivable
    {
        public DateTime? ArchivedAt { get; set; }
    }
}