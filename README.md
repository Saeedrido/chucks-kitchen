# Chuks Kitchen - Food Ordering & Customer Management System

## Client Information
- **Client**: Mr. Chukwudi Okorie
- **Business**: Chuks Kitchen
- **Project**: Food Ordering & Customer Management System
- **Deliverable**: Backend Developer Internship Project

## System Overview

Chuks Kitchen is a digital food ordering platform that allows customers to browse food items, add items to cart, place orders, and track order status. Admins can manage food items, update prices, and manage order statuses.

## Technology Stack

- **Language**: C#
- **Framework**: .NET 8
- **API**: ASP.NET Core Web API
- **ORM**: Entity Framework Core 8.0
- **Database**: PostgreSQL
- **Architecture**: Clean Architecture with Service Layer
- **API Documentation**: Swagger/OpenAPI

## Architecture

### Clean Architecture Layers

```
ChuksKitchen.sln
├── ChuksKitchen.Domain          # Core entities, enums, interfaces
├── ChuksKitchen.Application     # Services, DTOs, business logic
├── ChuksKitchen.Persistence     # DbContext, repositories
├── ChuksKitchen.Infrastructure # External services (OTP, Email, SMS)
└── ChuksKitchen.API           # Controllers, HTTP endpoints
```

### Key Architectural Rules

1. **Domain Layer** - No dependencies on other layers
2. **Service Layer (MANDATORY)** - All business logic resides here
3. **Controllers** - Only handle HTTP requests/responses, call services
4. **Repositories** - Only handle database operations
5. **No business logic in Controllers or Repositories**

## Project Structure

```
ChuksKitchen/
├── ChuksKitchen.Domain/
│   ├── Entities/
│   │   ├── User.cs
│   │   ├── FoodItem.cs
│   │   ├── Cart.cs
│   │   ├── CartItem.cs
│   │   ├── Order.cs
│   │   └── OrderItem.cs
│   └── Enums/
│       ├── OrderStatus.cs
│       ├── UserRole.cs
│       └── RegistrationMethod.cs
│
├── ChuksKitchen.Application/
│   ├── DTOs/
│   │   ├── Requests/
│   │   │   ├── RegisterRequestDto.cs
│   │   │   ├── VerifyOtpRequestDto.cs
│   │   │   ├── LoginRequestDto.cs
│   │   │   ├── CreateFoodItemRequestDto.cs
│   │   │   ├── UpdateFoodItemRequestDto.cs
│   │   │   ├── AddToCartRequestDto.cs
│   │   │   ├── UpdateCartItemRequestDto.cs
│   │   │   ├── CreateOrderRequestDto.cs
│   │   │   └── UpdateOrderStatusRequestDto.cs
│   │   └── Responses/
│   │       ├── ResponseDto.cs
│   │       ├── UserResponseDto.cs
│   │       ├── FoodItemResponseDto.cs
│   │       ├── CartItemResponseDto.cs
│   │       ├── CartResponseDto.cs
│   │       ├── OrderItemResponseDto.cs
│   │       └── OrderResponseDto.cs
│   ├── Services/
│   │   ├── AuthService.cs
│   │   ├── FoodService.cs
│   │   ├── CartService.cs
│   │   └── OrderService.cs
│   └── Interfaces/
│       ├── IAuthService.cs
│       ├── IFoodService.cs
│       ├── ICartService.cs
│       └── IOrderService.cs
│
├── ChuksKitchen.Persistence/
│   ├── Data/
│   │   └── AppDbContext.cs
│   ├── Repositories/
│   │   ├── Repository.cs
│   │   ├── IRepository.cs
│   │   ├── UserRepository.cs
│   │   ├── FoodItemRepository.cs
│   │   ├── CartRepository.cs
│   │   └── OrderRepository.cs
│   └── Configurations/
│       ├── UserConfiguration.cs
│       ├── FoodItemConfiguration.cs
│       ├── CartConfiguration.cs
│       ├── CartItemConfiguration.cs
│       ├── OrderConfiguration.cs
│       └── OrderItemConfiguration.cs
│
├── ChuksKitchen.Infrastructure/
│   ├── Services/
│   │   ├── EmailService.cs
│   │   ├── SmsService.cs
│   │   └── OtpService.cs
│   └── Helpers/
│       ├── PasswordHelper.cs
│       ├── ValidationHelper.cs
│       └── DateTimeHelper.cs
│
└── ChuksKitchen.API/
    ├── Controllers/
    │   ├── AuthController.cs
    │   ├── FoodController.cs
    │   ├── CartController.cs
    │   └── OrderController.cs
    ├── appsettings.json
    ├── appsettings.Development.json
    └── Program.cs
```

## API Endpoints

### Authentication Endpoints

#### POST /api/v1/auth/register
Register a new user

