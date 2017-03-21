using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;

namespace dg.document.db
{
    public interface IDocumentDbRepository<T> where T : class
    {
        Task<T> GetAsync(string id);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate);
        Task<Document> CreateAsync(T item);
        Task<Document> UpdateAsync(string id, T item);
        Task DeleteAsync(string id);
        void Initialize();
    }
}
