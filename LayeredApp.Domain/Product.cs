using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayeredApp.Domain
{

    // DDD PRINCIPLE: ENCAPSULATION
    //===================================
    // Properties are private set to ensure that:
    //-state can only be changed through domain methods
    //-business rules are enforced during state changes
    //-External code can not bypass domain logic.
    public class Product
    {
        public int Id { get; private set; }
        public string Name { get; private set; }

        public string Description { get; private set; }

        public Money Price { get; private set; }
        public int StockQuantity { get; private set; }

        public bool IsActive { get; private set; }
        public DateTime CreateAt { get; private set; }

        public DateTime UpdateAt { get; private set; }

        //DDD PRINCIPLE: PRIVATE CONSTRUCTOR FOR EF CORE
        // This allows Entity Framework to create instances
        // while still enforcing business rules through public constructor

        private Product() { }

         //DDD Principles:Constructor with business rules
         //The constructor enforces business rules(invariants)
         //This ensures that a Product cannot be created in an invalid state
         public Product(string name,string description,decimal price,int stockQuantity,string currency="USD")
        {
            //DDD PRINCIPLE:INVARIANT VALIDATION
            // Business rules are enforced at the domain level
            // These are the "invariants" that must always be true

            if (string.IsNullOrEmpty(name)) throw new ArgumentException("Product name cannot be empty", nameof(name));
            if(string.IsNullOrEmpty(description)) throw new ArgumentException("Product description cannot be empty", nameof(description));
            if (price<=0) throw new ArgumentException("Product price must be greater than zero", nameof(description));
            if(stockQuantity<0)
                throw new ArgumentException("Stock quantity cannot be negative",nameof(stockQuantity));

            //DDD PRINCIPLE: STATE INITIALIZATION
            //Set properties with proper domain logic
            Name = name.Trim();
            Description = description.Trim();
            Price=new Money(price,currency);
            StockQuantity = stockQuantity;
            IsActive = true;
            CreateAt = DateTime.UtcNow;
        }

        //Overload for using Money directly
        public Product(string name,string description,Money price,int stockQuantity)
        {
            if(string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name cannot be empty", nameof(name));
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Product description cannot be empty", nameof(description));
            if (price is null)
                throw new ArgumentNullException(nameof(price));
            if (price.Amount <= 0)
                throw new ArgumentException("Product price must be greater than zero", nameof(price));
            if (stockQuantity < 0)
                throw new ArgumentException("Stock quantity cannot be negative", nameof(stockQuantity));

            Name = name.Trim();
            Description = description.Trim();
            Price=price; 
            StockQuantity = stockQuantity;
            IsActive = true;
            CreateAt = DateTime.UtcNow;

        }

        //DDD PRINCIPLE: DOMAIN METHODS(BEHAVIOR)
         //These methods conaint business logic and ensure
         //that state changes follow business rules

        public void UpdateDetails(string name,string description,decimal price,string currency="USD")
        {
            //DDD PRINCIPLE:BUSINESS RULE ENFORCEMENT
            //Same business rules as constructor to maintain invariants
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name cannot be empty", nameof(name));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Product description cannot be empty", nameof(description));

            if (price <= 0)
                throw new ArgumentException("Product price must be greater than zero", nameof(price));

            //DDD PRINCIPLE:CONTROLLED STATE CHANGE
            //State is only changed through domain methods
            //This ensures business rules are always followed
            Name = name.Trim();
            Description = description.Trim();
            Price=new Money(price, currency);
            UpdateAt= DateTime.UtcNow;
        }
        public void UpdateDetails(string name,string description,Money price)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name cannot be empty", nameof(name));
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Product description cannot be empty", nameof(description));
            if (price is null)
                throw new ArgumentNullException(nameof(price));
            if (price.Amount <= 0)
                throw new ArgumentException("Product price must be greater than zero", nameof(price));
            Name=name.Trim();
            Description=description.Trim();
            Price = price;
            UpdateAt= DateTime.UtcNow;
        }
        public void UpdateStock(int newQuantity)
        {
            //DDD PRINCIPLE: DOMAIN VALIDATION
            // Business rule: Stock cannot be negative
            if(newQuantity < 0) throw new ArgumentException("Stock quantity cannot be negative",nameof(newQuantity));
             StockQuantity=newQuantity;
             UpdateAt= DateTime.UtcNow;
        }
        public void DecreaseStock(int quantity)
        {
            //DDD PRINCIPLE:BUSINESS RULE ENFORCEMENT
            //Business rules:
            //1. Quantity to decrease must be positive
            //2. Cannot decrease more than available stock
             if(quantity <= 0) throw new ArgumentException("Quantity to decrease must be positive",nameof(quantity));
            if (StockQuantity < quantity)
                throw new ArgumentException($"Insufficient stock,Available:{StockQuantity},Requested:{quantity}");

            StockQuantity -= quantity;
            UpdateAt= DateTime.UtcNow;
        }
        public void IncreaseStock(int quantity)
        {
            //DDD PRINCIPLE: DOMAIN LOGIC
            // Business rule:quantity to increase must be positive
            if (quantity <= 0)
                throw new ArgumentException("Quantity to increase must be positive", nameof(quantity));
            StockQuantity += quantity;
            UpdateAt= DateTime.UtcNow;
        }
        public void Activate()
        {
            //DDD PRINCIPLE:DOMAIN BEHAVIOR
            //Simple domain method that changes state
            IsActive= true;
            UpdateAt = DateTime.UtcNow; 
        }
        public void Deactivate()
        {
            // DDD PRINCIPLES: DOMAIN BEHAVIOR
            //Simple domaing method that changes state

            IsActive= false;
            UpdateAt = DateTime.UtcNow;
        }


        //DDD PRINCIPLE: DOMAIN QUERIES
        // These methods provide business logic for querying state
        // They encapsulate complex business rules about the entity
        public bool IsInStock()
        {
            //DDD PRINCIPLE: BUSINESS LOGIC ENCAPSULATION
            //Business rule: Product is in stock if active and has quantity>0
            return IsActive && StockQuantity > 0;
        }

        public bool IsLowStock(int threshold=10)
        {
            //DDD PRINCIPLE: BUSINESS RULE ENCAPSULATION
            //Business rule: product is low stock  if active,has stock, but below threshold
            return IsActive && StockQuantity <= threshold && StockQuantity>0;
        }
        public bool IsOutOfStock()
        {
            //DDD PRINCIPLE:DOMAIN QUERY
            // Simple business query
            return StockQuantity == 0;
        }
    }
}
