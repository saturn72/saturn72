#region

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using Saturn72.Core;

#endregion

namespace Saturn72.Module.EntityFramework.ContextManagement
{
    public class DbContextCommander<TDbContext> : IDbContextCommander<TDbContext> where TDbContext : DbContext, new()
    {
        public virtual int SaveChangesToContext(TDbContext ctx)
        {
            try
            {
                return ctx.SaveChanges();
            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message, ex);
                throw ex;
            }
        }

        public virtual TResult QueryNewContext<TResult>(Func<TDbContext, TResult> query)
        {
            using (var db = new TDbContext())
            {
                return query(db);
            }
        }

        public int CommandNewContext(Action<TDbContext> command)
        {
            using (var db = new TDbContext())
            {
                command(db);
                return db.SaveChanges();
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