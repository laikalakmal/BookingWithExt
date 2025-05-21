using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Infrastructure.Persistence.Extensions
{
    // Helper extension method
    internal static class ValueConversionExtensions
    {
        public static PropertyBuilder<T> HasJsonConversion<T>(this PropertyBuilder<T> propertyBuilder)
        {
            return propertyBuilder.HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<T>(v, (JsonSerializerOptions?)null) ?? default!);
        }
    }

}

//What does this code do is it defines an extension method for Entity Framework Core's PropertyBuilder class. This extension method, HasJsonConversion, allows you to easily configure a property to be stored as JSON in the database. It uses the System.Text.Json library to serialize and deserialize the property value to and from JSON format. The method is generic, meaning it can be used with any type T, making it versatile for different entity properties that need JSON conversion.


