using WebApplication1.Models;

namespace WebApplication1.Services
{
    public interface IAPIInterface<T>
    {
        Task<List<ExchangeRate>> GetDataFromAPI();
    }
}
