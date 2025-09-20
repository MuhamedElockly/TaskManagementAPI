using Domain.Contracts;
using Domain.Contracts.SpecificationContracts;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Presistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presistence.Repositories
{
	public class GenericRepository<T, TK> : IGenaricRepository<T, TK> where T : BaseEntity<TK>
	{
		readonly ApplicationDbContext _context;
		public GenericRepository(ApplicationDbContext dbContext)
		{
			_context = dbContext;
		}
		public async Task AddAsync(T entity)
		{
			await _context.Set<T>().AddAsync(entity);
		}

		public async Task<int> CountAsync(ISpecification<T, TK> specification)
		{
			IQueryable<T> query = _context.Set<T>().AsQueryable();
			query = query.ApplySpecefication<T, TK>(specification);
			return await query.CountAsync();
		}

		public async Task DeleteAsync(TK id)
		{
			T enity = await _context.Set<T>().FirstOrDefaultAsync();
			if (enity != null)
			{
				_context.Set<T>().Remove(enity);
			}
		}

		public async Task<IEnumerable<T>> GetAllAsync()
		{
			return await _context.Set<T>().ToListAsync();
		}

		public async Task<IEnumerable<T>> GetAllAsynce(ISpecification<T, TK> specification)
		{
			IQueryable<T> query = _context.Set<T>().AsQueryable();
			query = query.ApplySpecefication(specification);
			return await query.ToListAsync();
		}

		public async Task<T> GetByIdAsync(ISpecification<T, TK> specification)
		{
			IQueryable<T> query = _context.Set<T>().AsQueryable();
			query = query.ApplySpecefication(specification);
			return await query.FirstOrDefaultAsync();
		}

		public async Task<T> GetFirstOrDefaultAsync(ISpecification<T, TK> specification)
		{
			IQueryable<T> query = _context.Set<T>().AsQueryable();
			query = query.ApplySpecefication(specification);
			return await query.FirstOrDefaultAsync();
		}

		public void Update(T entity)
		{
			_context.Set<T>().Update(entity);
		}

		
	}
}
