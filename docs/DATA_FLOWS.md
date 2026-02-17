# Data Flows & Business Logic

This document explains how data flows through the Chuks Kitchen system.

## 1. User Registration & Verification Flow

```
┌─────────┐    ┌─────────┐    ┌──────────┐    ┌─────────┐    ┌─────────┐
│Frontend │───▶│Controller│───▶│AuthService│───▶│Repository│───▶│ Database │
└─────────┘    └─────────┘    └──────────┘    └─────────┘    └─────────┘
                      │                │
                      │                │
                      ▼                ▼
                  Response        Business Logic
                                  - Validate duplicates
                                  - Validate referral
                                  - Generate OTP
                                  - Hash password
```

### Step-by-Step Process:

**1. Controller Layer:**
- Receives `RegisterRequestDto`
- Calls `AuthService.RegisterAsync()`
- Returns `ResponseDto<UserResponseDto>`

**2. Service Layer (Business Logic):**
- Check for duplicate email/phone
- Validate referral code (if provided)
- Hash password using SHA256
- Create User entity
- Generate 6-digit OTP
- Set OTP expiry (10 minutes)
- Save to database via Repository
- Return UserResponseDto

**3. Repository Layer:**
- Execute database queries
- Save changes
- Return entity

**4. Database:**
- Insert User record
- Return created record

### Business Rules:
- ✅ Email must be unique
- ✅ Phone must be unique
- ✅ Referral code must be existing user's email
- ✅ Password hashed before storage
- ✅ OTP valid for 10 minutes
- ✅ Account locks after 3 failed OTP attempts

---

## 2. Food Browsing Flow

```
┌─────────┐    ┌─────────┐    ┌──────────┐    ┌─────────┐    ┌─────────┐
│Frontend │───▶│Controller│───▶│FoodService│───▶│Repository│───▶│ Database │
└─────────┘    └─────────┘    └──────────┘    └─────────┘    └─────────┘
                      │
                      ▼
                  Response
                  - Filter available items
                  - Calculate prices
```

### Step-by-Step Process:

**1. Controller Layer:**
- Receives GET request
- Calls `FoodService.GetAvailableFoodItemsAsync()`

**2. Service Layer (Business Logic):**
- Query food items from repository
- Filter: `IsAvailable = true AND StockQuantity > 0`
- Sort by Category, then Name
- Map to FoodItemResponseDto
- Return list

**3. Data Required by Frontend:**
- Food ID, Name, Description
- Price, Image URL
- Category, Spice Level
- Preparation Time, Stock Quantity
- Availability status

### Business Rules:
- ✅ Only show available items
- ✅ Only show items with stock > 0
- ✅ Organized by category

---

## 3. Add to Cart Flow

```
┌─────────┐    ┌─────────┐    ┌──────────┐    ┌─────────┐    ┌─────────┐
│Frontend │───▶│Controller│───▶│CartService│───▶│Repository│───▶│ Database │
└─────────┘    └─────────┘    └──────────┘    └─────────┘    └─────────┘
                      │                │
                      │                │
                      ▼                ▼
                  Response        Business Logic
                                  - Check food availability
                                  - Validate stock
                                  - Create or get cart
                                  - Handle duplicates
```

### Step-by-Step Process:

**1. Controller Layer:**
- Receives `AddToCartRequestDto` + userId header
- Calls `CartService.AddToCartAsync()`

**2. Service Layer (Business Logic):**
- Get user's cart (create if doesn't exist)
- Get food item
- **Check availability**: `IsAvailable = true`
- **Check stock**: `StockQuantity >= requestedQuantity`
- **Check for duplicate**: Item already in cart?
  - Yes: Update quantity (validate new total)
  - No: Add new cart item
- Save changes
- Return updated cart

**3. Data Required by Frontend:**
- Cart ID
- Cart items with totals
- Subtotal, Delivery fee, Total
- Food item details

### Business Rules:
- ✅ Food must be available
- ✅ Sufficient stock required
- ✅ Quantity must be > 0
- ✅ Duplicates merge quantities
- ✅ Stock validated for merged quantity

### Edge Cases:
1. **Food became unavailable after adding to cart**
   - Detected on update/delete
   - Error message returned

2. **Stock depleted after adding to cart**
   - Detected on checkout
   - User informed to update cart

---

## 4. Order Placement Flow

```
┌─────────┐    ┌─────────┐    ┌──────────┐    ┌─────────┐    ┌─────────┐
│Frontend │───▶│Controller│───▶│OrderService│───▶│Repository│───▶│ Database │
└─────────┘    └─────────┘    └──────────┘    └─────────┘    └─────────┘
                      │                │
                      │                │
                      ▼                ▼
                  Response        Business Logic
                                  - Validate cart
                                  - Check all items
                                  - Generate order number
                                  - Reserve stock
                                  - Clear cart
```

### Step-by-Step Process:

**1. Controller Layer:**
- Receives `CreateOrderRequestDto` + userId header
- Calls `OrderService.CreateOrderAsync()`

**2. Service Layer (Business Logic):**
- Get user's cart
- **Validate**: Cart not empty
- **Validate all items**:
  - Each item still available?
  - Sufficient stock for each?
- Calculate totals: Subtotal + Delivery fee (₦500)
- **Generate order number**: `CK + timestamp + random`
- Create Order entity (Status: Pending)
- Create OrderItem entities from CartItems
- **Update stock**: Reduce stock for each item
- Save Order and OrderItems
- **Clear cart**: Remove all items from cart
- Return OrderResponseDto

**3. Data Required by Frontend:**
- Order ID, Order Number
- Status, Status Display
- Items with details
- Totals (Subtotal, Delivery, Total)
- Timestamps (Created, Confirmed, etc.)

