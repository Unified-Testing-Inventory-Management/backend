# StockWise Backend

**StockWise** is a C# .NET Core Web API for managing inventory efficiently. This backend provides full CRUD functionality, product stock tracking, and Excel import capabilities, making it easy to monitor inventory and prevent stock issues.

---

## Features

- Full **CRUD operations** for products  
- **Stock tracking**: identify low-stock and out-of-stock items  
- **Excel import** for bulk product data  
- Uses **Microsoft SQL Server** for data storage  

---

## Technologies

- **Language:** C#  
- **Framework:** .NET Core Web API  
- **Database:** Microsoft SQL Server  
- **Libraries:** Entity Framework Core, EPPlus (for Excel import), SkiaSharp (for barcode generation), Scalar (API)

---

## Product Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/v1/products` | Retrieve all products |
| GET | `/api/v1/products/{id}` | Retrieve a single product by ID |
| POST | `/api/v1/products` | Create a new product |
| POST | `/api/v1/products/import-excel` | Import products from an Excel file |
| PATCH | `/api/v1/products/{id}` | Update an existing product |
| DELETE | `/api/v1/products/{id}` | Delete a product |

## User Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/v1/auth/me` | Retrieve the currently authenticated user's profile |
| POST | `/api/v1/auth/login` | Authenticate a user and obtain a token |
| POST | `/api/v1/auth/register` | Register a new user account |
| POST | `/api/v1/auth/logout` | Log out the current user and invalidate the token |
| PATCH | `/api/v1/auth/update-profile` | Update the profile details of the authenticated user |

## Getting Started

1. **Clone the repository**  

- Using HTTPS
```bash
git clone https://github.com/Unified-Testing-Inventory-Management/backend.git
cd backend
```
- Using SSH
```bash
  git clone git@github.com:Unified-Testing-Inventory-Management/backend.git
  cd backend
```

2. **Restore the dependencies**
```bash
  dotnet restore
```

3. **Build the application**
```bash
  dotnet build
```

3. **Run the solution**
```bash
  dotnet run
```

**Databases**

1. **Apply the migrations**
```bash
  dotnet ef migrations add (migration name)
```

2. **Apply the databaae**
```bash
  dotnet ef database update
```

3. **Drop the database**
```bash
  dotnet ef database drop
```
