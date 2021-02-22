using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Recipe.Api.Models.Abstract;
using Recipe.Api.Models.DefaultContextModels;
using Recipe.Api.Util;


namespace Recipe.Api.Models.Context {
    public class DefaultContext : DbContext {
        public DefaultContext(DbContextOptions<DefaultContext> options) : base(options) { }
        
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Instruction> Instructions { get; set; }
        public DbSet<DefaultContextModels.Recipe> Recipes { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            User.OnModelCreating(modelBuilder);
            Role.OnModelCreating(modelBuilder);
            DefaultContextModels.Recipe.OnModelCreating(modelBuilder);
            modelBuilder.HandleInterfaces();
        }
        
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }
        
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public async Task<TEntity> AddAndSaveAsync<TEntity>(TEntity entity) where TEntity : class
        {
            Add(entity);
            await SaveChangesAsync();
            return entity;
        }
        /*
         *  Sets the entity to modified if it's primary key is not 0.
         */
        public void UpdateEntityIfIdNot0(IPrimaryKeyModel item)
        {
            if (item == null) return;
            Entry(item).State = item.Id == 0 ? EntityState.Added : EntityState.Modified;
        }
        
        /**
         * Sets the entity to modified if not in the new status
         */
        public void UpdateEntity(object item)
        {
            if (item == null) return;
            if (Entry(item).State == EntityState.Added) return;
            Entry(item).State = EntityState.Modified;
        }

        public T TrackAsUnchanged<T>(T item)
        {
            if (item == null) return default;
            Entry(item).State = EntityState.Unchanged;
            return item;
        }
        
        public T TrackAsUnchangedOrReplace<T>(T item) where T : IPrimaryKeyModel
        {
            if (item == null) return default;

            var alreadyExisting = ChangeTracker.Entries()
                .Where(e => e.Entity.GetType() == item.GetType())
                .Where(e => ((IPrimaryKeyModel) e.Entity).Id == item.Id)
                .Select(e => (T)e.Entity)
                .FirstOrDefault();

            if (alreadyExisting != null) {
                return alreadyExisting;
            }
            Entry(item).State = EntityState.Unchanged;
            return item;
        }
        
        public T TrackAsAdded<T>(T item)
        {
            if (item == null) return default;
            Entry(item).State = EntityState.Added;
            return item;
        }
        
        public void DetachEntity(object item)
        {
            if (item == null) return;
            Entry(item).State = EntityState.Detached;
        }
        
        private void OnBeforeSaving()
        {
            ContextUtil.UpdateTimeStamps(ChangeTracker);
            ContextUtil.UpdateArchived(ChangeTracker);
        }
        
        /**
         * Removed all entities from the change tracker so that any additional
         * data pulled from the database is not "auto fix-up" by values in the change tracker.
         * @see https://docs.microsoft.com/en-us/ef/core/querying/related-data and search "fix-up"
         */
        public void DetachAllEntities()
        {
            var changedEntriesCopy = this.ChangeTracker.Entries().ToList();
            // The two lines below are needed to prevent an InvalidOperationException from being thrown.
            // @see https://github.com/dotnet/efcore/issues/18406
            ChangeTracker.CascadeDeleteTiming = CascadeTiming.OnSaveChanges;
            ChangeTracker.DeleteOrphansTiming = CascadeTiming.OnSaveChanges;
            foreach (var entry in changedEntriesCopy)
            {
                entry.State = EntityState.Detached;
            }
        }
    }
}