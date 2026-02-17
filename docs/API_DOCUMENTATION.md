# Chuks Kitchen API Documentation

## Base URL
```
http://localhost:5000/api/v1
```

## API Versioning
This API uses versioning to ensure backward compatibility. Current version: **v1.0**

All endpoints include the version prefix: `/api/v1/`

## Authentication
Currently using `userId` header for simplicity. In production, use JWT Bearer tokens.

```
userId: 1
```

---

## Authentication APIs

### 1. Register User
**Endpoint:** `POST /api/v1/auth/register`

**Description:** Register a new user with email or phone number. OTP is generated and returned in response (development mode).

**Request Body:**
```json
{
  "email": "customer@example.com",
  "phone": "+2348012345678",
  "password": "SecurePassword123",
  "firstName": "John",
  "lastName": "Doe",
  "referralCode": "referrer@email.com",
  "registrationMethod": 1
}
```

**Request Schema:**
| Field | Type | Required | Description |
|-------|------|----------|-------------|
| email | string | Yes* | User email (required if registrationMethod=1) |
| phone | string | Yes* | User phone (required if registrationMethod=2) |
| password | string | Yes | Password (min 6 characters) |
| firstName | string | Yes | User's first name |
| lastName | string | Yes | User's last name |
| referralCode | string | No | Referrer's email for referral bonus |
| registrationMethod | integer | Yes | 1=Email, 2=Phone |

**Success Response:** `200 OK`
```json
{
  "success": true,
  "message": "Registration successful. Please verify your account with the OTP sent to your email/phone. Your OTP is: 123456",
  "data": {
    "id": 1,
    "email": "customer@example.com",
    "phone": "+2348012345678",
    "firstName": "John",
    "lastName": "Doe",
    "isVerified": false,
    "role": 1,
    "fullName": "John Doe",
    "referralCode": "CK-ABC123"
  }
}
```

**Response Schema:**
```json
{
  "success": boolean,
  "message": string,
  "data": {
    "id": integer,
    "email": string,
    "phone": string,
    "firstName": string,
    "lastName": string,
    "isVerified": boolean,
    "role": integer,
    "fullName": string,
    "referralCode": string
  }
}
```

**Error Responses:**
- `400 Bad Request`: Email/phone already registered
  ```json
  {
    "success": false,
    "message": "Email already registered",
    "errors": []
  }
  ```
- `400 Bad Request`: Invalid referral code
  ```json
  {
    "success": false,
    "message": "Invalid referral code",
    "errors": []
  }
  ```

**Edge Cases Handled:**
- Duplicate email/phone detection
- Invalid referral code validation
- Referral relationship tracking
- Auto-generated unique referral code (CK-XXXXXX format)

---

### 2. Verify OTP
**Endpoint:** `POST /api/v1/auth/verify`

**Description:** Verify user account using OTP sent to email/phone.

**Request Body:**
```json
{
  "emailOrPhone": "customer@example.com",
  "otpCode": "123456"
}
```

**Request Schema:**
| Field | Type | Required | Description |
|-------|------|----------|-------------|
| emailOrPhone | string | Yes | User's email or phone number |
| otpCode | string | Yes | 6-digit OTP code |

**Success Response:** `200 OK`
```json
{
  "success": true,
  "message": "Account verified successfully",
  "data": {
    "id": 1,
    "email": "customer@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "isVerified": true,
    "role": 1,
    "referralCode": "CK-ABC123"
  }
}
```

**Error Responses:**
- `400 Bad Request`: Invalid OTP
  ```json
  {
    "success": false,
    "message": "Invalid OTP code",
    "errors": []
  }
  ```
- `400 Bad Request`: OTP expired (> 10 minutes)
  ```json
  {
    "success": false,
    "message": "OTP has expired. Please generate a new OTP.",
    "errors": []
  }
  ```
- `400 Bad Request`: Account already verified
  ```json
  {
    "success": false,
    "message": "Account is already verified",
    "errors": []
  }
  ```
- `400 Bad Request`: Too many failed attempts (account locked)
  ```json
  {
    "success": false,
    "message": "Account locked due to too many failed OTP attempts. Please contact support.",
    "errors": []
  }
  ```

