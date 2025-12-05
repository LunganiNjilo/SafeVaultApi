# 🗄️ Database ERD & Domain Relationships

The SafeVault database consists of three core domain entities modeled using
**Clean Architecture principles** and persisted via **EF Core**:

- `User`
- `Account`
- `Transaction`

These align directly with the assessment requirements for **Persons, Accounts, and Transactions**.

---

## 📊 Entity Relationship Diagram (ERD)

![ERD](../diagrams/ERD.drawio.png)

> A `User` can have multiple `Accounts`, and each `Account` can have multiple `Transactions`.

---

## 🧩 Entities and Constraints

### 👤 User
| Property | Rule |
|---------|-----|
| FirstName / LastName | Required, max length 100 |
| IdNumber | Required, **unique**, length 13 |
| DateOfBirth | Required |
| Email | Required, **unique** |
| PasswordHash | Hashed for security |

**Assessment alignment:**
✔ A person can only be created once with the same ID Number  
✔ Person is uniquely identifiable  

---

### 💼 Account
| Property | Rule |
|---------|-----|
| AccountNumber | Required, **unique**, max length 30 |
| Name | Required |
| AccountType | Required (enum: Savings / Current) |
| Balance | Decimal(18,2), **never edited directly** |
| IsClosed | Default: false |
| UserId | FK → User |

**Business rules enforced**
✔ Multiple accounts allowed per user  
✔ Accounts cannot be created before User exists  
✔ Account balance must auto-update when transactions occur  
✔ Cannot close account if balance ≠ 0  
✔ No transactions allowed on closed accounts  

---

### 💳 Transaction
| Property | Rule |
|---------|-----|
| Amount | Decimal(18,2), cannot be `0` |
| Type | Debit / Credit |
| Description | Optional |
| CreatedAt | Auto-stamped |
| BalanceAfter | Balance snapshot after transaction |
| AccountId | FK → Account |
| IsManual | Default: false |

**Validation rules**
✔ Must be tied to an existing Account  
✔ Date cannot be in the future  
✔ Capture date is auto-managed (immutable)  

---

## 🔐 Referential Integrity

| Relationship | Behavior |
|-------------|----------|
| User → Accounts | Cascade Delete |
| Account → Transactions | Cascade Delete |

> This ensures **no orphaned accounts** and **transactions always remain traceable**.

---

## 🧱 Architectural Alignment

This ERD supports:

- **Auditability:** every balance change recorded as a transaction  
- **Security:** users can only access their own accounts  
- **Assessment compliance:** meets all CRUD and validation expectations  
- **Domain correctness:** real banking fundamentals reflected  

---

## 📈 Future Expansion

| Feature | Impact |
|--------|-------|
| Multiple Users authentication | Supports real multi-customer system |
| More Account Types | (Credit card, loans, investments) |
| Transaction Fees & Reversals | Better financial control |
| Account closure auditing | Regulatory compliance |

The model is intentionally simple — but **rock solid** as a foundation for a production banking system.
