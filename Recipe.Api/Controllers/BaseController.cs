using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Recipe.Api.Config;
using Recipe.Api.Models.Context;
using Recipe.Api.Models.DefaultContextModels;

namespace Recipe.Api.Controllers
{
    public class BaseController: ControllerBase
    {
        protected readonly DefaultContext DatabaseContext;
        private User user = null;
        private readonly IMapper mapper;
        
        protected AppSettings AppSettings { get; }
        
        protected T Map<T>(object value)
        {
            return mapper.Map<T>(value);
        }
        
        public BaseController(BaseControllerDependencies dependencies)
        {
            DatabaseContext = dependencies.DatabaseContext;
            mapper = dependencies.Mapper;
            AppSettings = dependencies.AppSettings;
        }
        
        public async Task<User> GetUser()
        {
            if (user != null)
                return user;
            try
            {
                user = await DatabaseContext.Users.SingleOrDefaultAsync(u => u.Id == int.Parse(this.User.Identity.Name));
                user.SetNulls();
                return user;
            }
            catch (ArgumentNullException) { return null; }
            catch (FormatException) { return null; }
            catch (OverflowException) { return null; }
        }


        public IActionResult GetResponse<T>(T value)
        {
            if (value == null)
                return NotFound();
            return Ok(value);
        }
    }
}