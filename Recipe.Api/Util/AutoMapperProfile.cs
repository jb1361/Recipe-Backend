using AutoMapper;
using Recipe.Api.Models.DefaultContextModels;
using Recipe.Api.Models.Requests;

namespace Recipe.Api.Util
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RegistrationRequest, User>();
        }
    }
}