**Request Body:**
```json
{
  "email": "customer@example.com",
  "phone": "+2348012345678",
  "password": "SecurePassword123",
  "firstName": "John",
  "lastName": "Doe",
  "referralCode": "referrer@example.com",
  "registrationMethod": 1
}
```

**Response:**
```json
{
  "success": true,
  "message": "Registration successful. Please verify your account with the OTP sent to your email/phone.",
  "data": {
    "id": 1,
    "email": "customer@example.com",
    "phone": "+2348012345678",
    "firstName": "John",
    "lastName": "Doe",
    "isVerified": false,
    "role": 1
  }
}
```

#### POST /api/v1/auth/verify
Verify user account with OTP

**Request Body:**
```json
{
  "emailOrPhone": "customer@example.com",
  "otpCode": "123456"
}
```

#### POST /api/v1/auth/login
Login user

**Request Body:**
```json
{
  "emailOrPhone": "customer@example.com",
  "password": "SecurePassword123"
}
```

#### POST /api/v1/auth/generate-otp
Generate new OTP

**Request Body:**
```json
{
  "emailOrPhone": "customer@example.com"
}
```

### Food Endpoints

#### GET /api/v1/food
Get all food items (Admin)

#### GET /api/v1/food/available
Get available food items for customers

#### GET /api/v1/food/category/{category}
Get food items by category

#### GET /api/v1/food/{id}
Get food item by ID

#### POST /api/v1/food
Create new food item (Admin)

**Headers:**
```
adminId: 1
```

**Request Body:**
```json
{
  "name": "Jollof Rice",
  "description": "Delicious Jollof rice with chicken",
  "price": 2500,
  "imageUrl": "https://example.com/jollof.jpg",
  "category": "Rice Dishes",
  "preparationTimeMinutes": 20,
  "stockQuantity": 50,
  "spiceLevel": "Medium"
}
```

#### PUT /api/v1/food/{id}
Update food item (Admin)

#### DELETE /api/v1/food/{id}
Delete food item (Admin)

### Cart Endpoints

#### GET /api/v1/cart
Get user's cart

**Headers:**
```
userId: 1
```

#### POST /api/v1/cart/add
Add item to cart

**Headers:**
```
userId: 1
```

**Request Body:**
```json
{
  "foodItemId": 1,
  "quantity": 2,
  "specialInstructions": "No onions please"
}
```

#### PUT /api/v1/cart/update
Update cart item

**Headers:**
```
userId: 1
```

**Request Body:**
```json
{
  "cartItemId": 1,
  "quantity": 3
}
```

#### DELETE /api/v1/cart/remove/{cartItemId}
Remove item from cart

**Headers:**
```
userId: 1
```

#### DELETE /api/v1/cart/clear
Clear cart

**Headers:**
```
userId: 1
```

### Order Endpoints

#### POST /api/v1/order
Create a new order

**Headers:**
```
userId: 1
```

**Request Body:**
```json
{
  "deliveryAddress": "123 Main Street, Lagos",
  "specialInstructions": "Please call when you arrive"
}
```

#### GET /api/v1/order/{orderId}
Get order by ID

**Headers:**
```
userId: 1
```

#### GET /api/v1/order/number/{orderNumber}
Get order by order number

#### GET /api/v1/order/user
Get user's orders

**Headers:**
```
userId: 1
```

#### GET /api/v1/order/all
Get all orders (Admin)

#### PUT /api/v1/order/{orderId}/status
Update order status (Admin)

**Request Body:**
```json
{
  "newStatus": 2,
  "cancellationReason": null
}
```

Order Status values:
- 1: Pending
- 2: Confirmed
- 3: Preparing
- 4: OutForDelivery
- 5: Completed
- 6: Cancelled

#### POST /api/v1/order/{orderId}/cancel
Cancel order

**Headers:**
```
userId: 1
```

**Request Body:**
```json
{
  "reason": "Changed my mind"
}
```

## Data Models

### Order Status Lifecycle

```
Pending → Confirmed → Preparing → OutForDelivery → Completed
                     ↓
                  Cancelled
```

### Key Entities

**User**
- Email OR Phone registration
- Optional referral code
- OTP verification required
- Role: Customer or Admin

**FoodItem**
- Name, description, price
- Category, spice level
- Availability status
- Stock quantity

**Cart**
- One cart per user
- Multiple items

**Order**
- Unique order number (CK + timestamp + random)
- Status tracking with timestamps
- Delivery fee: ₦500

## Business Logic & Edge Cases

### Authentication Flow
1. User registers with email/phone
2. OTP is generated and sent (simulated in logs)
3. User verifies account with OTP
4. OTP expires after 10 minutes
5. Account locks after 3 failed OTP attempts

### Cart Management
1. Food availability checked before adding
2. Stock quantity validated
3. Duplicates merge (quantity increased)
4. Unavailable items detected and prevented

### Order Processing
1. Cart items validated for availability and stock
2. Stock reserved immediately
3. Unique order number generated
4. Cart cleared after order placement
5. Stock restored on cancellation

