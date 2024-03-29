using System;

namespace Recipe.Api.Models.Abstract
{
    public interface ICreatedAtTrackable
    {
        DateTime CreatedAt { get; set; }
    }
    public abstract class CreatedAtTrackable : ICreatedAtTrackable
    {
        public DateTime CreatedAt { get; set; }
    }
}