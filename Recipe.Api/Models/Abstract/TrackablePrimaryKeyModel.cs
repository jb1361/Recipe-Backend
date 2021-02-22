namespace Recipe.Api.Models.Abstract
{
    public class TrackablePrimaryKeyModel : Trackable, IPrimaryKeyModel
    {
        public int Id { get; set; }
    }
}