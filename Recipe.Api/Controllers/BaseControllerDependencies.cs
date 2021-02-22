using AutoMapper;
using Recipe.Api.Config;
using Recipe.Api.Models.Context;

namespace Recipe.Api.Controllers
{
    public class BaseControllerDependencies
    {
        public  DefaultContext DatabaseContext { get; }
         
        public IMapper Mapper { get; }
        
        public AppSettings AppSettings { get; set; }

        public BaseControllerDependencies(DefaultContext defaultContext, IMapper mapper, AppSettings appSettings)
        {
            this.DatabaseContext = defaultContext;
            this.Mapper = mapper;
            this.AppSettings = appSettings;
        }
    }
}