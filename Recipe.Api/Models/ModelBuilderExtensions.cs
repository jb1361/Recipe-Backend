using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recipe.Api.Models.Abstract;

namespace Recipe.Api.Models
{
    public static class ModelBuilderExtensions
    {

        public static void HandleInterfaces(this ModelBuilder builder)
        {
            var entityTypes = builder.Model.GetEntityTypes();
            
            foreach (var entityType in entityTypes)
            {
                if (typeof(ITrackable).IsAssignableFrom(entityType.ClrType))
                {
                    builder.Entity(entityType.ClrType).Property<DateTime>(nameof(ITrackable.UpdatedAt))
                        .HasDefaultValueSql("UTC_TIMESTAMP()")
                        .IsRequired();
                }
                
                if (typeof(IArchivable).IsAssignableFrom(entityType.ClrType))
                {
                    builder.Entity(entityType.ClrType).Property<DateTime>(nameof(IArchivable.ArchivedAt))
                        .IsRequired(false);
                }
                
                if (typeof(ICreatedAtTrackable).IsAssignableFrom(entityType.ClrType))
                {
                    builder.Entity(entityType.ClrType).Property<DateTime>(nameof(ICreatedAtTrackable.CreatedAt))
                        .HasDefaultValueSql("UTC_TIMESTAMP()")
                        .IsRequired();
                }
            }
        }
   
        public static void Unique<TEntity>(this ModelBuilder builder, Expression<Func<TEntity, object>> indexExpression) where TEntity: class
        {
            builder.Entity<TEntity>().HasIndex(indexExpression).IsUnique();
        }
        public static void UniqueIndex<TEntity>(this EntityTypeBuilder<TEntity> builder, Expression<Func<TEntity, object>> indexExpression) where TEntity: class
        {
            builder.HasIndex(indexExpression).IsUnique();
        }
        
        public static IndexBuilder Index<TEntity>(this ModelBuilder builder, Expression<Func<TEntity, object>> indexExpression) where TEntity: class
        {
            return builder.Entity<TEntity>().HasIndex(indexExpression);
        }

        public static PropertyBuilder<TProperty> Property<TEntity, TProperty>(this ModelBuilder builder, Expression<Func<TEntity, TProperty>> propertyExpression) where TEntity: class
        {
            return builder.Entity<TEntity>().Property(propertyExpression);
        }
    }
}