### Order Status Rules
1. Only Pending/Confirmed orders can be cancelled
2. Valid transitions enforced
3. Automatic timestamp tracking
4. Stock restoration on cancellation

## Setup Instructions

### Prerequisites
- .NET 8 SDK
- PostgreSQL 14+
- Visual Studio 2022 / VS Code

### Database Setup

1. Install PostgreSQL
2. Create database:
```sql
CREATE DATABASE chukskitchen_dev;
```

3. Update connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=chukskitchen_dev;Username=postgres;Password=your_password"
  }
}
```

### Running the Application

1. Open solution in Visual Studio
2. Set ChuksKitchen.API as startup project
3. Press F5 or run:
```bash
cd ChuksKitchen.API
dotnet run
```

4. Access Swagger UI: `https://localhost:5000/swagger`

### Creating Database Tables

The database will be auto-created on first run using `EnsureCreated()`.

For production, use migrations:
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## Scalability Considerations

### Current Scale (100 users)
- Single database server
- In-memory caching
- Synchronous processing

### Scaling to 10,000 Users

1. **Database Optimization**
   - Add indexes on frequently queried fields
   - Implement database sharding
   - Use read replicas for queries

2. **Caching Layer**
   - Redis for session management
   - Cache frequently accessed food items
   - Cache user cart data

3. **Horizontal Scaling**
   - Load balancer (Nginx/Azure Load Balancer)
   - Multiple API instances
   - Stateless authentication (JWT)

4. **Background Jobs**
   - Hangfire for order processing
   - Async email/SMS queue
   - Scheduled tasks for order cleanup

5. **CDN for Static Assets**
   - Azure Blob Storage / AWS S3
   - CloudFlare CDN

6. **Monitoring & Logging**
   - Application Insights / ELK Stack
   - Performance monitoring
   - Error tracking

## API Testing Example

### Using cURL

**Register User:**
```bash
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john@example.com",
    "phone": "+2348012345678",
    "password": "SecurePass123",
    "firstName": "John",
    "lastName": "Doe",
    "registrationMethod": 1
  }'
```

**Get Available Food:**
```bash
curl http://localhost:5000/api/food/available
```

**Add to Cart:**
```bash
curl -X POST http://localhost:5000/api/cart/add \
  -H "Content-Type: application/json" \
  -H "userId: 1" \
  -d '{
    "foodItemId": 1,
    "quantity": 2
  }'
```

**Create Order:**
```bash
curl -X POST http://localhost:5000/api/orders \
  -H "Content-Type: application/json" \
  -H "userId: 1" \
  -d '{
    "deliveryAddress": "123 Main St, Lagos",
    "specialInstructions": "Call when you arrive"
  }'
```

## Assumptions & Notes

1. **Authentication**: Currently using userId header. In production, use JWT tokens
2. **OTP**: Generated and logged. In production, integrate with real SMS/Email provider
3. **Payments**: Payment logic not implemented (database field exists)
4. **Delivery Fee**: Fixed at ₦500. Could be distance-based in production
5. **Order Number**: Generated as CK + timestamp + random digits
6. **Soft Delete**: Entities use IsDeleted flag instead of hard delete
7. **Timezone**: All timestamps in UTC. Convert to Nigerian time (WAT) for display

## Deliverable Summary

### ✅ Completed

1. **Backend Flow Design**
   - Signup & verification flow documented
   - Food browsing flow documented
   - Cart flow documented
   - Order placement flow documented
   - Order status lifecycle documented
   - Admin management flow documented

2. **Working APIs (10+ endpoints)**
   - User: Register, Verify, Login, Generate OTP
   - Food: Get all, Get available, Get by ID, Get by category, Create, Update, Delete
   - Cart: Get, Add, Update, Remove, Clear
   - Order: Create, Get by ID, Get by order number, Get user orders, Get all, Update status, Cancel

3. **Data Modeling**
   - User entity with relationships
   - FoodItem entity with relationships
   - Cart & CartItem entities
   - Order & OrderItem entities
   - Enums for OrderStatus, UserRole, RegistrationMethod

4. **Clean Architecture**
   - Domain layer with entities
   - Application layer with services (ALL business logic)
   - Persistence layer with repositories
   - Infrastructure layer with external services
   - API layer with controllers

5. **Edge Cases Handled**
   - Duplicate email/phone detection
   - Invalid/expired OTP handling
   - Invalid referral code handling
   - Abandoned signup handling
   - Food unavailability after cart addition
   - Stock quantity validation
   - Order cancellation rules
   - Admin and customer cancellation support

6. **Documentation**
   - System overview
   - Flow explanations
   - API endpoints
   - Edge case handling
   - Scalability considerations (100 → 10,000 users)

---

**Developer**: Abdul-Rasheed Muhammad
**Date**: February 2025
**Program**: Trueminds Internship Program
