using Domain.Contracts.SpecificationContracts;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specefication_Implementation
{
    public class Specification<T, TK> : ISpecification<T, TK> where T : BaseEntity<TK>
    {
        public Specification(Expression<Func<T, bool>> _criteria)
        {
            Criteria = _criteria;
        }
        #region Include
        public Expression<Func<T, bool>> Criteria { get; private set; }

        public List<Expression<Func<T, object>>> Includes { get; private set; } = [];

        public void AddInclude(Expression<Func<T, object>> _includeExpression)
        {
            Includes.Add(_includeExpression);
        }
        #endregion
        #region Ordering
        public Expression<Func<T, object>> OrderBy { get; private set; }

        public Expression<Func<T, object>> OrderByDescending { get; private set; }
        public void SetOrderBy(Expression<Func<T, object>> _orderByExpression)
        {
            OrderBy = _orderByExpression;
        }
        public void SetOrderByDescending(Expression<Func<T, object>> _orderByDescendingExpression)
        {
            OrderByDescending = _orderByDescendingExpression;
        }
        #endregion
        #region Pagination
        public bool IsPagingEnabled { get; private set; }
        public int? Take { get; private set; }

        public int Skip { get; private set; }
        public void ApplyPaging(int _skip, int _take)
        {
            IsPagingEnabled = true;
            Skip = _skip;
            Take = _take;
        }
        #endregion
    }
}
