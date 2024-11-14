using HumanResourceManager.Domain.Dto.Miscellaneous;
using HumanResourceManager.Domain.Entities;

namespace HumanResourceManager.Domain.Interfaces
{
    public interface IUserService
    {
        Task<Response<User>> Create(User user);
        Task<Response<User>> Update(User user);
        Task<Response<dynamic>> Delete(int id);
        Task<Pagination<User>> List(int skip = 0, int take = 50);
    }
}
