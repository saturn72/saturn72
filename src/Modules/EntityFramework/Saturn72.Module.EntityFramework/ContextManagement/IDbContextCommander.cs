using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace Saturn72.Module.EntityFramework.ContextManagement
{
    public interface IDbContextCommander<TDbContext> where TDbContext : DbContext
    {
        int SaveChangesToContext(TDbContext ctx);
        TResult QueryNewContext<TResult>(Func<TDbContext, TResult> query);
        void CommandNewContext(Action<TDbContext> command);
        IEnumerable<string> GetUnchangedProperties<TEntity>(TEntity source, TEntity destination);
    }
}