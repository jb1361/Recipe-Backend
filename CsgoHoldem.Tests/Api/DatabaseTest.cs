using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NUnit.Framework;
using CsgoHoldem.Api.Models.Context;
using CsgoHoldem.Api.Services;

namespace CsgoHoldem.Tests.Api
{
    public abstract class DatabaseTest
    {
        protected DefaultContext context;
        protected DbContextOptions<DefaultContext> dbOptions;
        private IDbContextTransaction _transaction;
        
        [SetUp]
        public void Setup()
        {
            var optionsBuilder = new DbContextOptionsBuilder<DefaultContext>();
            optionsBuilder.ConfigureMysql("Server=127.0.0.1;Database=RecipeBackendTesting;User=root;Password=;", false);
            dbOptions = optionsBuilder.Options;
            context = new DefaultContext(dbOptions);
            context.Database.Migrate();
            _transaction = context.Database.BeginTransaction();
        }

        [TearDown]
        public void TearDown()
        {
            _transaction.Rollback();
            context.DetachAllEntities();
        }
    }
}