### Business Rules:
- ✅ Cart cannot be empty
- ✅ All items must be available
- ✅ All items must have sufficient stock
- ✅ Stock reserved immediately
- ✅ Cart cleared after order
- ✅ Unique order number generated

### Order Number Format:
```
CK202502151530451234
│  │              │    │
│  │              │    └─ Random 4 digits
│  │              └────── Timestamp
│  └─────────────────── Date
└────────────────────── Prefix (Chuks Kitchen)
```

---

## 5. Order Status Update Flow (Admin)

```
┌─────────┐    ┌─────────┐    ┌──────────┐    ┌─────────┐    ┌─────────┐
│Frontend │───▶│Controller│───▶│OrderService│───▶│Repository│───▶│ Database │
└─────────┘    └─────────┘    └──────────┘    └─────────┘    └─────────┘
                      │                │
                      │                │
                      ▼                ▼
                  Response        Business Logic
                                  - Validate transition
                                  - Update timestamp
                                  - Restore stock (if cancelled)
```

### Step-by-Step Process:

**1. Controller Layer:**
- Receives `UpdateOrderStatusRequestDto`
- Calls `OrderService.UpdateOrderStatusAsync()`

**2. Service Layer (Business Logic):**
- Get order with items
- **Validate status transition**:
  ```
  Pending → Confirmed, Cancelled
  Confirmed → Preparing, Cancelled
  Preparing → OutForDelivery
  OutForDelivery → Completed
  ```
- Update order status
- **Update appropriate timestamp**:
  - Confirmed → `ConfirmedAt`
  - Preparing → `PreparingAt`
  - OutForDelivery → `OutForDeliveryAt`
  - Completed → `CompletedAt`
  - Cancelled → `CancelledAt`, set reason
- **If cancelled**: Restore stock for all items
- Save changes
- Return updated order

### Valid Transitions:
```
        ┌──────────┐
        │ Pending  │
        └────┬─────┘
             │
      ┌────┴────┐
      ▼         ▼
   Confirmed   Cancelled
      │
      ▼
  Preparing
      │
      ▼
OutForDelivery
      │
      ▼
   Completed
```

### Business Rules:
- ✅ Only valid transitions allowed
- ✅ Automatic timestamp tracking
- ✅ Stock restored on cancellation
- ✅ Cancellation reason required

---

## 6. Order Cancellation Flow (Customer)

```
┌─────────┐    ┌─────────┐    ┌──────────┐    ┌─────────┐    ┌─────────┐
│Frontend │───▶│Controller│───▶│OrderService│───▶│Repository│───▶│ Database │
└─────────┘    └─────────┘    └──────────┘    └─────────┘    └─────────┘
                      │                │
                      ▼                ▼
                  Response        Business Logic
                                  - Check status
                                  - Validate permission
                                  - Restore stock
                                  - Record reason
```

### Step-by-Step Process:

**1. Controller Layer:**
- Receives cancellation request + userId
- Calls `OrderService.CancelOrderAsync()`

**2. Service Layer (Business Logic):**
- Get order (must belong to user)
- **Validate status**: Must be Pending or Confirmed
- **If Preparing or later**: Reject cancellation
- **Restore stock**:
  - For each OrderItem
  - Find FoodItem
  - Add quantity back to StockQuantity
  - Save
- Update order: Status = Cancelled
- Set timestamps and reason
- Save changes
- Return updated order

### Business Rules:
- ✅ Only Pending/Confirmed can be cancelled
- ✅ User can only cancel own orders
- ✅ Stock automatically restored
- ✅ Cancellation reason recorded

---

## Data Requirements by Screen

### 1. Login Screen
**Required Data:**
- Email or Phone
- Password

**Backend Provides:**
- User validation
- Verification status check
- JWT token (future)

---

### 2. Food Browse Screen
**Required Data:**
- List of food items
- Filters (category, availability)

**Backend Provides:**
- FoodItemResponseDto[]
- ID, Name, Description
- Price, Image, Category
- Availability, Stock

---

### 3. Cart Screen
**Required Data:**
- Cart items
- Item quantities
- Totals

**Backend Provides:**
- CartResponseDto
- Items with food details
- Subtotal, Delivery fee, Total
- Item availability status

---

### 4. Order History Screen
**Required Data:**
- User's orders
- Order statuses
- Order details

**Backend Provides:**
- OrderResponseDto[]
- Order number, status, dates
- Items, totals
- Cancellation info (if applicable)

---

### 5. Order Details Screen
**Required Data:**
- Full order details
- Items with quantities
- Status timeline
- Delivery info

**Backend Provides:**
- OrderResponseDto
- All timestamps
- Customer info (admin)
- Cancellation reason

---

### 6. Admin Food Management Screen
**Required Data:**
- All food items
- Add/Edit forms

**Backend Provides:**
- FoodItemResponseDto[] (all items)
- CRUD operations
- Availability toggle
- Stock management

---

### 7. Admin Order Management Screen
**Required Data:**
- All orders
- Status updates

**Backend Provides:**
- OrderResponseDto[] (all orders)
- Status update endpoint
- Customer information

---

## API Communication Pattern

### Request Format:
```
POST /api/endpoint
Headers: {
  "Content-Type": "application/json",
  "userId": "1"  // or JWT in production
}
Body: {
  // Request DTO fields
}
```

### Response Format:
```
{
  "success": true/false,
  "message": "Human readable message",
  "data": { /* Response DTO */ },
  "errors": [/* Validation errors */]
}
```

### Error Handling:
- Controllers catch exceptions
- Return consistent error format
- Log errors for debugging
- Business logic returns meaningful messages
