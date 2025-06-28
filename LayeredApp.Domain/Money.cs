using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayeredApp.Domain
{

    // DDD PRINCIPLE: VALUE OBJECT
    //This class represents a value object for money (price,cost,etc)
    //- No identity(no Id property)
    //- Immutable (no setters,only contructor)
    //- Equality is based on value, not reference
    public sealed class Money : IEquatable<Money>
    {
        public decimal Amount { get; }
        public string Currency { get; }

        public Money(decimal amount,string currency)
        {
            if (amount < 0)
            {
                throw new ArgumentException("Amount cannot be negative", nameof(amount));
            
            }
            if(string.IsNullOrWhiteSpace(currency))
            {
                throw new ArgumentException("Currency is required",nameof(currency));
            }
            Amount = amount;
            Currency = currency;
        }

        //DDD:Value objects compare by value , not reference
        public override bool Equals(object? obj)=> Equals(obj as Money);

        public bool Equals(Money? other)=>other is not null && Amount == other.Amount 
            &&  Currency == other.Currency;

        public override int GetHashCode()=> HashCode.Combine(Amount, Currency);
        public override string ToString()=> $"{Amount}{Currency}";
    }
}