**Edge Cases Handled:**
- OTP expiration (10 minutes)
- OTP generation timestamp validation (5 minutes minimum)
- Failed attempt tracking (3 attempts max)
- Account lockout after failures
- Duplicate verification prevention

---

### 3. Login
**Endpoint:** `POST /api/v1/auth/login`

**Description:** Authenticate user with email/phone and password.

**Request Body:**
```json
{
  "emailOrPhone": "customer@example.com",
  "password": "SecurePassword123"
}
```

**Success Response:** `200 OK`
```json
{
  "success": true,
  "message": "Login successful",
  "data": {
    "id": 1,
    "email": "customer@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "isVerified": true,
    "role": 1,
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
  }
}
```

**Error Responses:**
- `400 Bad Request`: Invalid credentials
- `400 Bad Request`: Account not verified

---

### 4. Generate OTP
**Endpoint:** `POST /api/v1/auth/generate-otp`

**Description:** Generate a new OTP for user verification.

**Request Body:**
```json
{
  "emailOrPhone": "customer@example.com"
}
```

**Success Response:** `200 OK`
```json
{
  "success": true,
  "message": "OTP generated successfully. Valid for 10 minutes. Your OTP is: 789012",
  "data": "789012"
}
```

---

## Food Management APIs

### 5. Get All Food Items
**Endpoint:** `GET /api/v1/food`

**Description:** Get all food items (Admin view).

**Success Response:** `200 OK`
```json
{
  "success": true,
  "message": "Operation successful",
  "data": [
    {
      "id": 1,
      "name": "Jollof Rice",
      "description": "Delicious Jollof rice with chicken",
      "price": 2500.00,
      "imageUrl": "https://example.com/jollof.jpg",
      "category": "Rice Dishes",
      "isAvailable": true,
      "preparationTimeMinutes": 20,
      "stockQuantity": 50,
      "spiceLevel": "Medium",
      "addedByAdmin": "Admin User",
      "createdAt": "2025-02-15T10:00:00Z"
    },
    {
      "id": 2,
      "name": "Egusi Soup",
      "description": "Traditional melon seed soup with beef",
      "price": 3000.00,
      "imageUrl": "https://example.com/egusi.jpg",
      "category": "Soups",
      "isAvailable": true,
      "preparationTimeMinutes": 25,
      "stockQuantity": 30,
      "spiceLevel": "Hot",
      "addedByAdmin": "Admin User",
      "createdAt": "2025-02-15T10:30:00Z"
    }
  ]
}
```

**Response Schema:**
```json
{
  "success": boolean,
  "message": string,
  "data": [
    {
      "id": integer,
      "name": string,
      "description": string,
      "price": number,
      "imageUrl": string,
      "category": string,
      "isAvailable": boolean,
      "preparationTimeMinutes": integer,
      "stockQuantity": integer,
      "spiceLevel": string,
      "addedByAdmin": string,
      "createdAt": string (ISO 8601 datetime)
    }
  ]
}
```

---

### 6. Get Available Food Items
**Endpoint:** `GET /api/v1/food/available`

**Description:** Get only available food items for customers to browse.

**Headers:**
```
userId: 1
```

**Success Response:** `200 OK`
```json
{
  "success": true,
  "message": "Operation successful",
  "data": [
    {
      "id": 1,
      "name": "Jollof Rice",
      "description": "Delicious Jollof rice with chicken",
      "price": 2500.00,
      "imageUrl": "https://example.com/jollof.jpg",
      "category": "Rice Dishes",
      "isAvailable": true,
      "preparationTimeMinutes": 20,
      "stockQuantity": 50,
      "spiceLevel": "Medium"
    }
  ]
}
```

**Note:** Only items where `IsAvailable=true` AND `StockQuantity>0` are returned.

---

### 7. Get Food Items by Category
**Endpoint:** `GET /api/v1/food/category/{category}`

**Parameters:**
- `category` (path): Category name (e.g., "Rice Dishes", "Soups", "Proteins")

**Success Response:** `200 OK`
```json
{
  "success": true,
  "data": [
    // Items filtered by category
  ]
}
```

---

