#region

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Reflection;
using Saturn72.Core;

#endregion

namespace Saturn72.Module.EntityFramework.ContextManagement
{
    public class DbContextCommander<TDbContext> : IDbContextCommander<TDbContext> where TDbContext:DbContext, new()
    {
        public virtual int SaveChangesToContext(TDbContext ctx)
        {
            try
            {
                return ctx.SaveChanges();
            }

            catch (Exception ex)
            {
                DefaultOutput.WriteLine(ex.Message);
                DefaultOutput.WriteLine(ex);
                throw;
            }
        }

        public virtual TResult QueryNewContext<TResult>(Func<TDbContext, TResult> query)
        {
            using (var db = new TDbContext())
            {
                return query(db);
            }
        }

        public void QueryNewContext(Action<TDbContext> query)
        {

            using (var db = new TDbContext())
            {
                query(db);
            }
        }

        public virtual IEnumerable<string> GetUnchangedProperties<TEntity>(TEntity source, TEntity destination)
        {
            if (source == null || destination == null)
                throw new ArgumentException("On or more instance are null");

            var allPropertyInfos = typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var result = new List<string>();

            foreach (var pInfo in allPropertyInfos)
            {
                if (result.Contains(pInfo.Name))
                    continue;
                var srcValue = pInfo.GetValue(source, null);
                var destValue = pInfo.GetValue(destination, null);

                if (srcValue == destValue)
                    result.Add(pInfo.Name);
            }
            return result;
        }
    }
}