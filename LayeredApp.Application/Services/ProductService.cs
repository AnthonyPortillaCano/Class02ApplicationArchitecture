using LayeredApp.Application.DTOs;
using LayeredApp.Domain;

namespace LayeredApp.Application.Services
{
    // ===========================================
    // APPLICATION LAYER - SERVICE IMPLEMENTATION
    // ===========================================
    // This service implements the application use cases
    // It orchestrates the domain objects and coordinates with the repository
    // It handles the conversion between DTOs and domain objects
    //
    // Key responsibilities:
    // - Coordinate domain objects
    // - Handle transactions
    // - Convert between DTOs and domain objects
    // - Implement use cases

    // ===========================================
    // DDD PRINCIPLE: APPLICATION SERVICE
    // ===========================================
    // Application Services in DDD:
    // - Orchestrate domain objects and repositories
    // - Implement use cases (business workflows)
    // - Handle transactions and coordination
    // - Do NOT contain business rules (those belong in Domain)
    // - Act as the facade for the domain layer
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        // ===========================================
        // DDD PRINCIPLE: DEPENDENCY INJECTION
        // ===========================================
        // The service depends on the repository interface (abstraction)
        // This follows the Dependency Inversion Principle (DIP)
        // The concrete implementation is injected from the Infrastructure layer
        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        // ===========================================
        // DDD PRINCIPLE: QUERY OPERATIONS
        // ===========================================
        // These methods implement query use cases
        // They retrieve data and convert domain objects to DTOs
        // No business logic here - just data retrieval and transformation

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            // ===========================================
            // DDD PRINCIPLE: AGGREGATE ROOT ACCESS
            // ===========================================
            // Get the aggregate root from the repository
            // The repository returns the domain entity with all business logic intact
            var product = await _productRepository.GetByIdAsync(id);
            return product != null ? MapToDto(product) : null;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            // ===========================================
            // DDD PRINCIPLE: COLLECTION OPERATION
            // ===========================================
            // Get all aggregate roots and convert to DTOs
            var products = await _productRepository.GetAllAsync();
            return products.Select(MapToDto);
        }

        public async Task<IEnumerable<ProductDto>> GetActiveProductsAsync()
        {
            // ===========================================
            // DDD PRINCIPLE: DOMAIN-SPECIFIC QUERY
            // ===========================================
            // Use a domain-specific query method
            // This query is designed around business concepts, not technical needs
            var products = await _productRepository.GetActiveProductsAsync();
            return products.Select(MapToDto);
        }

        public async Task<IEnumerable<ProductDto>> GetLowStockProductsAsync(int threshold = 10)
        {
            // ===========================================
            // DDD PRINCIPLE: BUSINESS QUERY
            // ===========================================
            // Use a business-specific query method
            // The threshold parameter represents a business rule
            var products = await _productRepository.GetLowStockProductAsync(threshold);
            return products.Select(MapToDto);
        }

        public async Task<IEnumerable<ProductDto>> SearchByNameAsync(string name)
        {
            // ===========================================
            // DDD PRINCIPLE: SEARCH OPERATION
            // ===========================================
            // Use a domain-specific search method
            // This is designed around business needs, not technical implementation
            var products = await _productRepository.SearchByNameAsync(name);
            return products.Select(MapToDto);
        }

        // ===========================================
        // DDD PRINCIPLE: COMMAND OPERATIONS
        // ===========================================
        // These methods implement command use cases
        // They orchestrate domain objects and ensure business rules are followed
        // The actual business logic is in the domain entities

        public async Task<ProductDto> CreateAsync(CreateProductDto createDto)
        {
            // ===========================================
            // DDD PRINCIPLE: AGGREGATE CREATION
            // ===========================================
            // Create the aggregate root using its constructor
            // The constructor enforces business rules (invariants)
            // This ensures the product cannot be created in an invalid state
            var money = MapToMoney(createDto.Price);
            var product = new Product(
                createDto.Name,
                createDto.Description,
                money,
                createDto.StockQuantity
            );

            // ===========================================
            // DDD PRINCIPLE: AGGREGATE PERSISTENCE
            // ===========================================
            // Save the aggregate root to the repository
            // The repository handles the persistence details
            var savedProduct = await _productRepository.AddAsync(product);
            return MapToDto(savedProduct);
        }

        public async Task<ProductDto> UpdateAsync(int id, UpdateProductDto updateDto)
        {
            // ===========================================
            // DDD PRINCIPLE: AGGREGATE ROOT RETRIEVAL
            // ===========================================
            // Get the aggregate root first
            // This ensures we're working with a valid domain entity
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                throw new ArgumentException($"Product with ID {id} not found");

            // ===========================================
            // DDD PRINCIPLE: DOMAIN METHOD INVOCATION
            // ===========================================
            // Call domain methods to update the aggregate
            // The domain methods enforce business rules
            // The application service doesn't contain business logic
            var money = MapToMoney(updateDto.Price);
            product.UpdateDetails(updateDto.Name, updateDto.Description, money);

            // ===========================================
            // DDD PRINCIPLE: AGGREGATE PERSISTENCE
            // ===========================================
            // Save the updated aggregate root
            var updatedProduct = await _productRepository.UpdateAsync(product);
            return MapToDto(updatedProduct);
        }

        public async Task<ProductDto> UpdateStockAsync(int id, UpdateStockDto stockDto)
        {
            // ===========================================
            // DDD PRINCIPLE: AGGREGATE ROOT RETRIEVAL
            // ===========================================
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                throw new ArgumentException($"Product with ID {id} not found");

            // ===========================================
            // DDD PRINCIPLE: DOMAIN METHOD INVOCATION
            // ===========================================
            // Call domain method to update stock
            // The domain method contains the business logic for stock updates
            // It will throw exceptions if business rules are violated
            product.UpdateStock(stockDto.Quantity);

            // ===========================================
            // DDD PRINCIPLE: AGGREGATE PERSISTENCE
            // ===========================================
            var updatedProduct = await _productRepository.UpdateAsync(product);
            return MapToDto(updatedProduct);
        }

        public async Task<ProductDto> ActivateAsync(int id)
        {
            // ===========================================
            // DDD PRINCIPLE: AGGREGATE ROOT RETRIEVAL
            // ===========================================
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                throw new ArgumentException($"Product with ID {id} not found");

            // ===========================================
            // DDD PRINCIPLE: DOMAIN BEHAVIOR INVOCATION
            // ===========================================
            // Call domain method to activate the product
            // The domain method contains the business logic
            product.Activate();

            var updatedProduct = await _productRepository.UpdateAsync(product);
            return MapToDto(updatedProduct);
        }

        public async Task<ProductDto> DeactivateAsync(int id)
        {
            // ===========================================
            // DDD PRINCIPLE: AGGREGATE ROOT RETRIEVAL
            // ===========================================
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                throw new ArgumentException($"Product with ID {id} not found");

            // ===========================================
            // DDD PRINCIPLE: DOMAIN BEHAVIOR INVOCATION
            // ===========================================
            // Call domain method to deactivate the product
            product.Deactivate();

            var updatedProduct = await _productRepository.UpdateAsync(product);
            return MapToDto(updatedProduct);
        }

        public async Task DeleteAsync(int id)
        {
            // ===========================================
            // DDD PRINCIPLE: AGGREGATE VALIDATION
            // ===========================================
            // Check if the aggregate exists before attempting to delete
            if (!await _productRepository.ExistsAsync(id))
                throw new ArgumentException($"Product with ID {id} not found");

            // ===========================================
            // DDD PRINCIPLE: AGGREGATE REMOVAL
            // ===========================================
            // Remove the aggregate root from the repository
            await _productRepository.DeleteAsync(id);
        }

        // ===========================================
        // DDD PRINCIPLE: DOMAIN-TO-DTO MAPPING
        // ===========================================
        // This method converts domain objects to DTOs
        // It's part of the application layer's responsibility
        // The mapping ensures that domain logic is preserved in the DTO
        private static ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = MapToDto(product.Price),
                StockQuantity = product.StockQuantity,
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt,
                // ===========================================
                // DDD PRINCIPLE: DOMAIN LOGIC PRESERVATION
                // ===========================================
                // Call domain methods to get computed values
                // This ensures that business logic is preserved in the DTO
                IsInStock = product.IsInStock(),
                IsLowStock = product.IsLowStock()
            };
        }

        // Mapping from Money to MoneyDto
        private static MoneyDto MapToDto(Money money)
        {
            return new MoneyDto
            {
                Amount = money.Amount,
                Currency = money.Currency
            };
        }

        // Mapping from MoneyDto to Money
        private static Money MapToMoney(MoneyDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));
            return new Money(dto.Amount, dto.Currency);
        }
    }
}