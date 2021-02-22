using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Recipe.Api.Models.Abstract;
using Recipe.Api.Models.Context;

namespace Recipe.Api.Util
{
    public static class ContextUtil
    {
        public static void UpdateTimeStamps(ChangeTracker changeTracker)
        {
            // Credit to:
            // https://www.meziantou.net/entity-framework-core-generate-tracking-columns.htm
            var entries = changeTracker.Entries();
            foreach (var entry in entries)
            {
                if (entry.Entity is ICreatedAtTrackable trackable)
                {
                    DateTime now = DateTime.UtcNow;
                    if ((entry.State == EntityState.Added || entry.State == EntityState.Modified) && entry.Entity is ITrackable updated) {
                       updated.UpdatedAt = now;
                    }
                    if (entry.State == EntityState.Added) {
                        trackable.CreatedAt = now;
                    }
                }
            }
        }
        
        public static void UpdateArchived(ChangeTracker changeTracker)
        {
            // Credit to:
            // https://www.meziantou.net/entity-framework-core-generate-tracking-columns.htm
            var entries = changeTracker.Entries();
            foreach (var entry in entries)
            {
                var archivedAt = entry.Entity.GetType().GetProperty("ArchivedAt");
                var name = entry.Entity.GetType().GetProperty("Name");
                if (archivedAt != null)
                {
                    DateTime now = DateTime.UtcNow;
                    if (entry.State == EntityState.Deleted) {
                        entry.State = EntityState.Modified;
                        archivedAt.SetValue(entry.Entity, now);
                        name?.SetValue(entry.Entity, $"{name.GetValue(entry.Entity)}-Deleted-{now}");
                    }
                }
            }
        }

        public static List<string> GetTotals(this ChangeTracker tracker)
        {
            return tracker.Entries()
                .Select(e => e.Entity)
                .GroupBy(a => a.GetType().Name)
                .Select(g => $"{g.Key} + {g.Count()}")
                .ToList();
        }

        public static List<string> GetStrings(this DefaultContext context, string query)
        {
            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                command.CommandType = CommandType.Text;

                context.Database.OpenConnection();
                var entities = new List<string>();
                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        entities.Add(result.GetString(0));
                    }
                }
                return entities;
            }
        }
    }
}