### 8. Get Food Item by ID
**Endpoint:** `GET /api/v1/food/{id}`

**Parameters:**
- `id` (path): Food item ID

**Success Response:** `200 OK`
```json
{
  "success": true,
  "message": "Operation successful",
  "data": {
    "id": 1,
    "name": "Jollof Rice",
    "description": "Delicious Jollof rice with chicken",
    "price": 2500.00,
    "imageUrl": "https://example.com/jollof.jpg",
    "category": "Rice Dishes",
    "isAvailable": true,
    "preparationTimeMinutes": 20,
    "stockQuantity": 50,
    "spiceLevel": "Medium",
    "addedByAdmin": "Admin User",
    "createdAt": "2025-02-15T10:00:00Z"
  }
}
```

**Error Response:** `404 Not Found`
```json
{
  "success": false,
  "message": "Food item not found",
  "errors": []
}
```

---

### 9. Create Food Item
**Endpoint:** `POST /api/v1/food`

**Description:** Add a new food item (Admin only).

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

**Request Schema:**
| Field | Type | Required | Description |
|-------|------|----------|-------------|
| name | string | Yes | Food item name |
| description | string | Yes | Food description |
| price | number | Yes | Price in Naira (must be > 0) |
| imageUrl | string | No | Image URL |
| category | string | Yes | Food category |
| preparationTimeMinutes | integer | No | Preparation time (default: 15) |
| stockQuantity | integer | No | Available stock (default: 100) |
| spiceLevel | string | No | Spice level (Mild, Medium, Hot) |

**Success Response:** `201 Created`
```json
{
  "success": true,
  "message": "Food item created successfully",
  "data": {
    "id": 1,
    "name": "Jollof Rice",
    "description": "Delicious Jollof rice with chicken",
    "price": 2500.00,
    "imageUrl": "https://example.com/jollof.jpg",
    "category": "Rice Dishes",
    "isAvailable": true,
    "preparationTimeMinutes": 20,
    "stockQuantity": 50,
    "spiceLevel": "Medium",
    "addedByAdmin": "Admin User",
    "createdAt": "2025-02-15T10:00:00Z"
  }
}
```

**Error Responses:**
- `400 Bad Request`: Invalid price (must be > 0)
  ```json
  {
    "success": false,
    "message": "Price must be greater than zero",
    "errors": []
  }
  ```
- `400 Bad Request`: Invalid stock (cannot be negative)
- `401 Unauthorized`: User is not admin

---

### 10. Update Food Item
**Endpoint:** `PUT /api/v1/food/{id}`

**Description:** Update food item details (Admin only).

**Request Body:**
```json
{
  "name": "Jollof Rice",
  "description": "Delicious Jollof rice with chicken and plantain",
  "price": 2800.00,
  "imageUrl": "https://example.com/jollof.jpg",
  "category": "Rice Dishes",
  "isAvailable": true,
  "preparationTimeMinutes": 20,
  "stockQuantity": 45,
  "spiceLevel": "Hot"
}
```

**Success Response:** `200 OK`
```json
{
  "success": true,
  "message": "Food item updated successfully",
  "data": {
    // Updated food item
  }
}
```

---

### 11. Delete Food Item
**Endpoint:** `DELETE /api/v1/food/{id}`

**Description:** Soft delete food item (Admin only).

**Success Response:** `200 OK`
```json
{
  "success": true,
  "message": "Food item deleted successfully",
  "data": true
}
```

---

## Cart APIs

### 12. Get Cart
**Endpoint:** `GET /api/v1/cart`

**Description:** Get user's current cart with all items.

**Headers:**
```
userId: 1
```

**Success Response:** `200 OK`
```json
{
  "success": true,
  "message": "Operation successful",
  "data": {
    "id": 1,
    "userId": 1,
    "items": [
      {
        "id": 1,
        "foodItemId": 1,
        "foodItemName": "Jollof Rice",
        "foodItemImage": "https://example.com/jollof.jpg",
        "quantity": 2,
        "unitPrice": 2500.00,
        "totalPrice": 5000.00,
        "specialInstructions": "No onions",
        "isAvailable": true
      },
      {
        "id": 2,
        "foodItemId": 2,
        "foodItemName": "Egusi Soup",
        "foodItemImage": "https://example.com/egusi.jpg",
        "quantity": 1,
        "unitPrice": 3000.00,
        "totalPrice": 3000.00,
        "specialInstructions": "Extra beef",
        "isAvailable": true
      }
    ],
    "totalItems": 3,
    "subTotal": 8000.00,
    "deliveryFee": 500.00,
    "totalAmount": 8500.00
  }
}
```

