namespace CsgoHoldem.Api.Models.Requests
{
    public interface IRequestConversion<T>
    {
        public T Value { get; }
    }
}