# 💾 CHUKSKITCHEN DATA MODEL

## Entity Relationship Diagram (ERD)

```
┌─────────────────────┐
│      USER           │
│─────────────────────┘
│ PK Id               │
│    FK ReferrerId ───────────────────┐
│    Email (unique)   │                  │
│    Phone (unique)   │                  │
│    PasswordHash     │                  │
│    ReferralCode     │                  │
│    Role             │                  │
└─────────┬───────────┘                  │
          │                             │
          │                             │
          │ 1                           │ 1
    ┌─────▼──────┐              ┌──────▼───────┐
    │  FOODITEM │              │   CART       │
    └─────┬─────┘              └──────┬───────┘
          │                             │
          │                             │
          │ N                           │ 1
    ┌─────▼──────────┐              │ 1
    │  CARTITEM      │              ┌───▼────────┐
    │               │              │ ORDERITEM  │
    └───────┬───────┘              └────┬───────┘
            │                             │
            │ N                           │ N
      ┌─────▼─────────┐               ┌───▼──────┐
      │    ORDER      │               │  ORDER   │
      └──────────────┘               │ (Side)   │
                                     └─────────┘
```

---

## 🔗 RELATIONSHIPS

### One-to-Many (1:N)

**User → FoodItem**
- One user (admin) can create multiple food items
- Relationship: `FoodItem.AddedByAdminId → User.Id`

**User → Cart**
- One user has one active cart
- Relationship: `Cart.UserId → User.Id`

**User → Order**
- One user can place multiple orders
- Relationship: `Order.UserId → User.Id`

**User → Referrals**
- One user can refer multiple other users
- Relationship: `User.ReferrerId → User.Id` (self-referencing)

**Cart → CartItem**
- One cart contains multiple items
- Relationship: `CartItem.CartId → Cart.Id`

**Order → OrderItem**
- One order contains multiple items
- Relationship: `OrderItem.OrderId → Order.Id`

**FoodItem → CartItem**
- One food item can be in multiple carts
- Relationship: `CartItem.FoodItemId → FoodItem.Id`

**FoodItem → OrderItem**
- One food item can be in multiple orders
- Relationship: `OrderItem.FoodItemId → FoodItem.Id`

---

## 📊 TABLE STRUCTURES

### 1. Users

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | int | PK, Auto-increment | Unique identifier |
| Email | string(256) | NOT NULL, Unique | User's email address |
| Phone | string(20) | Unique | User's phone number |
| PasswordHash | string(512) | NOT NULL | BCrypt encrypted password |
| FirstName | string(100) | NOT NULL | First name |
| LastName | string(100) | NOT NULL | Last name |
| ReferralCode | string(20) | NOT NULL, Unique | Auto-generated referral code |
| ReferrerId | int? | FK → Users.Id | ID of user who referred this user |
| IsVerified | bool | Default: false | Account verification status |
| OtpCode | string(10) | | OTP for verification |
| OtpExpiry | datetime? | | OTP expiration timestamp |
| OtpGeneratedAt | datetime? | | When OTP was generated |
| FailedOtpAttempts | int | Default: 0 | Number of failed OTP attempts |
| Address | string(500) | | Delivery address |
| Role | int | NOT NULL | 1=Customer, 2=Admin |
| RegistrationMethod | int | NOT NULL | 1=Email, 2=Phone |
| CreatedAt | datetime | Auto | Creation timestamp |
| UpdatedAt | datetime | Auto | Last update timestamp |

---

### 2. FoodItems

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | int | PK, Auto-increment | Unique identifier |
| Name | string(100) | NOT NULL | Food name |
| Description | string(1000) | NOT NULL | Food description |
| Price | decimal(18,2) | NOT NULL | Price in Naira |
| ImageUrl | string | | URL to food image |
| Category | string(50) | NOT NULL | Food category |
| IsAvailable | bool | Default: true | Availability status |
| StockQuantity | int | Default: 100 | Available stock |
| PreparationTimeMinutes | int | Default: 15 | Preparation time |
| SpiceLevel | string(20) | | Spice level |
| AddedByAdminId | int | FK → Users.Id | Admin who added this |
| CreatedAt | datetime | Auto | Creation timestamp |
| UpdatedAt | datetime | Auto | Last update timestamp |

---

### 3. Carts

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | int | PK, Auto-increment | Unique identifier |
| UserId | int | FK → Users.Id, NOT NULL | Cart owner |
| CreatedAt | datetime | Auto | Creation timestamp |
| UpdatedAt | datetime | Auto | Last update timestamp |

---

