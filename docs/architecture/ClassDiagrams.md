# Class Diagrams

This section provides an overview of the main API components and their responsibilities within the Clean Architecture structure. It helps developers understand how requests flow from controllers to the application layer and domain.

---

## Architectural Placement

- API Layer: Controllers orchestrate requests, validation, and mapping.
- Application Layer: Use cases and service abstractions (IAccountRepository, ITransactionRepository, IUnitOfWork).
- Domain Layer: Core entities (User, Account, Transaction) and enums (TransactionType, AccountType).

---

## Controllers and Dependencies

### Auth Controller

Handles login and user authentication.

![Auth Controller](../diagrams/AuthController.drawio.png)

---

### User Controller

Allows viewing and updating the logged-in user profile.

![User Controller](../diagrams/UserController.drawio.png)

---

### Account Controller

Responsible for:

- Getting account balances
- Listing accounts for a user
- Closing accounts (with validation)

![Account Controller](../diagrams/AccountController.drawio.png)

---

### Transfer Controller

Handles internal account-to-account transfers.

![Transfer Controller](../diagrams/TransferController.drawio.png)

---

### Airtime Controller

Coordinates airtime purchase requests via a stub integration.

![Airtime Controller](../diagrams/AirtimeController.drawio.png)

---

### Transaction Controller

Supports:

- Retrieving account transaction history
- Managing manual transactions

![Transaction Controller](../diagrams/TransactionController.drawio.png)

---

## Notes

- Controllers do not implement business rules.
- All core logic lives in the Application layer via services and repositories.
- Mapping between request DTOs and domain responses ensures boundaries remain clear.

---

## Related Documentation

- [Architecture Overview](Overview.md)
- [Database ERD & Domain Relationships](Domain_ERD.md)
- [Sequence Diagrams](SequenceDiagrams.md)
