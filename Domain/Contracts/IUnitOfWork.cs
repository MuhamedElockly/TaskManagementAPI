using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts
{
	public interface IUnitOfWork
	{
		public IGenaricRepository<T, TK> GetRepository<T, TK>() where T : BaseEntity<TK>;
		public Task<int> SaveChangesAsync();
	}
}
