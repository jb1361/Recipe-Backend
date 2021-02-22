using System;

namespace Recipe.Api.Models.Abstract
{
    public interface ITrackable : ICreatedAtTrackable
    {
        public DateTime UpdatedAt { get; set; }
    }

    public abstract class Trackable : CreatedAtTrackable, ITrackable
    {
        public DateTime UpdatedAt { get; set; }
    }
}