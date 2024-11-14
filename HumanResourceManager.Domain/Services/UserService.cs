
using HumanResourceManager.Domain.Dto.Miscellaneous;
using HumanResourceManager.Domain.Entities;
using HumanResourceManager.Domain.Interfaces;
using HumanResourceManager.Domain.Util;
using Serilog;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HumanResourceManager.Domain.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger _logger;
        private readonly IRepository _repo;
        public UserService(ILogger logger, IRepository repository)
        {
            _logger = logger;
            _repo = repository;
        }
        public async Task<Response<User>> Create(User user)
        {
            return await Task<Response<User>>.Run(() => {
                var errors = InputValidate(user);
                if (errors.Count > 0)
                {
                    _logger.Error($"Create User :  {JsonSerializer.Serialize(user)} \r\n Msg: {string.Join(",",errors)} ");
                    return new Response<User>() { Success = false, Messages = errors, Data = null };
                }
                _logger.Information($"Create User : {JsonSerializer.Serialize(user)}");
                try
                {
                    user.Birthdate = DateTime.SpecifyKind(user.Birthdate, DateTimeKind.Utc);
                    user.Password = Encrypter.ComputeMD5Hash(user.Password);
                    _repo.Add<User>(user);
                    return new Response<User>() { Success = true, Messages = [],Data = user };
                }
                catch (Exception ex)
                {
                    _logger.Error($"Create User :  {JsonSerializer.Serialize(user)} \r\n Msg: {ex.Message} ");
                    return new Response<User>() { Success = false, Messages = [ex.Message], Data = null };
                }
            });
        }

        public async Task<Response<dynamic>> Delete(int id)
        {
            return await Task<Response<dynamic>>.Run(() => {
                _logger.Information($"Delete User : {id}");
                try
                {
                    User? user = _repo.List<User>(0, 1, a => a.Id.Equals(id))?.FirstOrDefault();
                    if (user != null)
                    {
                        _repo.Delete<User>(user!);
                        return new Response<dynamic>() { Success = true, Messages = [], Data = null };
                    }

                    _logger.Warning($"Delete User (User Not Exists) : {id}");
                    return new Response<dynamic>() { Success = false, Messages = ["Usuário não existe"], Data = null };

                }
                catch (Exception ex)
                {
                    _logger.Error($"Create User :  {id} \r\n Msg: {ex.Message} ");
                    return new Response<dynamic>() { Success = false, Messages = [ex.Message], Data = null };
                }
            });
        }

        public async Task<Pagination<User>> List(int skip = 0, int take = 50)
        {
            return await Task.Run(() => {
                _logger.Information($"List User (Params): skip={skip}, take={take}");
                try
                {
                    return new Pagination<User> { Page = skip == 0 ? skip + 1 : (int)(skip/take) > 0 ? (int)(skip / take) : 1, Total = _repo.Count<User>(), Content = _repo.List<User>(skip, take) };
                }
                catch (Exception ex)
                {
                    _logger.Error($"List User (Params): skip={skip}, take={take} \r\n Msg: {ex.Message} ");
                }

                return new Pagination<User> { Page = skip + 1, Total = 0, Content = new List<User>() };
            });

        }

        public async Task<Response<User>> Update(User user)
        {
            return await Task<Response<User>>.Run(() => {
                var errors = InputValidate(user);
                if (errors.Count > 0)
                {
                    _logger.Error($"Create User :  {JsonSerializer.Serialize(user)} \r\n Msg: {string.Join(",", errors)} ");
                    return new Response<User>() { Success = false, Messages = errors, Data = null };
                }

                _logger.Information($"Update User : {JsonSerializer.Serialize(user)}");
                try
                {
                    if (_repo.Exists<User>(a => a.Id.Equals(user.Id)))
                    {
                        user.Birthdate = DateTime.SpecifyKind(user.Birthdate, DateTimeKind.Utc);
                        user.Password = Encrypter.ComputeMD5Hash(user.Password);
                        _repo.Update<User>(user);
                        return new Response<User>() { Success = true, Messages = [], Data = user };
                    }

                    _logger.Warning($"Update User (User Not Exists) : {JsonSerializer.Serialize(user)}");
                    return new Response<User>() { Success = false, Messages = ["Usuário não existe"], Data = user };
                    ;
                }
                catch (Exception ex)
                {
                    _logger.Error($"Update User :  {JsonSerializer.Serialize(user)} \r\n Msg: {ex.Message} ");
                    return new Response<User>() { Success = false, Messages = [ex.Message], Data = null };
                }
            });
        }

        private List<string> InputValidate(User user) 
        {
            List<string> errors = new List<string>();

            if (user.Name.Length < 3) 
            {
                errors.Add("Tamanho mínimo do campo nome é de 3 caracteres");
            }

            if (!Validator.IsEmail(user.Email))
            {
                errors.Add("E-mail inválido");
            }

            if (user.Password.Length < 6)
            {
                errors.Add("Tamanho mínimo do campo senha é de 6 caracteres");
            }

            return errors;
        }
    }
}
