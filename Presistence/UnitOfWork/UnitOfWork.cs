using Domain.Contracts;
using Domain.Entities;
using Presistence.Data;
using Presistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presistence.UnitOfWork
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly Dictionary<string, object> _repositories = new Dictionary<string, object>();
		private readonly ApplicationDbContext _context;
		public UnitOfWork(ApplicationDbContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}
		public IGenaricRepository<T, TK> GetRepository<T, TK>() where T : BaseEntity<TK>
		{
			if (_repositories.ContainsKey(typeof(T).Name))
			{
				return (IGenaricRepository<T, TK>)_repositories[typeof(T).Name];
			}
			else
			{
				IGenaricRepository<T, TK> repository = new GenericRepository<T, TK>(_context);
				_repositories.Add(typeof(T).Name, repository);
				return repository;
			}
		}

		public async Task<int> SaveChangesAsync()
		{
			return await _context.SaveChangesAsync();
		}
	}
}
