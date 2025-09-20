using Domain.Contracts.SpecificationContracts;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Presistence
{
	public static class SpeceficationEvaluation
	{
		public static IQueryable<T> ApplySpecefication<T, TK>( this IQueryable<T> queryable, ISpecification<T, TK> specification) where T : BaseEntity<TK>
		{

			if (specification.Criteria != null)
			{
				queryable = queryable.Where(specification.Criteria);
			}
			if (specification.OrderBy != null)
			{
				queryable = queryable.OrderBy(specification.OrderBy);
			}
			else if (specification.OrderByDescending != null)
			{
				queryable = queryable.OrderByDescending(specification.OrderByDescending);
			}
			if (specification.IsPagingEnabled != null)
			{
				queryable = queryable.Skip(specification.Skip).Take(specification.Take ?? 10);
			}
			foreach (Expression<Func<T, object>> include in specification.Includes)
			{
				queryable = queryable.Include(include);
			}
			return queryable;
		}
	}
}
