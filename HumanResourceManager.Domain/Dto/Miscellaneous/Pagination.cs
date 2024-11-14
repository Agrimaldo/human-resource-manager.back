
namespace HumanResourceManager.Domain.Dto.Miscellaneous
{
    public class Pagination<T> where T : class
    {
        public int Page { get; set; }
        public int Total { get; set; }
        public IList<T>? Content { get; set; } = new List<T>();
    }
}
