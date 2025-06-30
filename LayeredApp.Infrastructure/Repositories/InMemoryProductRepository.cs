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

        // ===========================================
        // DDD PRINCIPLE: REPOSITORY INITIALIZATION
        // ===========================================
        // The repository can initialize with sample data
        // This is an infrastructure concern, not a domain concern
        public InMemoryProductRepository()
        {
            // Seed some sample data using Money value object
            SeedData();
        }


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
            // ===========================================
            // DDD PRINCIPLE: SEARCH IMPLEMENTATION
            // ===========================================
            // Implement a domain-specific search method
            // The search logic is designed around business needs
            var searchResults = _products.Where(p =>
                p.IsActive &&
                p.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(searchResults);
        }

        public Task<Product> UpdateAsync(Product product)
        {
            // ===========================================
            // DDD PRINCIPLE: AGGREGATE UPDATE
            // ===========================================
            // Find the existing aggregate root
            var existingProduct = _products.FirstOrDefault(p => p.Id == product.Id);
            if (existingProduct == null)
                throw new ArgumentException($"Product with ID {product.Id} not found");

            // ===========================================
            // DDD PRINCIPLE: AGGREGATE REPLACEMENT
            // ===========================================
            // In a real implementation, EF Core would handle the update
            // For this demo, we'll just return the product as-is
            // The domain entity contains all the updated state
            return Task.FromResult(product);
        }
        // ===========================================
        // DDD PRINCIPLE: INFRASTRUCTURE HELPER METHOD
        // ===========================================
        // This method is an infrastructure concern
        // It's used to initialize the repository with sample data
        // The domain layer doesn't know or care about this method
        private void SeedData()
        {
            // ===========================================
            // DDD PRINCIPLE: AGGREGATE CREATION WITH VALUE OBJECT
            // ===========================================
            // Create aggregate roots using their constructors and Money value object
            var sampleProducts = new[]
            {
                new Product("Laptop", "High-performance laptop for professionals", new Money(1299.99m, "USD"), 15),
                new Product("Mouse", "Wireless optical mouse", new Money(29.99m, "USD"), 50),
                new Product("Keyboard", "Mechanical gaming keyboard", new Money(89.99m, "USD"), 25),
                new Product("Monitor", "27-inch 4K monitor", new Money(399.99m, "USD"), 8),
                new Product("Headphones", "Noise-cancelling wireless headphones", new Money(199.99m, "USD"), 12),
                new Product("Webcam", "HD webcam for video conferencing", new Money(79.99m, "USD"), 30),
                new Product("USB Drive", "64GB USB 3.0 flash drive", new Money(19.99m, "USD"), 100),
                new Product("Printer", "All-in-one wireless printer", new Money(149.99m, "USD"), 5)
            };

            foreach (var product in sampleProducts)
            {
                _products.Add(product);
            }
        }

    }
}
