using AzureAssignment.Models;
using AzureAssignment.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AzureAssignment.Repository.Implementation
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly AppDbContext _context;
        private DbSet<TEntity> entities;

        public Repository(AppDbContext context)
        {
            _context = context;
            entities = context.Set<TEntity>();
        }
        public async Task Delete(TEntity entity)
        {
            entities.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<TEntity> Get(int id)
        {
            return await entities.AsNoTracking().SingleOrDefaultAsync(s => s.Id == id);
        }

        public Task<List<TEntity>> GetAll()
        {
            return entities.ToListAsync();
        }

        public async Task Insert(TEntity entity)
        {
            await entities.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Update(TEntity entity)
        {
            entities.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
