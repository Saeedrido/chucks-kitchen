# Flow Diagrams - Chuks Kitchen

This document describes the flow diagrams included in the `/Diagrams/` folder.

---

## Diagram 1: User Registration & Verification Flow

**Location:** `/Diagrams/01-User-Registration-Verification-Flow.pdf`

**Process:**
1. Customer enters registration details (Email/Phone, Password, Name, Referral Code)
2. System validates:
   - Email/Phone not duplicate
   - Referral code valid (if provided)
3. System generates:
   - Unique referral code (CK-XXXXXX)
   - 6-digit OTP
   - Password hash (BCrypt)
4. User account created (unverified)
5. Customer enters OTP to verify
6. System validates OTP:
   - Check not expired (10 min)
   - Track failed attempts (max 3)
7. Account verified → Customer can login

**Key Decisions:**
- OTP expires after 10 minutes (security)
- Account locks after 3 failed attempts (prevent brute force)
- Referral code optional (viral marketing)

---

## Diagram 2: Food Browsing Flow

**Location:** `/Diagrams/02-Food-Browsing-Flow.pdf`

**Process:**
1. Customer browses food menu
2. System displays available food items
3. Customer can filter by:
   - Category (Rice, Swallow, Soup, Drinks)
   - Spice level
   - Price range
4. System checks:
   - Food availability
   - Stock quantity
5. Customer views food details
6. Add to cart or continue browsing

**Key Decisions:**
- Customers see only available items
- Admins see all items (including unavailable)
- Category filtering for easy navigation

---

## Diagram 3: Cart & Order Placement Flow

**Location:** `/Diagrams/03-Cart-Order-Placement-Flow.pdf`

**Process:**

**Adding to Cart:**
1. Customer selects food item
2. System validates:
   - Item is available
   - Sufficient stock
3. Add to cart (or update if exists)
4. Show cart total

**Placing Order:**
1. Customer reviews cart
2. Enters delivery address
3. Clicks "Place Order"
4. System validates:
   - Cart not empty
   - All items still available
   - Sufficient stock
5. System:
   - Reserves stock
   - Calculates total (items + ₦500 delivery)
   - Generates unique order number (CK + timestamp + random)
   - Creates order (status: Pending)
   - Clears cart
6. Returns order confirmation

**Key Decisions:**
- Stock reserved immediately (prevent overselling)
- Fixed delivery fee (₦500)
- Unique order number for tracking
- Cart cleared after order

---

## Diagram 4: Order Status Lifecycle

**Location:** `/Diagrams/04-Order-Status-Lifecycle.pdf`

**Status Flow:**
```
Pending → Confirmed → Preparing → OutForDelivery → Completed
                     ↓
                  Cancelled
```

**Process:**
1. **Pending** - Order created, awaiting confirmation
2. **Confirmed** - Admin confirmed, preparing to cook
3. **Preparing** - Food is being prepared
4. **OutForDelivery** - Food is with delivery person
5. **Completed** - Order delivered successfully
6. **Cancelled** - Order cancelled (by customer or admin)

**Rules:**
- Status cannot go backward (except cancellation)
- Customer can cancel: Pending, Confirmed
- Admin can cancel: Any status (except Completed)
- Stock restored on cancellation
- Each status has timestamp

**Key Decisions:**
- Clear status progression
- Cancellation allowed early in flow
- Timestamp tracking for analytics

---

## Diagram 5: Admin Management Flow

**Location:** `/Diagrams/05-Admin-Management-Flow.pdf`

**Food Management:**
1. Admin logs in
2. Can:
   - Add new food item
   - Update food (price, description, availability)
   - Delete food (soft delete)
   - View all food (including unavailable)

**Order Management:**
1. Admin views all orders
2. Can:
   - Update order status
   - Cancel any order
   - View order details
   - Track order progress

**Key Decisions:**
- Soft delete (preserve data)
- Admin can override customer actions
- Full visibility of all data

---

## Diagram 6: Edge Case Handling

**Location:** `/Diagrams/06-Edge-Case-Handling.pdf`

**Edge Cases Covered:**

**Registration:**
- Duplicate email/phone → Error message
- Invalid referral code → Allow registration without referral
- OTP expired → Request new OTP
- 3 failed OTP attempts → Account locked

**Cart & Orders:**
- Out of stock → Error message, show available quantity
- Item becomes unavailable → Remove from cart, notify user
- Empty cart checkout → Error "Cart is empty"
- Concurrent orders → First succeeds, second fails with stock error

**Order Status:**
- Invalid status transition → Error "Invalid status change"
- Cancel completed order → Error "Cannot cancel completed order"
- Cancel preparing order → Admin only

**Data Integrity:**
- Food deleted but in cart → Validate on order placement
- Database connection lost → Global exception handler
- Invalid JSON → Model validation error

**Key Decisions:**
- Clear error messages for all cases
- User-friendly error handling
- System resilience

---

## How to Use These Diagrams

### **For Evaluators:**
1. Review each diagram to understand system flow
2. Cross-reference with API documentation
3. Test flows using Swagger UI
4. Verify edge case handling

### **For Developers:**
1. Understand business logic before coding
2. Follow documented flows
3. Handle all edge cases shown
4. Maintain consistency

---

## Diagram File Summary

| File | Description | Pages |
|------|-------------|-------|
| 01-User-Registration-Verification-Flow.pdf | Signup and verification process | 1 |
| 02-Food-Browsing-Flow.pdf | Browse and filter food items | 1 |
| 03-Cart-Order-Placement-Flow.pdf | Add to cart and place order | 1 |
| 04-Order-Status-Lifecycle.pdf | Order status transitions | 1 |
| 05-Admin-Management-Flow.pdf | Admin operations | 1 |
| 06-Edge-Case-Handling.pdf | Error scenarios | 1 |

**Total Diagrams:** 6
**Format:** PDF
**Creation Tool:** Draw.io / diagrams.net

---

## Additional Documentation

- **API Details:** See `API_DOCUMENTATION.md`
- **Business Logic:** See `DATA_FLOWS.md`
- **Edge Cases:** See `EDGE_CASES.md`
- **Main README:** See `README.md`

---

**All diagrams are included in the submission package.**
