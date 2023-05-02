namespace WebApplication1.Services
{
    public interface IAPIInterface<T>
    {
        Task<HttpResponseMessage> GetDataFromAPI();
    }
}