**Response Schema:**
```json
{
  "success": boolean,
  "message": string,
  "data": {
    "id": integer,
    "userId": integer,
    "items": [
      {
        "id": integer,
        "foodItemId": integer,
        "foodItemName": string,
        "foodItemImage": string,
        "quantity": integer,
        "unitPrice": number,
        "totalPrice": number,
        "specialInstructions": string,
        "isAvailable": boolean
      }
    ],
    "totalItems": integer,
    "subTotal": number,
    "deliveryFee": number,
    "totalAmount": number
  }
}
```

**Edge Cases Handled:**
- Empty cart returns empty items array
- Shows availability of each item
- Calculates totals dynamically
- Unavailable items are marked but shown

---

### 13. Add to Cart
**Endpoint:** `POST /api/v1/cart/add`

**Description:** Add food item to cart.

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

**Success Response:** `200 OK`
```json
{
  "success": true,
  "message": "Item added to cart successfully",
  "data": {
    "id": 1,
    "userId": 1,
    "items": [
      {
        "id": 1,
        "foodItemId": 1,
        "foodItemName": "Jollof Rice",
        "quantity": 2,
        "unitPrice": 2500.00,
        "totalPrice": 5000.00,
        "specialInstructions": "No onions please",
        "isAvailable": true
      }
    ],
    "totalAmount": 5500.00
  }
}
```

**Error Responses:**
- `400 Bad Request`: Food item not found
- `400 Bad Request`: Food item unavailable
- `400 Bad Request`: Insufficient stock
- `400 Bad Request`: Invalid quantity

**Edge Cases Handled:**
- Food availability check
- Stock quantity validation
- Duplicate items merge (quantity updated)
- Stock validation for merged quantity

---

### 14. Update Cart Item
**Endpoint:** `PUT /api/v1/cart/update`

**Description:** Update quantity of item in cart.

**Headers:**
```
userId: 1
```

**Request Body:**
```json
{
  "cartItemId": 1,
  "quantity": 3,
  "specialInstructions": "Extra spicy"
}
```

---

### 15. Remove from Cart
**Endpoint:** `DELETE /api/v1/cart/remove/{cartItemId}`

**Success Response:** `200 OK`

---

### 16. Clear Cart
**Endpoint:** `DELETE /api/v1/cart/clear`

**Success Response:** `200 OK`

---

## Order APIs

### 17. Create Order
**Endpoint:** `POST /api/v1/order`

**Description:** Place order from cart items.

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

**Success Response:** `201 Created`
```json
{
  "success": true,
  "message": "Order placed successfully",
  "data": {
    "id": 1,
    "orderNumber": "CK202502171530451234",
    "status": 1,
    "statusDisplay": "Pending",
    "totalAmount": 5500.00,
    "subTotal": 5000.00,
    "deliveryFee": 500.00,
    "deliveryAddress": "123 Main Street, Lagos",
    "specialInstructions": "Please call when you arrive",
    "createdAt": "2025-02-17T15:30:45Z",
    "isPaid": false,
    "items": [
      {
        "id": 1,
        "foodItemId": 1,
        "foodItemName": "Jollof Rice",
        "quantity": 2,
        "unitPrice": 2500.00,
        "totalPrice": 5000.00,
        "specialInstructions": "No onions please"
      }
    ],
    "customer": {
      "id": 1,
      "fullName": "John Doe",
      "email": "customer@example.com",
      "phone": "+2348012345678"
    }
  }
}
```

