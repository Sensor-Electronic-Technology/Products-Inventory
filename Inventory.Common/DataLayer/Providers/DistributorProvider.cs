using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Inventory.Common.BuisnessLayer;
using Inventory.Common.EntityLayer.Model;
using Inventory.Common.EntityLayer.Model.Entities;

namespace Inventory.Common.DataLayer.Providers {
    public class DistributorProvider : IEntityDataProvider<Distributor> {
        public Distributor GetEntity(string entityName) => throw new NotImplementedException();
        public Distributor GetEntity(Expression<Func<Distributor, bool>> expression) => throw new NotImplementedException();
        public IEnumerable<Distributor> GetEntityList(Expression<Func<Distributor, bool>> expression = null, Func<IQueryable<Distributor>, IOrderedQueryable<Distributor>> orderBy = null) => throw new NotImplementedException();
        public Task<IEnumerable<Distributor>> GetEntityListAsync(Expression<Func<Distributor, bool>> expression = null, Func<IQueryable<Distributor>, IOrderedQueryable<Distributor>> orderBy = null) => throw new NotImplementedException();
        public void LoadData() => throw new NotImplementedException();
        public Task LoadDataAsync() => throw new NotImplementedException();
    }
}
