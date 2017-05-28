using System;
using System.Collections.Generic;
using MongoDB.Driver;
using Saturn72.Core.Configuration;
using Saturn72.Core.Domain;
using Saturn72.Core.Services.Impl.Data;
using Saturn72.Extensions;
using Saturn72.Modules.MongoDb.Config;

namespace Saturn72.Modules.MongoDb
{
    public class MongoDbUnitOfWork<TDomainModel> : IUnitOfWork<TDomainModel>
        where TDomainModel : DomainModelBase
    {
        protected static IMongoClient MongoClient;
        protected static IMongoDatabase MongoDatabase;

        static MongoDbUnitOfWork()
        {
            var c = ConfigManager.GetConfigMap<MongoDbConfigMap>().Config;

            var connectionString = c.ConnectionString;
            MongoClient = connectionString.HasValue()
                ? new MongoClient(connectionString)
                : new MongoClient();

            MongoDatabase = MongoClient.GetDatabase(c.DatabaseName);
        }

        public IEnumerable<TDomainModel> GetAll()
        {
            throw new NotImplementedException();
        }

        public TDomainModel GetById(long id)
        {
            throw new NotImplementedException();
        }

        public TDomainModel Replace(TDomainModel model)
        {
            throw new NotImplementedException();
        }

        public TDomainModel Update(TDomainModel model)
        {
            throw new NotImplementedException();
        }

        public object Create(TDomainModel model)
        {
            throw new NotImplementedException();
        }

        public int Delete(long id)
        {
            throw new NotImplementedException();
        }

        public int Delete(IEnumerable<long> ids)
        {
            throw new NotImplementedException();
        }
    }
}