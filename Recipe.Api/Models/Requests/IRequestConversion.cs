namespace Recipe.Api.Models.Requests
{
    public interface IRequestConversion<T>
    {
        public T Value { get; }
    }
}