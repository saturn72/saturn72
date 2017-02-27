    #region

    using Calculator.Common.Domain.Calculations;
    using Saturn72.Common.Data;

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