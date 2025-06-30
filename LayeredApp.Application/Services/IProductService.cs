using LayeredApp.Application.DTOs;

namespace LayeredApp.Application.Services
{
    // ===========================================
    // APPLICATION LAYER - SERVICE INTERFACE
    // ===========================================
    // This interface defines the application services
    // Application services orchestrate the use cases
    // They coordinate between the domain and infrastructure layers
    //
    // The Application layer contains:
    // - Use cases and business workflows
    // - Coordination between domain objects
    // - Transaction management
    // - No business rules (those belong in Domain)

    public interface IProductService
    {
        // Query operations
        Task<ProductDto?> GetByIdAsync(int id);
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<IEnumerable<ProductDto>> GetActiveProductsAsync();
        Task<IEnumerable<ProductDto>> GetLowStockProductsAsync(int threshold = 10);
        Task<IEnumerable<ProductDto>> SearchByNameAsync(string name);

        // Command operations
        Task<ProductDto> CreateAsync(CreateProductDto createDto);
        Task<ProductDto> UpdateAsync(int id, UpdateProductDto updateDto);
        Task<ProductDto> UpdateStockAsync(int id, UpdateStockDto stockDto);
        Task<ProductDto> ActivateAsync(int id);
        Task<ProductDto> DeactivateAsync(int id);
        Task DeleteAsync(int id);
    }
}