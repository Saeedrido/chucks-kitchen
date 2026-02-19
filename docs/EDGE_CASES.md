# Edge Cases & Error Handling

This document details all edge cases handled in the Chuks Kitchen system.

---

## 1. Authentication Edge Cases

### Duplicate Email or Phone
**Scenario:** User tries to register with existing email/phone

**Handling:**
- ✅ Check both email and phone for duplicates
- ✅ Return specific error message
- ✅ Prevent duplicate registration

---

### Invalid or Expired OTP
**Scenario:** User enters wrong or expired OTP code

**Handling:**
- ✅ OTP expires after 10 minutes
- ✅ Failed attempts tracked (max 3)
- ✅ Account locked after 3 failed attempts
- ✅ Clear error messages for each case

---

### Invalid Referral Code
**Scenario:** User enters referral code that doesn't exist

**Handling:**
- ✅ Validate referral code against database
- ✅ Allow registration to continue without referral
- ✅ Inform user of invalid code

---

### Account Lockout
**Scenario:** User enters wrong OTP 3 times

**Handling:**
- ✅ Account locked after 3 failed attempts
- ✅ User must generate new OTP to unlock
- ✅ Clear message about lockout

---

## 2. Food & Inventory Edge Cases

### Out of Stock
**Scenario:** User tries to add unavailable food to cart

**Handling:**
- ✅ Check stock quantity before adding
- ✅ Return error if insufficient stock
- ✅ Show available quantity to user

---

### Food Becomes Unavailable
**Scenario:** Food was in cart but became unavailable

**Handling:**
- ✅ Validate availability at order placement
- ✅ Remove unavailable items from cart
- ✅ Inform user of removed items
- ✅ Allow order with remaining items

---

### Price Changes
**Scenario:** Food price changes after adding to cart

**Handling:**
- ✅ Snapshot price at cart addition time
- ✅ Use cart price for order calculation
- ✅ Update to current price on next addition

---

## 3. Cart Management Edge Cases

### Duplicate Cart Items
**Scenario:** User adds same food item twice

**Handling:**
- ✅ Merge items (increase quantity)
- ✅ Keep original special instructions
- ✅ Update unit price snapshot

---

### Cart Abandonment
**Scenario:** User adds items but doesn't checkout

**Handling:**
- ✅ Cart persists in database
- ✅ No automatic cleanup (per requirements)
- ✅ Cart available on next login

---

### Empty Cart Checkout
**Scenario:** User tries to place order with empty cart

**Handling:**
- ✅ Validate cart has items before order
- ✅ Return error: "Cart is empty"
- ✅ Prevent order creation

---

### Quantity Updates
**Scenario:** User updates quantity to zero or negative

**Handling:**
- ✅ Validate quantity > 0
- ✅ Remove item if quantity becomes 0
- ✅ Return error for negative values

---

## 4. Order Placement Edge Cases

### Concurrent Order Placement
**Scenario:** Two users order last item simultaneously

**Handling:**
- ✅ Stock reserved immediately on validation
- ✅ First order succeeds, second fails
- ✅ Clear error for insufficient stock
- ✅ Restore stock on order cancellation

---

### Invalid Delivery Address
**Scenario:** User provides empty or invalid address

**Handling:**
- ✅ Validate address not empty
- ✅ Validate minimum length (10 chars)
- ✅ Return specific error message

---

### Order Number Generation
**Scenario:** Multiple orders created at same time

**Handling:**
- ✅ Unique order number format: CK + timestamp + random
- ✅ 6-digit random suffix prevents collision
- ✅ Database uniqueness constraint

---

## 5. Order Status Edge Cases

### Invalid Status Transitions
**Scenario:** Admin tries invalid status change

**Valid Transitions:**
```
Pending → Confirmed → Preparing → OutForDelivery → Completed
                     ↓
                  Cancelled
```

**Handling:**
- ✅ Validate all status transitions
- ✅ Return error for invalid transitions
- ✅ Prevent backward moves (except cancellation)

---

### Cancellation Rules
**Scenario:** Customer or Admin tries to cancel order