### 4. CartItems

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | int | PK, Auto-increment | Unique identifier |
| CartId | int | FK → Carts.Id, NOT NULL | Parent cart |
| FoodItemId | int | FK → FoodItems.Id, NOT NULL | Food item |
| Quantity | int | NOT NULL | Number of items |
| UnitPrice | decimal(18,2) | NOT NULL | Price at time of adding |
| SpecialInstructions | string | | Special requests |
| CreatedAt | datetime | Auto | Creation timestamp |
| UpdatedAt | datetime | Auto | Last update timestamp |

---

### 5. Orders

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | int | PK, Auto-increment | Unique identifier |
| OrderNumber | string | NOT NULL, Unique | Order number (CK+timestamp) |
| UserId | int | FK → Users.Id, NOT NULL | Customer who placed order |
| Status | int | NOT NULL | 1=Pending, 2=Confirmed, 3=Preparing, 4=OutForDelivery, 5=Completed, 6=Cancelled |
| TotalAmount | decimal(18,2) | NOT NULL | Total including delivery fee |
| DeliveryFee | decimal(18,2) | NOT NULL | Delivery charge (₦500) |
| DeliveryAddress | string(500) | NOT NULL | Delivery location |
| SpecialInstructions | string | | Customer notes |
| IsPaid | bool | Default: false | Payment status |
| ConfirmedAt | datetime? | | When confirmed |
| PreparingAt | datetime? | | When preparation started |
| OutForDeliveryAt | datetime? | | When rider picked up |
| CompletedAt | datetime? | | When delivered |
| CancelledAt | datetime? | | When cancelled |
| CancellationReason | string | | Reason for cancellation |
| CreatedAt | datetime | Auto | Creation timestamp |
| UpdatedAt | datetime | Auto | Last update timestamp |

---

### 6. OrderItems

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | int | PK, Auto-increment | Unique identifier |
| OrderId | int | FK → Orders.Id, NOT NULL | Parent order |
| FoodItemId | int | FK → FoodItems.Id, NOT NULL | Food item |
| Quantity | int | NOT NULL | Number of items |
| UnitPrice | decimal(18,2) | NOT NULL | Price at time of order |
| SpecialInstructions | string | | Special requests |

---

## 📐 SCALING CONSIDERATIONS

### For 100 Users:
- InMemory database sufficient
- Single API instance
- No caching needed
- Response time: < 100ms

### For 1,000 Users:
- Upgrade to PostgreSQL or MySQL
- Add Redis caching for frequently accessed data
- Add database indexes on Email, Phone, OrderNumber
- Response time: < 200ms

### For 10,000+ Users:
- **Database:** PostgreSQL with read replicas
- **Caching:** Redis cluster for session and data
- **Load Balancing:** Multiple API instances behind nginx
- **CDN:** CloudFront or Cloudflare for static assets
- **Monitoring:** Application Insights or Datadog
- **Horizontal Scaling:** Stateless API for easy scaling
- **Queue System:** RabbitMQ for async order processing
- **Database Sharding:** Split orders by date range
- **Response Time:** < 500ms (SLA)

---

## 🎯 KEY DESIGN DECISIONS

1. **ReferralCode uniqueness** - Ensures each user gets shareable code
2. **OrderNumber format** - CK prefix + timestamp for uniqueness
3. **Price snapshot** - UnitPrice saved at order time (protects against price changes)
4. **Stock management** - Automatic deduction on order, restoration on cancellation
5. **OTP expiry** - 10-minute validity for security
6. **Status transitions** - Enforced valid state changes
7. **Self-referencing User** - Allows tracking referral tree

---

## 📊 ENTITY ENUMS

### UserRole
```csharp
Customer = 1
Admin = 2
```

### OrderStatus
```csharp
Pending = 1
Confirmed = 2
Preparing = 3
OutForDelivery = 4
Completed = 5
Cancelled = 6
```

### RegistrationMethod
```csharp
Email = 1
Phone = 2
```

---

## 🔍 INDEXING STRATEGY (For PostgreSQL Migration)

```sql
CREATE INDEX idx_users_email ON Users(Email);
CREATE INDEX idx_users_phone ON Users(Phone);
CREATE INDEX idx_users_referral_code ON Users(ReferralCode);
CREATE INDEX idx_orders_number ON Orders(OrderNumber);
CREATE INDEX idx_orders_user_id ON Orders(UserId);
CREATE INDEX idx_orders_status ON Orders(Status);
CREATE INDEX idx_orders_created_at ON Orders(CreatedAt);
CREATE INDEX idx_food_items_category ON FoodItems(Category);
CREATE INDEX idx_food_items_available ON FoodItems(IsAvailable);
```

---

**Total Entities:** 6
**Total Relationships:** 7
**Database Size:** ~5MB (InMemory for 100 users)