**Response Schema:**
```json
{
  "success": boolean,
  "message": string,
  "data": {
    "id": integer,
    "orderNumber": string,
    "status": integer,
    "statusDisplay": string,
    "totalAmount": number,
    "subTotal": number,
    "deliveryFee": number,
    "deliveryAddress": string,
    "specialInstructions": string,
    "createdAt": string (ISO 8601),
    "isPaid": boolean,
    "items": [
      {
        "id": integer,
        "foodItemId": integer,
        "foodItemName": string,
        "quantity": integer,
        "unitPrice": number,
        "totalPrice": number,
        "specialInstructions": string
      }
    ],
    "customer": {
      "id": integer,
      "fullName": string,
      "email": string,
      "phone": string
    }
  }
}
```

**Order Status Values:**
- `1` = Pending
- `2` = Confirmed
- `3` = Preparing
- `4` = OutForDelivery
- `5` = Completed
- `6` = Cancelled

**Error Responses:**
- `400 Bad Request`: Cart is empty
- `400 Bad Request`: Food item unavailable
- `400 Bad Request`: Insufficient stock

**Edge Cases Handled:**
- Cart emptiness check
- All items validated for availability
- All items validated for stock
- Stock reserved immediately
- Cart cleared after order
- Unique order number generated (CK + timestamp + random)

---

### 18. Get Order by ID
**Endpoint:** `GET /api/v1/order/{orderId}`

