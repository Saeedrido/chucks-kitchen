# Chuks Kitchen - Food Ordering & Customer Management System

## 📋 Project Overview

Chuks Kitchen is a **backend API** for a food ordering platform that allows customers to:
- Register and verify their accounts
- Browse available food items
- Add items to cart and place orders
- Track order status in real-time
- Admins can manage food items and orders

**Tech Stack:** ASP.NET Core 8.0, Entity Framework Core, JWT Authentication

---

## 🏗️ Architecture

```
ChuksKitchen.sln
├── ChuksKitchen.Domain          # Core entities (User, Food, Order, etc.)
├── ChuksKitchen.Application     # Business logic & services
├── ChuksKitchen.Persistence     # Database (InMemory) & repositories
├── ChuksKitchen.Infrastructure  # External services (Email, SMS, OTP)
└── ChuksKitchen.API            # REST API endpoints
```

---

## ✨ Features

### **Customer Features**
- ✅ User registration (Email/Phone)
- ✅ OTP-based account verification
- ✅ JWT authentication with password hashing
- ✅ Browse and filter food items
- ✅ Shopping cart management
- ✅ Order placement & tracking
- ✅ Order cancellation

### **Admin Features**
- ✅ Add/Update/Delete food items
- ✅ Manage prices & availability
- ✅ Update order statuses
- ✅ View all orders

### **Security**
- ✅ JWT authentication
- ✅ BCrypt password hashing
- ✅ Global exception handling
- ✅ Input validation
- ✅ Role-based access control (Customer/Admin)

---

## 🚀 Quick Setup

### **Prerequisites**
- .NET 8.0 SDK
- Visual Studio 2022 or VS Code

### **Run the Application**

```bash
# 1. Navigate to API folder
cd ChuksKitchen.API

# 2. Run the application
dotnet run

# 3. Access Swagger UI
# Open: http://localhost:5183/swagger
```

**That's it!** The database is InMemory and auto-created on startup.

---

## 📡 API Endpoints Summary

### **Authentication** (4 endpoints)
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/v1/auth/register` | Register new user |
| POST | `/api/v1/auth/verify` | Verify account with OTP |
| POST | `/api/v1/auth/login` | Login & get JWT token |
| POST | `/api/v1/auth/generate-otp` | Resend OTP |

### **Food Management** (7 endpoints)
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/v1/food` | Get all food items (Admin) |
| GET | `/api/v1/food/available` | Get available food (Customer) |
| GET | `/api/v1/food/{id}` | Get food by ID |
| GET | `/api/v1/food/category/{category}` | Filter by category |
| POST | `/api/v1/food` | Add food item (Admin) |
| PUT | `/api/v1/food/{id}` | Update food (Admin) |
| DELETE | `/api/v1/food/{id}` | Delete food (Admin) |

### **Cart** (5 endpoints)
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/v1/cart` | Get user's cart |
| POST | `/api/v1/cart/add` | Add item to cart |
| PUT | `/api/v1/cart/update` | Update cart item |
| DELETE | `/api/v1/cart/remove/{id}` | Remove item |
| DELETE | `/api/v1/cart/clear` | Clear cart |

### **Orders** (8 endpoints)
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/v1/order` | Create order |
| GET | `/api/v1/order/{id}` | Get order by ID |
| GET | `/api/v1/order/number/{orderNumber}` | Get by order number |
| GET | `/api/v1/order/user` | Get user's orders |
| GET | `/api/v1/order/all` | Get all orders (Admin) |
| PUT | `/api/v1/order/{id}/status` | Update status (Admin) |
| POST | `/api/v1/order/{id}/cancel` | Cancel order |

### **User** (2 endpoints)
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/v1/user/me` | Get current user profile |
| GET | `/api/v1/user/by-referral-code/{code}` | Validate referral code |

### **Health & System** (6 endpoints)
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/health` | Basic health check |
| GET | `/api/v1/health/detailed` | Detailed health status |
| GET | `/api/v1/health/ready` | Readiness probe |
| GET | `/api/v1/health/live` | Liveness probe |
| GET | `/api/v1/health/system` | System information |
| GET | `/api/v1/health/time` | Server time |

**Total: 32 Endpoints**

---

## 🧪 Testing the API

### **Quick Test with cURL**

```bash
# 1. Register User
curl -X POST http://localhost:5183/api/v1/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john@example.com",
    "phone": "+2348012345678",
    "password": "SecurePass123!",
    "firstName": "John",
    "lastName": "Doe",
    "registrationMethod": 1
  }'

# 2. Get Available Food
curl http://localhost:5183/api/v1/food/available

# 3. Login (get token from response)
curl -X POST http://localhost:5183/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "emailOrPhone": "john@example.com",
    "password": "SecurePass123!"
  }'

# 4. Health Check
curl http://localhost:5183/api/v1/health/detailed
```

### **Using Swagger UI (Recommended)**
1. Go to `http://localhost:5183/swagger`
2. Click on any endpoint
3. Click "Try it out"
4. Fill in the request body
5. Click "Execute"

---

## 📊 Order Status Lifecycle

```
Pending → Confirmed → Preparing → OutForDelivery → Completed
                     ↓
                  Cancelled
```

---

## 💾 Data Model

### **Core Entities**
- **User** - Customer accounts with referral codes
- **FoodItem** - Menu items with prices & availability
- **Cart/CartItem** - Shopping cart
- **Order/OrderItem** - Customer orders with status tracking

### **Key Features**
- Unique order numbers (CK + timestamp + random)
- OTP verification (expires in 10 minutes)
- Stock management
- Referral code system
- Delivery fee: ₦500

---

## 📚 Detailed Documentation

| Document | Description |
|----------|-------------|
| [SUBMISSION_README.md](SUBMISSION_README.md) | Submission package & checklist |
| [docs/API_DOCUMENTATION.md](docs/API_DOCUMENTATION.md) | Complete API reference |
| [docs/DATA_FLOWS.md](docs/DATA_FLOWS.md) | Business flow explanations |
| [docs/EDGE_CASES.md](docs/EDGE_CASES.md) | Edge case handling |
| [docs/FLOW_DIAGRAMS.md](docs/FLOW_DIAGRAMS.md) | Flow diagram descriptions |

---

## 🎯 Deliverable Status

| Requirement | Status |
|-------------|--------|
| Backend Flow Diagrams | ✅ 6 PDF diagrams in `/Diagrams/` |
| Working APIs | ✅ 32 endpoints (6 APIs) |
| Data Model | ✅ Complete ERD & entities |
| Documentation | ✅ Comprehensive docs |

---

## 📝 Important Notes

1. **Database:** Using InMemory database (auto-created on startup)
2. **OTP:** Generated and logged to console (check your terminal)
3. **Authentication:** Currently using userId header for simplicity
4. **Delivery Fee:** Fixed at ₦500
5. **Timezone:** All timestamps in UTC

---

## 🏆 Scalability Considerations

**For 100 users:** Current setup works perfectly
**For 10,000+ users:** Would need:
- PostgreSQL instead of InMemory
- Redis caching for frequently accessed data
- Load balancing with multiple API instances
- CDN for static assets

---

## 👤 Developer

**Name:** [Your Name]
**Project:** Chuks Kitchen Food Ordering API
**Internship:** TrueMinds Innovations Ltd
**Date:** February 2026

---

**Built with ❤️ using ASP.NET Core 8.0 & Clean Architecture**
