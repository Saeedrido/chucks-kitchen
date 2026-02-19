# API Documentation - Chuks Kitchen

## 📡 Base URL
```
http://localhost:5183/api/v1
```

---

## 🔐 Authentication API

### **POST /auth/register**
Register a new user with email or phone.

**Request Body:**
```json
{
  "email": "john@example.com",
  "phone": "+2348012345678",
  "password": "SecurePass123!",
  "firstName": "John",
  "lastName": "Doe",
  "referralCode": "REF123",
  "registrationMethod": 1
}
```
**registrationMethod:** 1 = Email, 2 = Phone

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Registration successful. Please verify your account.",
  "data": {
    "id": 1,
    "email": "john@example.com",
    "phone": "+2348012345678",
    "firstName": "John",
    "lastName": "Doe",
    "isVerified": false,
    "role": 1
  }
}
```

**Errors:**
- 400: Email/phone already exists
- 400: Invalid referral code

---

### **POST /auth/verify**
Verify account with OTP.

**Request Body:**
```json
{
  "emailOrPhone": "john@example.com",
  "otpCode": "123456"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Account verified successfully!",
  "data": {
    "id": 1,
    "email": "john@example.com",
    "isVerified": true
  }
}
```

**Errors:**
- 400: Invalid OTP
- 400: OTP expired (10 minutes)
- 400: Account locked (3 failed attempts)

---

### **POST /auth/login**
Login with email/phone and password.

**Request Body:**
```json
{
  "emailOrPhone": "john@example.com",
  "password": "SecurePass123!"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Login successful",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "user": {
      "id": 1,
      "email": "john@example.com",
      "firstName": "John",
      "lastName": "Doe"
    }
  }
}
```

**Errors:**
- 400: Invalid credentials
- 400: Account not verified

---

### **POST /auth/generate-otp**
Generate new OTP code.

**Request Body:**
```json
{
  "emailOrPhone": "john@example.com"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "New OTP sent to your email/phone"
}
```

---

## 🍔 Food Management API

### **GET /food**
Get all food items (Admin only).

**Headers:**
```
adminId: 1
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "name": "Jollof Rice",
      "description": "Delicious jollof rice with chicken",
      "price": 2500,
      "category": "Rice Dishes",
      "isAvailable": true,
      "stockQuantity": 50
    }
  ]
}
```

---

### **GET /food/available**
Get available food items for customers.

**Response (200 OK):**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "name": "Jollof Rice",
      "price": 2500,
      "category": "Rice Dishes"
    }
  ]
}
```

---

### **GET /food/{id}**
Get food item by ID.

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "name": "Jollof Rice",
    "description": "Delicious jollof rice",
    "price": 2500,
    "imageUrl": "http://example.com/jollof.jpg",
    "category": "Rice Dishes",
    "isAvailable": true,
    "stockQuantity": 50,
    "preparationTimeMinutes": 20,
    "spiceLevel": "Medium"
  }
}
```

**Errors:**
- 404: Food item not found

---

### **GET /food/category/{category}**
Get food items by category.

**Categories:** Rice Dishes, Swallow, Soups, Drinks, etc.

**Response (200 OK):**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "name": "Jollof Rice",
      "price": 2500,
      "category": "Rice Dishes"
    }
  ]
}
```

---

### **POST /food**
Add new food item (Admin only).

**Headers:**
```
adminId: 1
```

**Request Body:**
```json
{
  "name": "Jollof Rice",
  "description": "Delicious jollof rice with chicken",
  "price": 2500,
  "imageUrl": "http://example.com/jollof.jpg",
  "category": "Rice Dishes",
  "preparationTimeMinutes": 20,
  "stockQuantity": 50,
  "spiceLevel": "Medium"
}
```

**Response (201 Created):**
```json
{
  "success": true,
  "message": "Food item added successfully",
  "data": {
    "id": 1,
    "name": "Jollof Rice",
    "price": 2500
  }
}
```

---

### **PUT /food/{id}**
Update food item (Admin only).

**Headers:**
```
adminId: 1
```

**Request Body:**
```json
{
  "name": "Jollof Rice Special",
  "price": 3000,
  "isAvailable": false
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Food item updated successfully"
}
```

---

### **DELETE /food/{id}**
Delete food item (Admin only).

**Headers:**
```
adminId: 1
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Food item deleted successfully"
}
```

---

## 🛒 Cart API

### **GET /cart**
Get user's cart.

**Headers:**
```
userId: 1
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "userId": 1,
    "items": [
      {
        "id": 1,
        "foodItemId": 1,
        "foodName": "Jollof Rice",
        "quantity": 2,
        "unitPrice": 2500,
        "specialInstructions": "No onions"
      }
    ],
    "totalItems": 2,
    "totalAmount": 5000
  }
}
```

---

### **POST /cart/add**
Add item to cart.

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

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Item added to cart"
}
```

**Errors:**
- 400: Food item not available
- 400: Insufficient stock

---

### **PUT /cart/update**
Update cart item quantity.

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

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Cart updated"
}
```

---

### **DELETE /cart/remove/{cartItemId}**
Remove item from cart.

**Headers:**
```
userId: 1
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Item removed from cart"
}
```

