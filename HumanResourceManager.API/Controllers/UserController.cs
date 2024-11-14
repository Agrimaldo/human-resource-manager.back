using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HumanResourceManager.Domain.Dto.Miscellaneous;
using HumanResourceManager.Domain.Interfaces;
using HumanResourceManager.Domain.Entities;
using Microsoft.AspNetCore.Cors;

namespace HumanResourceManager.API.Controllers
{
    [ApiController, Route("api/[controller]"), EnableCors("General")]
    public class UserController : ControllerBase
    {
        private IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public Task<Pagination<User>> Get([FromQuery] int skip=0, int take=50)
        {
            return _userService.List(skip, take);
        }

        [HttpPost]
        public Task<Response<User>> Create([FromBody]User user)
        {
            return _userService.Create(user);
        }

        [HttpPut]
        public Task<Response<User>> Update([FromBody] User user)
        {
            return _userService.Update(user);
        }

        [HttpDelete, Route("{id?}")]
        public Task<Response<dynamic>> Delete([FromRoute] int id)
        {
            return _userService.Delete(id);
        }
    }
}
