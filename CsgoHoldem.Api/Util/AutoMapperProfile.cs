using AutoMapper;
using CsgoHoldem.Api.Models.DefaultContextModels;
using CsgoHoldem.Api.Models.Requests;

namespace CsgoHoldem.Api.Util
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RegistrationRequest, User>();
        }
    }
}