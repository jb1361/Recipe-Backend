using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CsgoHoldem.Api.Models.Abstract;

namespace CsgoHoldem.Api.Util
{
    public interface IUniqueFilter
    {
        IQueryable<IPrimaryKeyModel> FilterUnique(IQueryable<IPrimaryKeyModel> query);
    }
    public class UniqueInDBAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (IPrimaryKeyModel)validationContext.ObjectInstance;
            Func<IQueryable<IPrimaryKeyModel>, IQueryable<IPrimaryKeyModel>> filter = null;
            if (model is IUniqueFilter filterable) {
                filter = filterable.FilterUnique;
            }
            var propertyName = validationContext.MemberName;
            var propertyValue = model.GetType().GetProperty(propertyName).GetValue(model);
            
            return validationContext.ValidateUnique(propertyName, propertyValue, model.Id,
                propertyName + " already exists", filter, model.GetType());
        }
    }
}