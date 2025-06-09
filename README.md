# 🏝️ Travel Booking Integration Platform - BookWithExt (product service)

A modern, extensible travel booking platform that integrates with multiple external travel APIs to provide a unified interface for tour packages, holiday accommodations, and custom travel products.

## 🏗️ Architecture

This solution follows Clean Architecture principles with a clear separation of concerns:

```
├── Core
│   ├── Domain          # Entities, value objects, domain events
│   └── Application     # Use cases, DTOs, interfaces, services
└── Infrastructure
    ├── Adapters        # External API integrations
    └── Persistence     # Database implementations
```

## 🔑 Key Features

- ✅ **Multi-Provider Integration**: Seamlessly connect with multiple travel APIs (booking.com, agoda.com)
- ✅ **Product Synchronization**:  Sync products from external sources to local database
- ✅ **Unified Purchase Flow**: Common purchase interface regardless of product source
- ✅ **Extensible Adapter System**: Easily add new travel providers by implementing the `IExternalProductApiAdapter` interface


## 🧩 Core Components

### External API Adapters

The platform uses adapters to integrate with different travel provider APIs:

- **TourApiAdapter**: Connects to booking.com(name for mock api for tour) for tour packages
- **HolidayPackageAdapter**: Connects to agoda.com(name for mock api for holiday) for holiday accommodations

Each adapter handles data mapping from the external API's format to our unified `ProductDto` model.
All adapter related customization is done here and returns as a unified entity that receives from service layer.

### Product Service

The central service managing all product operations:

- Fetch products from local database
- Retrieve and sync products from external APIs
- Handle product CRUD operations
- Process purchase requests


## 🛠️ Technologies

- **.NET 8**
- **Entity Framework Core 9.0**
- **C# 12**
- **SQL Server** for data persistence

## 🚀 Getting Started

1. Clone the repository
2. Run `docker-compose up --build`
3. Run database migrations: `dotnet ef migrations --project .\Infrastructure.Persistence.csproj add wewew --startup-project ..\ProductService.API\` from inside project `BookingWithExt\Infrastructure.Persistence`.also make sure to update database with `dotnet ef database update --startup-project ..\ProductService.API\` as well.

Note:product service (this project) uses the cotainarized database that defined in the _docker-compose.yaml_.

Also make sure to clone the repo and containerize the order service to add products to the  cart and purchase functionalities.

Order-Service repo: https://github.com/laikalakmal/BookWithExt-OrderService.git


## 🔌 Adding New Providers

To integrate a new travel API provider:

1. Create a new adapter class implementing `IExternalProductApiAdapter`
2. Implement required methods: `FetchProductsAsync`, `FetchProductByIdAsync`, and `PurchaseProductAsync`
3. Map provider-specific data to the unified `ProductDto` format
4. Register your adapter in the DI container


```
