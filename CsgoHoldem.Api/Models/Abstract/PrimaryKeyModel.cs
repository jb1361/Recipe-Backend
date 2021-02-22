namespace CsgoHoldem.Api.Models.Abstract
{
    public interface IPrimaryKeyModel
    {
        int Id { get; set; }
    }
    public abstract class PrimaryKeyModel : IPrimaryKeyModel
    {
        public int Id { get; set; }
    }
}