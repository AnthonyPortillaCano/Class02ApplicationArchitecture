using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayeredApp.Domain
{
    public interface IProductRepository
    {
        //DDD PRINCIPLE: QUERY METHODS

        //These methods represent domain queries
        //They are designed around business need ,not technical implementation

        // DDD PRINCIPLE :AGGREGATE ROOT ACCESS
        // GetByIdAsync returns the aggreate root(Product)
        // This ensures that business rules are enforced when accessing the entity
        Task<Product> GetByIdAsync(int id);

        // DDD PRINCIPLE: COLLECTION-LIKE INTERFACE

        //GetAllAsync provides a collection-like interface
        //This makes the repository feel like working with an in-memory collection
        Task<IEnumerable<Product>> GetAllAsync();



        // DDD PRINCIPLE: DOMAIN SPECIFIC QUERIES
        // These queries are designed around business concepts
        // They represent specific business needs,not generic crud operations
        Task<IEnumerable<Product>> GetActiveProductsAsync();
        Task<IEnumerable<Product>> GetLowStockProductAsync(int threshold=10);
        Task<IEnumerable<Product>> SearchByNameAsync(string name);


        //DDD PRINCIPLE :AGREGATE PERSISTENCE
        //AddAsync and UpdateAsync work with the aggregate root
        //This ensures that the entire aggregate is saved consistently
        Task<Product> AddAsync(Product product);
        Task<Product> UpdateAsync(Product product);

        //DDD PRINCIPLE: AGGREAGATE REMOVAL
        //DeleteAsync removes the aggregate by Id
        //This ensures that entire aggregate is removed consistently
        Task DeleteAsync(int id);

        //DDD PRINCIPLE: DOMAIN VALIDATION SUPPORT

        // ExistsAsync supports domain validation
        //This allows the domain to check if an aggregate exists before operations
        Task<bool> ExistsAsync(int id);

    }
}
