namespace LayeredApp.Application.DTOs
{
    // ===========================================
    // APPLICATION LAYER - DATA TRANSFER OBJECTS (DTOs)
    // ===========================================
    // DTOs are used to transfer data between layers
    // They are simple data containers without business logic
    // They help decouple the presentation layer from the domain model

    // DDD PRINCIPLE: VALUE OBJECT DTO
    // This DTO represents the Money value object in the domain
    public class MoneyDto
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
    }

    // DTO for creating a new product
    public class CreateProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public MoneyDto Price { get; set; } = new MoneyDto();
        public int StockQuantity { get; set; }
    }

    // DTO for updating an existing product
    public class UpdateProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public MoneyDto Price { get; set; } = new MoneyDto();
    }

    // DTO for updating product stock
    public class UpdateStockDto
    {
        public int Quantity { get; set; }
    }

    // DTO for product responses
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public MoneyDto Price { get; set; } = new MoneyDto();
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsInStock { get; set; }
        public bool IsLowStock { get; set; }
    }

    // DTO for product list responses
    public class ProductListDto
    {
        public IEnumerable<ProductDto> Products { get; set; } = Enumerable.Empty<ProductDto>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}