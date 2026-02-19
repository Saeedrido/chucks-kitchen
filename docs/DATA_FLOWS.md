# Data Flows & Business Logic

This document explains how data flows through the Chuks Kitchen system.

---

## 1. User Registration & Verification Flow

```
Frontend → Controller → AuthService → Repository → Database
                         ↓
                    Business Logic:
                    - Validate duplicates
                    - Validate referral
                    - Generate OTP
                    - Hash password
```

### Steps:

1. **Controller** receives registration request
2. **AuthService** validates input:
   - Check for duplicate email/phone
   - Validate referral code (if provided)
   - Hash password with BCrypt
3. **Create User entity** with:
   - 6-digit OTP
   - OTP expiry (10 minutes)
   - Unique referral code (CK-XXXXXX)
4. **Save to database**
5. **Return user data** (OTP logged to console in dev)

### Business Rules:
- Email OR Phone required (not both)
- Password min 6 characters
- Referral code must be valid (optional)
- Account unverified until OTP confirmed

---

## 2. User Login Flow

```
Frontend → Controller → AuthService → Repository → Database
                         ↓
                    Business Logic:
                    - Find user by email/phone
                    - Verify password hash
                    - Check if verified
                    - Generate JWT token
```

### Steps:

1. **Controller** receives login request
2. **AuthService** validates credentials:
   - Find user by email OR phone
   - Compare password hash (BCrypt)
   - Check if account is verified
3. **Generate JWT token** if valid
4. **Return token + user data**

### Business Rules:
- Email OR Phone accepted
- Account must be verified first
- Token expires in 24 hours
- Wrong password = clear error message

---

## 3. Food Browsing Flow

```
Frontend → Controller → FoodService → Repository → Database
                         ↓
                    Business Logic:
                    - Get all or available items
                    - Filter by category
                    - Check stock/availability
```

### Steps:

1. **Controller** receives request
2. **FoodService** processes:
   - Admin: Get all items (including unavailable)
   - Customer: Get only available items
   - Optional: Filter by category
3. **Return food list** with prices and availability

### Business Rules:
- Admins see all items
- Customers see only available items
- Category filtering optional
- Prices shown as numbers (no currency symbol)

---

## 4. Cart Management Flow

```
Frontend → Controller → CartService → Repository → Database
                         ↓
                    Business Logic:
                    - Get/create user's cart
                    - Add/Update/Remove items
                    - Validate availability
                    - Check stock quantity
```

### Steps:

### Adding to Cart:
1. **Controller** receives add request
2. **CartService** validates:
   - Food item exists
   - Item is available
   - Sufficient stock quantity
3. **Add to cart** (or update if exists)
4. **Return updated cart**

### Updating Cart:
1. **Validate new quantity** (> 0)
2. **Update cart item**
3. **Recalculate totals**

### Business Rules:
- One cart per user
- Duplicate items merge (quantity increases)
- Stock validated on each add
- Special instructions preserved

---

## 5. Order Placement Flow

```
Frontend → Controller → OrderService → Repository → Database
                         ↓
                    Business Logic:
                    - Validate cart not empty
                    - Validate all items available
                    - Reserve stock
                    - Calculate total
                    - Generate unique order number
                    - Clear cart
```

### Steps:

1. **Controller** receives order request
2. **OrderService** validates:
   - Cart has items
   - All items still available
   - Sufficient stock for all items
3. **Reserve stock** immediately
4. **Calculate total:**
   - Sum of item prices
   - Add delivery fee (₦500)
5. **Generate order number:**
   - Format: CK + timestamp + 6-digit random
   - Example: CK20260220123456789
6. **Create order** with status "Pending"
7. **Clear user's cart**
8. **Return order details**

### Business Rules:
- Delivery fee: ₦500 (fixed)
- Stock reserved on order creation
- Cart cleared after successful order
- Unique order number guaranteed
- Initial status: Pending

---

## 6. Order Status Updates Flow

```
Frontend → Controller → OrderService → Repository → Database
                         ↓
                    Business Logic:
                    - Validate status transition
                    - Update status timestamp
                    - Restore stock if cancelled
```

### Valid Transitions:
```
Pending → Confirmed → Preparing → OutForDelivery → Completed
                     ↓
                  Cancelled
```

### Steps:

1. **Controller** receives status update
2. **OrderService** validates:
   - Current status allows transition
   - New status is valid
3. **Update status**
4. **Set timestamp** for new status
5. **If cancelled:** Restore stock quantities
6. **Return updated order**

### Business Rules:
- Status cannot go backward (except cancel)
- Completed orders cannot be changed
- Cancellation restores stock
- Each status has timestamp

---

## 7. Order Cancellation Flow

```
Frontend → Controller → OrderService → Repository → Database
                         ↓
                    Business Logic:
                    - Validate order can be cancelled
                    - Update status to Cancelled
                    - Restore stock
                    - Record cancellation reason
```

### Steps:

1. **Controller** receives cancellation request
2. **OrderService** validates:
   - Order belongs to user (customer)
   - Status allows cancellation
3. **Update status** to "Cancelled"
4. **Restore all stock** quantities
5. **Record cancellation reason**
6. **Return confirmation**

### Business Rules:
- **Customer can cancel:** Pending, Confirmed
- **Admin can cancel:** Any status (except Completed)
- Stock always restored on cancellation
- Reason required for cancellation

---

## 8. Admin Food Management Flow

```
Frontend → Controller → FoodService → Repository → Database
                         ↓
                    Business Logic:
                    - Validate admin rights
                    - Create/Update/Delete food
                    - Track admin who made changes
```

### Adding Food:
1. Validate admin permissions
2. Validate food data (name, price, category)
3. Create food item with admin ID
4. Return created item

### Updating Food:
1. Validate admin permissions
2. Validate food exists
3. Update allowed fields
4. Track updated timestamp

### Deleting Food:
1. Validate admin permissions
2. Soft delete (set IsDeleted = true)
3. Preserve historical data

### Business Rules:
- Only admins can modify food
- Admin ID tracked on all changes
- Soft delete (data preserved)
- Price updates affect future orders only

---

## 9. Referral System Flow

```
Registration → Validate Referral → Link Users → Record Referral
                    ↓
                Business Logic:
                - Check referrer exists
                - Link referrer to new user
                - Store referral relationship
```

### Steps:

1. **User registers** with referral code
2. **AuthService** validates:
   - Referral code exists in database
   - Referrer is verified user
3. **Link referrer** to new user
4. **Save relationship** in database

### Business Rules:
- Referral code = Referrer's email
- Optional (registration succeeds without it)
- Referrer must be verified user
- One-way relationship (referrer → referee)

---

## 10. OTP Management Flow

```
Registration/Login → Generate OTP → Store OTP → User Verifies → Check OTP
                                                   ↓
                                              Business Logic:
                                              - Match OTP code
                                              - Check expiry
                                              - Track attempts
                                              - Lock after 3 failures
```

### Generating OTP:
1. Generate 6-digit random number
2. Set expiry (UTC + 10 minutes)
3. Store in user record
4. Log to console (dev mode)

### Verifying OTP:
1. Find user by email/phone
2. Check OTP not expired
3. Compare OTP codes
4. Track failed attempts
5. Lock account after 3 failures
6. Clear OTP on success

### Business Rules:
- OTP expires: 10 minutes
- Max failed attempts: 3
- Account locked after failures
- New OTP required to unlock

---

## Architecture Layers

```
┌─────────────────────────────────────────────┐
│              API Layer                      │
│  - Controllers (HTTP in/out)                │
│  - No business logic                        │
└─────────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────────┐
│           Application Layer                 │
│  - Services (business logic)                │
│  - DTOs (data transfer)                     │
│  - Validation rules                         │
└─────────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────────┐
│          Persistence Layer                  │
│  - Repositories (data access)               │
│  - DbContext (EF Core)                      │
└─────────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────────┐
│            Database Layer                   │
│  - InMemory Database                        │
│  - Entities & Tables                        │
└─────────────────────────────────────────────┘
```

---

## Key Principles

1. **Separation of Concerns:**
   - Controllers handle HTTP only
   - Services handle business logic
   - Repositories handle data access

2. **Validation:**
   - Input validation at controller
   - Business rules in service
   - Database constraints

3. **Error Handling:**
   - Global exception handler
   - Consistent error responses
   - User-friendly messages

4. **Data Integrity:**
   - Stock management
   - Transaction isolation
   - Soft delete for history

---

**For detailed API documentation, see:** `API_DOCUMENTATION.md`