---

### **DELETE /cart/clear**
Clear entire cart.

**Headers:**
```
userId: 1
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Cart cleared"
}
```

---

## 📦 Order API

### **POST /order**
Create new order from cart.

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

**Response (201 Created):**
```json
{
  "success": true,
  "message": "Order placed successfully",
  "data": {
    "id": 1,
    "orderNumber": "CK20260220123456789",
    "userId": 1,
    "status": "Pending",
    "totalAmount": 5500,
    "deliveryFee": 500,
    "deliveryAddress": "123 Main Street, Lagos",
    "createdAt": "2026-02-20T10:30:00Z"
  }
}
```

**Errors:**
- 400: Cart is empty
- 400: Some items are out of stock

---

### **GET /order/{orderId}**
Get order by ID.

**Headers:**
```
userId: 1
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "orderNumber": "CK20260220123456789",
    "status": "Confirmed",
    "totalAmount": 5500,
    "deliveryFee": 500,
    "deliveryAddress": "123 Main Street",
    "items": [
      {
        "foodName": "Jollof Rice",
        "quantity": 2,
        "unitPrice": 2500
      }
    ],
    "statusHistory": [
      {
        "status": "Pending",
        "timestamp": "2026-02-20T10:30:00Z"
      },
      {
        "status": "Confirmed",
        "timestamp": "2026-02-20T10:35:00Z"
      }
    ]
  }
}
```

---

### **GET /order/number/{orderNumber}**
Get order by order number.

**Response (200 OK):** Same as above

---

### **GET /order/user**
Get user's order history.

**Headers:**
```
userId: 1
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "orderNumber": "CK20260220123456789",
      "status": "Confirmed",
      "totalAmount": 5500,
      "createdAt": "2026-02-20T10:30:00Z"
    }
  ]
}
```

---

### **GET /order/all**
Get all orders (Admin only).

**Response (200 OK):** Array of all orders

---

### **PUT /order/{orderId}/status**
Update order status (Admin only).

**Request Body:**
```json
{
  "newStatus": 3,
  "cancellationReason": null
}
```

**Status Values:**
- 1: Pending
- 2: Confirmed
- 3: Preparing
- 4: OutForDelivery
- 5: Completed
- 6: Cancelled

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Order status updated"
}
```

---

### **POST /order/{orderId}/cancel**
Cancel order.

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

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Order cancelled successfully"
}
```

**Errors:**
- 400: Order cannot be cancelled (already preparing/delivered)

---

## 👤 User API

### **GET /user/me**
Get current user profile.

**Headers:**
```
Authorization: Bearer {token}
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "email": "john@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "referralCode": "CK-ABC123",
    "isVerified": true,
    "role": "Customer"
  }
}
```

---

### **GET /user/by-referral-code/{code}**
Validate referral code.

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "referralCode": "CK-ABC123",
    "isValid": true
  }
}
```

---

## 🏥 Health & System API

### **GET /health**
Basic health check.

**Response (200 OK):**
```json
{
  "status": "healthy",
  "timestamp": "2026-02-20T10:30:00Z",
  "message": "Chuks Kitchen API is running"
}
```

---

### **GET /health/detailed**
Detailed health status.

**Response (200 OK):**
```json
{
  "status": "healthy",
  "timestamp": "2026-02-20T10:30:00Z",
  "api": {
    "name": "Chuks Kitchen API",
    "version": "v1.0.0",
    "environment": "Development",
    "uptime": "2h 15m"
  },
  "database": {
    "status": "healthy",
    "type": "InMemory"
  },
  "services": {
    "emailService": "registered",
    "smsService": "registered",
    "otpService": "registered"
  },
  "responseTime": "15.42ms"
}
```

---

### **GET /health/system**
System information.

**Response (200 OK):**
```json
{
  "timestamp": "2026-02-20T10:30:00Z",
  "server": {
    "time": "2026-02-20 10:30:00",
    "timeZone": "UTC",
    "machineName": "DESKTOP-XXX",
    "osVersion": "Windows 10",
    "processorCount": 8,
    "workingSet": "256MB"
  },
  "api": {
    "name": "Chuks Kitchen API",
    "version": "1.0.0",
    "framework": ".NET 8.0",
    "uptime": "2h 15m"
  }
}
```

---

### **GET /health/time**
Server time.

**Response (200 OK):**
```json
{
  "timestamp": "2026-02-20T10:30:00Z",
  "readable": "2026-02-20 10:30:00",
  "timeZone": "UTC"
}
```

---

## 📊 Response Format

All endpoints follow this response format:

**Success Response:**
```json
{
  "success": true,
  "message": "Operation successful",
  "data": { ... }
}
```

**Error Response:**
```json
{
  "success": false,
  "message": "Error description",
  "errors": [ ... ]
}
```

---

## 🔒 HTTP Status Codes

- **200 OK** - Request successful
- **201 Created** - Resource created
- **400 Bad Request** - Invalid input
- **401 Unauthorized** - Authentication required
- **403 Forbidden** - Permission denied
- **404 Not Found** - Resource not found
- **500 Internal Server Error** - Server error

---

**Full API Documentation available at:** `http://localhost:5183/swagger`