**Description:** Get specific order (user's own orders only).

**Success Response:** `200 OK`
```json
{
  "success": true,
  "message": "Operation successful",
  "data": {
    // Full order details
  }
}
```

**Error Response:** `404 Not Found`

---

### 19. Get Order by Number
**Endpoint:** `GET /api/v1/order/number/{orderNumber}`

**Description:** Get order by unique order number.

**Success Response:** `200 OK`

---

### 20. Get User Orders
**Endpoint:** `GET /api/v1/order/user`

**Description:** Get all orders for logged-in user.

**Headers:**
```
userId: 1
```

**Success Response:** `200 OK`
```json
{
  "success": true,
  "message": "Operation successful",
  "data": [
    // Orders sorted by CreatedAt DESC
  ]
}
```

---

### 21. Get All Orders
**Endpoint:** `GET /api/v1/order/all`

**Description:** Get all orders in system (Admin only).

**Success Response:** `200 OK`

---

### 22. Update Order Status
**Endpoint:** `PUT /api/v1/order/{orderId}/status`

**Description:** Update order status (Admin only).

**Request Body:**
```json
{
  "newStatus": 2,
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

**Success Response:** `200 OK`

**Error Responses:**
- `400 Bad Request`: Invalid status transition
  ```json
  {
    "success": false,
    "message": "Cannot transition from 'Completed' to 'Pending'. Valid transitions are: ...",
    "errors": []
  }
  ```

**Edge Cases Handled:**
- Valid status transitions enforced:
  - Pending → Confirmed, Cancelled
  - Confirmed → Preparing, Cancelled
  - Preparing → OutForDelivery
  - OutForDelivery → Completed
- Automatic timestamp updates
- Stock restoration on cancellation

---

### 23. Cancel Order
**Endpoint:** `POST /api/v1/order/{orderId}/cancel`

**Description:** Cancel order (customer or admin).

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

**Success Response:** `200 OK`
```json
{
  "success": true,
  "message": "Order cancelled successfully",
  "data": {
    // Updated order with status=Cancelled
  }
}
```

**Error Responses:**
- `400 Bad Request`: Cannot cancel order in current status
  ```json
  {
    "success": false,
    "message": "Only Pending or Confirmed orders can be cancelled",
    "errors": []
  }
  ```

**Edge Cases Handled:**
- Only Pending/Confirmed orders can be cancelled
- Stock automatically restored
- Cancellation reason recorded
- Timestamp for CancelledAt updated

---

## User APIs

### 24. Get Current User
**Endpoint:** `GET /api/v1/user/me`

**Description:** Get current user profile.

**Success Response:** `200 OK`
```json
{
  "success": true,
  "message": "Operation successful",
  "data": {
    "id": 1,
    "email": "customer@example.com",
    "phone": "+2348012345678",
    "firstName": "John",
    "lastName": "Doe",
    "fullName": "John Doe",
    "isVerified": true,
    "role": 1,
    "roleDisplay": "Customer",
    "referralCode": "CK-ABC123",
    "referralCount": 5,
    "address": "123 Main Street, Lagos",
    "createdAt": "2025-02-15T10:00:00Z"
  }
}
```

---

### 25. Get User by Referral Code
**Endpoint:** `GET /api/v1/user/by-referral-code/{code}`

**Description:** Lookup referral code to get referrer info.

**Success Response:** `200 OK`
```json
{
  "success": true,
  "message": "Referral code found",
  "data": {
    "id": 1,
    "fullName": "Jane Doe",
    "referralCode": "CK-XYZ789"
  }
}
```

---

## Error Response Format

All error responses follow this format:

```json
{
  "success": false,
  "message": "Error message describing what went wrong",
  "errors": [
    "Detailed error 1",
    "Detailed error 2"
  ]
}
```

### Error Scenarios

**Development Mode (detailed errors):**
```json
{
  "success": false,
  "message": "Object reference not set to an instance of an object.",
  "data": null,
  "errors": [
    "Type: NullReferenceException",
    "StackTrace: at ChuksKitchen.API.Controllers.AuthController.Register..."
  ]
}
```

**Production Mode (secure errors):**
```json
{
  "success": false,
  "message": "An internal server error occurred. Please try again later.",
  "data": null,
  "errors": []
}
```

---

## HTTP Status Codes

| Code | Meaning | Usage |
|------|---------|-------|
| 200 | OK | Successful GET, PUT, DELETE |
| 201 | Created | Successful POST (resource created) |
| 400 | Bad Request | Invalid input data or business rule violation |
| 401 | Unauthorized | Missing/invalid authentication |
| 404 | Not Found | Resource not found |
| 500 | Internal Server Error | Server error |

---

## Professional Enhancements

This API includes the following production-ready features:

### API Versioning
- All endpoints are versioned (v1.0)
- Allows future breaking changes without affecting existing clients
- Example: `/api/v1/auth/register`, `/api/v1/food/available`
- Swagger UI automatically configured for versioned endpoints

### Global Exception Handler
- Centralized error handling middleware
- Consistent error response format across all endpoints
- Development mode shows detailed errors with stack traces
- Production mode shows secure, generic error messages
- Automatic HTTP status code mapping based on exception type

**Exception Mappings:**
| Exception Type | HTTP Status | Example |
|---------------|-------------|---------|
| UnauthorizedAccessException | 401 | Invalid/missing JWT token |
| ArgumentNullException | 400 | Required parameter is null |
| ArgumentException | 400 | Invalid parameter value |
| InvalidOperationException | 400 | Invalid operation state |
| KeyNotFoundException | 404 | Resource not found |
| All other exceptions | 500 | Unexpected errors |

---

## Testing with Swagger

Navigate to `http://localhost:5000/swagger` to access interactive API documentation.

### Using Swagger UI:

1. **Expand any endpoint** - Click to see details
2. **Click "Try it out"** - Enables the test interface
3. **Fill in required parameters** - Enter test data
4. **Click "Execute"** - Sends the request
5. **View response** - See the API response below
6. **Check response schema** - Scroll down to see data structure

### Example Test Flow:

1. **Register a user**
   - POST `/api/v1/auth/register`
   - Enter test data
   - Execute and save the returned user ID

2. **Browse available food**
   - GET `/api/v1/food/available`
   - No parameters needed
   - Note down a food item ID

3. **Add to cart**
   - POST `/api/v1/cart/add`
   - Use the user ID in headers
   - Use the food item ID from step 2

4. **Place order**
   - POST `/api/v1/order`
   - Use the user ID in headers
   - Enter delivery address

---

## Best Practices

### For Developers:
1. Always handle error responses
2. Check `success` field before accessing `data`
3. Use appropriate HTTP methods (GET, POST, PUT, DELETE)
4. Include userId header for authenticated endpoints
5. Validate inputs before sending to API

### For Integration:
1. Implement retry logic for 500 errors
2. Cache food items for better performance
3. Handle network timeouts gracefully
4. Log all API interactions for debugging
5. Use exponential backoff for retries

---

**Last Updated:** February 17, 2026
**API Version:** v1.0
**Documentation Version:** 2.0