**Rules:**
- ✅ Customer can cancel: Pending, Confirmed
- ✅ Admin can cancel: Any status before Completed
- ✅ Cannot cancel: Completed orders
- ✅ Stock restored on cancellation

---

### Status Timestamp Tracking
**Scenario:** Order status changes multiple times

**Handling:**
- ✅ Timestamp for each status change
- ✅ Status history maintained
- ✅ Can track order journey

---

## 6. Payment Edge Cases

### Payment Failure
**Scenario:** Payment fails after order creation

**Handling:**
- ✅ Order created in Pending status
- ✅ Admin must confirm before processing
- ✅ No auto-cancellation (manual process)

---

### Payment Timeout
**Scenario:** User doesn't complete payment

**Handling:**
- ✅ Order remains in Pending status
- ✅ No automatic timeout (per requirements)
- ✅ Admin can cancel stale orders

---

## 7. Data Integrity Edge Cases

### Orphaned Cart Items
**Scenario:** Food deleted but exists in cart

**Handling:**
- ✅ Soft delete on food items
- ✅ Validate food exists on cart access
- ✅ Remove invalid items on order placement

---

### User Deletion
**Scenario:** User account deleted

**Handling:**
- ✅ Soft delete on user accounts
- ✅ Orders preserved (historical data)
- ✅ Cart preserved for reporting

---

### Database Connection Loss
**Scenario:** Database becomes unavailable

**Handling:**
- ✅ Global exception handler catches errors
- ✅ User-friendly error message
- ✅ Detailed error in development mode
- ✅ Logs error for investigation

---

## 8. API Edge Cases

### Missing Headers
**Scenario:** Required headers not provided

**Handling:**
- ✅ Validate userId header where required
- ✅ Validate adminId header for admin operations
- ✅ Return 401 Unauthorized for missing auth

---

### Invalid JSON
**Scenario:** Malformed request body

**Handling:**
- ✅ Automatic model validation
- ✅ Clear error message
- ✅ HTTP 400 Bad Request

---

### Large Payloads
**Scenario:** Very large request body

**Handling:**
- ✅ ASP.NET Core default size limits
- ✅ Returns 413 Payload Too Large
- ✅ Prevents DOS attacks

---

## 9. Concurrency Edge Cases

### Race Conditions
**Scenario:** Multiple requests modify same data

**Handling:**
- ✅ Database transaction isolation
- ✅ First-write-wins for stock updates
- ✅ Optimistic concurrency for critical updates

---

### Stale Data
**Scenario:** Data changes between read and write

**Handling:**
- ✅ Re-validate on write operations
- ✅ Use database constraints
- ✅ Clear error on conflict

---

## 10. Security Edge Cases

### Brute Force Attacks
**Scenario:** Multiple login attempts

**Handling:**
- ✅ Password hashed with BCrypt
- ✅ No account lockout on password (per requirements)
- ✅ OTP lockout after 3 attempts
- ✅ Consider rate limiting (future enhancement)

---

### SQL Injection
**Scenario:** Malicious input in fields

**Handling:**
- ✅ Parameterized queries (EF Core)
- ✅ Input validation on all fields
- ✅ No raw SQL queries

---

### XSS Attacks
**Scenario:** Script tags in user input

**Handling:**
- ✅ Input validation
- ✅ Output encoding in responses
- ✅ No HTML in API responses

---

## Summary

**Total Edge Cases Handled:** 30+

**Categories:**
- Authentication (5 cases)
- Food & Inventory (3 cases)
- Cart Management (4 cases)
- Order Placement (3 cases)
- Order Status (3 cases)
- Payment (2 cases)
- Data Integrity (3 cases)
- API (3 cases)
- Concurrency (2 cases)
- Security (3 cases)

**All edge cases include:**
- ✅ Detection logic
- ✅ Clear error messages
- ✅ User-friendly handling
- ✅ Logging for debugging

---

**For more details, see the source code in:**
- `/ChuksKitchen.Application/Services/`
- `/ChuksKitchen.API/Controllers/`
- `/ChuksKitchen.API/Middleware/`
