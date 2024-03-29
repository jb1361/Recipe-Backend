using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Dynamic.Core;
using Recipe.Api.Models.Abstract;
using Recipe.Api.Models.Context;

namespace Recipe.Api.Util
{
    public static class ValidationContextExtensions
    {
        public static DefaultContext GetDatabaseContext(this ValidationContext validationContext)
        {
            return (DefaultContext) validationContext.GetService(typeof(DefaultContext));
        }

        public static ValidationResult ValidateUnique<T>(
            this ValidationContext validationContext, 
            string propertyName,
            object propertyValue, 
            int? id, 
            string errorMessage = null, 
            Func<IQueryable<T>, IQueryable<T>> extraWhere = null, 
            Type typeFilter = null) 
            where T : IPrimaryKeyModel
        {
            if (errorMessage == null)
            {
                errorMessage = propertyName + " already exists";
            }
            var context = validationContext.GetDatabaseContext();
            if (typeFilter == null)
            {
                typeFilter = typeof(T);
            }
            var query = (IQueryable<T>) context.GetType()
                .GetProperties()
                .FirstOrDefault(p =>
                    p.PropertyType.GetGenericArguments().Any(t => t == typeFilter)
                ).GetValue(context);

            if (id != 0)
            {
                query = query.Where("Id != @0", id);
            }
            // dynamic query based on property name, is still immune to sql injection
            query = query.Where(propertyName + " = @0", propertyValue);

            if (extraWhere != null)
            {
                query = extraWhere(query);
            }
            
            if (query.Any())
            {
                return new ValidationResult(errorMessage);
            }

            return ValidationResult.Success;
        }
        
    }
}