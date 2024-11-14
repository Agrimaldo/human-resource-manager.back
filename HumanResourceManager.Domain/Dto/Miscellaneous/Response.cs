
namespace HumanResourceManager.Domain.Dto.Miscellaneous
{
    public class Response<T>
    {
        public bool Success { get; set; }
        public IList<string> Messages { get; set; } = new List<string>();
        public T? Data { get; set; }
    }
}
