    #region

    using Calculator.Common.Domain.Calculations;
    using Saturn72.Common.Data;
    using Saturn72.Common.Data.Repositories;

    #endregion

    namespace Calculator.DB.Model.Repositories
    {
        public class ExpressionRepository : RepositoryBase<ExpressionModel, Expression>
        {
            public ExpressionRepository(IUnitOfWork<ExpressionModel> unitOfWork) : base(unitOfWork)
            {
            }
        }
    }