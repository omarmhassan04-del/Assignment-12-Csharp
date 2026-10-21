# Assignment 12: EF Core Navigation Properties & Relationship Mapping

This repository contains the complete Entity Framework Core solutions for all 3 projects:

1. **Project 1: E-commerce System** (`/Project1_EcommerceSystem`)
   - Category, Product, Customer, Order, OrderDetail
   - Mapped Relationships:
     - One-to-Many: Category to Product
     - Many-to-Many: Order to Product through OrderDetail
     - One-to-Many: Customer to Order
   - Includes full Database ERD diagram in `README.md`.

2. **Project 2: Library System** (`/Project2_LibrarySystem`)
   - Author, Book, Borrower, Loan
   - Mapped Relationships:
     - One-to-Many: Author to Book
     - Many-to-Many: Book to Borrower through Loan
   - Includes full Database ERD diagram in `README.md`.

3. **Project 3: Health Care System** (`/Project3_HealthCareSystem`)
   - Patient, Doctor, Appointment
   - Mapped Relationships:
     - Many-to-Many: Patient to Doctor through Appointment
   - Includes LinkedIn article on Navigation Properties and full Database ERD diagram in `README.md`.

---

## LinkedIn Article: Navigation Properties in Entity Framework Core

### **Title**: Demystifying Navigation Properties in Entity Framework Core: From Relationships to Real Code

When developing applications with Entity Framework Core (EF Core), domain models are richly interconnected: customers place orders, authors write books, and patients schedule appointments with doctors. At the center of navigating these connections in object-oriented code lies the concept of **Navigation Properties**.

---

### What Are Navigation Properties?

In relational databases, relationships are formed purely using **Foreign Keys (FK)**: a column in one table stores the primary key value of another table.

In C# and object-oriented programming, however, we don't want to constantly perform manual joins using foreign key integer IDs. We want to navigate seamlessly between related objects:

```csharp
var doctorName = appointment.Doctor.Name;
var patientAppointments = patient.Appointments;
```

A **Navigation Property** is a property on an entity type that allows you to navigate from one end of a relationship to the other:
1. **Reference Navigation Property**: Points to a single related entity (e.g., `Appointment.Patient`, `Product.Category`). Represents the **"One"** side.
2. **Collection Navigation Property**: Points to multiple related entities (e.g., `Patient.Appointments`, `Category.Products`). Represents the **"Many"** side.

---

### Why Are They Crucial?

1. **Object-Relational Mapping (ORM) Fidelity**: Bridges relational foreign keys and object graph traversal.
2. **Clean Querying with Eager / Explicit Loading**:
   ```csharp
   var patientWithDoctor = context.Patients
       .Include(p => p.Appointments)
           .ThenInclude(a => a.Doctor)
       .FirstOrDefault(p => p.Id == 1);
   ```
3. **Encapsulation & Domain Logic**: Navigates the business aggregate without polluting domain logic with raw foreign key queries.

---

### Modeling Many-to-Many Relationships (Health Care System Case Study)

In our **Health Care System**, a patient can have appointments with multiple doctors, and a doctor treats multiple patients. Because an appointment has its own payload and identity (such as `AppointmentDate`), we model it using an explicit join entity:

```
[Patient] 1 <---> M [Appointment] M <---> 1 [Doctor]
```

#### The Fluent API Configuration:
```csharp
modelBuilder.Entity<Appointment>()
    .HasKey(a => new { a.PatientId, a.DoctorId, a.AppointmentDate });

modelBuilder.Entity<Appointment>()
    .HasOne(a => a.Patient)
    .WithMany(p => p.Appointments)
    .HasForeignKey(a => a.PatientId);

modelBuilder.Entity<Appointment>()
    .HasOne(a => a.Doctor)
    .WithMany(d => d.Appointments)
    .HasForeignKey(a => a.DoctorId);
```

### Key Takeaways
- Navigation properties transform static database records into rich, navigable C# domain models.
- Always provide inverse navigation properties when bidirectional traversal is needed.
- Using explicit join entities with navigation properties gives full control over payloads (dates, quantities, notes) in many-to-many associations.

#CSharp #DotNet #EntityFrameworkCore #CleanArchitecture #SoftwareEngineering #Databases #ORMMapping
