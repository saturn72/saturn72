    #region

    using Calculator.Common.Domain.Calculations;
    using Saturn72.Common.Data.Repositories;

    #endregion

    namespace Calculator.DB.Model.Repositories
    {
        public class ExpressionRepository : RepositoryBase<ExpressionDomainModel, long, Expression>
        {
            public ExpressionRepository(IUnitOfWork<long> unitOfWork) : base(unitOfWork)
            {
            }
        }
    }