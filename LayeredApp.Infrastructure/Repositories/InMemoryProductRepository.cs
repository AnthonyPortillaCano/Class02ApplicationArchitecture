using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayeredApp.Domain;

namespace LayeredApp.Infrastructure.Repositories
{

    //DDD PRINCIPLES: INFRASTRUCTURE CONCERNS
    // This class handles infraestructure concerns:
    // -Data storages (in-memory list in this case)
    // -Data retrieval and persistence
    // - Technical implementation details
    public class InMemoryProductRepository : IProductRepository
    {

        //DDD PRINCIPLE :DATA STORAGE ABSTRACTION
        // The repository abstracts the data storage mechanism
        // In this case it is an in memory list, but it could be a database
        // The domain and application layers don not know or care about this details
         private readonly List<Product> _products=new();
        private int _nextId = 1;



        //DDD PRINCIPLE : COMMAND METHOD IMPLEMENTATIONS
        // These methods implement the command contracts defined in the domain
        // They handle the technical details of data persistence
        // The business logic remains in the domain entities
        public Task<Product> AddAsync(Product product)
        {
            _products.Add(product);
            return Task.FromResult(product);
        }

        public Task DeleteAsync(int id)
        {

            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                _products.Remove(product);
            }
            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(int id)
        {
            
            var exists=_products.Any(p=>p.Id == id);
            return Task.FromResult(exists);
        }

        public Task<IEnumerable<Product>> GetActiveProductsAsync()
        {
           var activeProducts=_products.Where(p=>p.IsActive);
            return Task.FromResult(activeProducts);
        }

        public Task<IEnumerable<Product>> GetAllAsync()
        {
            //DDD PRINCIPLE: COLLECTION-LIKE INTERFACE
            return Task.FromResult(_products.AsEnumerable());
        }

        public Task<Product> GetByIdAsync(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            return Task.FromResult<Product>(product);
        }

        public Task<IEnumerable<Product>> GetLowStockProductAsync(int threshold = 10)
        {
            //DDD PRINCIPLE: BUSINESS QUERY IMPLEMENTATION
            var lowStockProducts = _products.Where(p => p.IsActive && p.StockQuantity <= threshold);
            return Task.FromResult(lowStockProducts);
        }

        public Task<IEnumerable<Product>> SearchByNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task<Product> UpdateAsync(Product product)
        {
            throw new NotImplementedException();
        }
    }
}
