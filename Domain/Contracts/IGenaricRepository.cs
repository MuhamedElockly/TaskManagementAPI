using Domain.Contracts.SpecificationContracts;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts
{
	public interface IGenaricRepository<T, TK> where T : BaseEntity<TK>
	{
		public Task<IEnumerable<T>> GetAllAsync();
		public Task<IEnumerable<T>> GetAllAsynce(ISpecification<T, TK> specification);
		public Task<T> GetByIdAsync(ISpecification<T, TK> specification);
		public Task AddAsync(T entity);
		public void Update(T entity);
		public Task DeleteAsync(TK id);
		public Task<int> CountAsync(ISpecification<T, TK> specification);
		public Task<T> GetFirstOrDefaultAsync(ISpecification<T, TK> specification);
	}
}
