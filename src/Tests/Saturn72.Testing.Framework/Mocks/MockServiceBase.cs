#region

using System.Collections.Generic;
using Saturn72.Core.Domain;

#endregion

namespace Saturn72.Testing.Framework.Mocks
{
    public abstract class MockServiceBase<TDomainModel> where TDomainModel : DomainModelBase<long>
    {
        private ICollection<TDomainModel> _items;

        public ICollection<TDomainModel> Items
        {
            get { return _items ?? (_items = new List<TDomainModel>()); }
            set { _items = value; }
        }

        public static long InsertionIndex { get; set; }

        protected void Create(TDomainModel model)
        {
            model.Id = ++InsertionIndex;
            Items.Add(model);
        }
    }
}