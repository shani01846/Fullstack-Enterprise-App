````markdown
# 🎟️ Chinese Auction Management System

**Full-Stack Web Application — ASP.NET Core Web API + Angular**

A robust full-stack web application designed to manage the complete Chinese auction lifecycle — from prize donations and ticket purchases to automated raffles and winner selection.

---

## 🧩 Project Overview

The system provides role-based access for three primary user types:

- 👤 **Donors** — Donate prizes to the auction, including details such as title, description, and category.
- 🛒 **Buyers** — Browse available prizes and purchase raffle tickets for specific items.
- 🛠️ **Administrators** — Manage users, prizes, and purchases, execute raffles, and review winning results.

---

## ⚙️ Tech Stack

### Backend

- **Framework:** ASP.NET Core Web API
- **ORM:** Entity Framework Core
- **Database:** SQL Server
- **Authentication:** JWT Authentication
- **Security:** Custom Middleware for Logging and Rate Limiting

### Frontend

- **Framework:** Angular
- **Language:** TypeScript
- **API Communication:** RESTful APIs
- **Authorization:** Role-Based Access Control (RBAC)

---

## 🔐 Key Features & Security

- **Authentication & Authorization**
  - Secure JWT-based authentication.
  - Role-based endpoint protection for `Admin`, `Donor`, and `Buyer`.

- **Prize Management**
  - Donors can submit prizes with relevant details.
  - Administrators can manage and review donated prizes.

- **Ticket Management**
  - Buyers can browse available prizes.
  - Buyers can purchase raffle tickets for selected prizes.

- **Automated Raffles**
  - Administrators can execute raffles.
  - Winners are selected automatically based on purchased tickets.

- **Data Integrity**
  - Relational database design using Entity Framework Core.
  - Proper entity relationships and transactional operations.

- **Error Handling & Middleware**
  - Centralized logging.
  - API rate limiting.
  - Structured error handling for improved reliability and security.

---

## 🚀 Getting Started

### Prerequisites

Make sure the following tools are installed:

- [.NET SDK](https://dotnet.microsoft.com/)
- [Node.js & npm](https://nodejs.org/)
- [Angular CLI](https://angular.dev/tools/cli)
- SQL Server

---

### 📥 Installation

#### 1. Clone the Repository

```bash
git clone https://github.com/YourUsername/Your-Repo-Name.git
cd Your-Repo-Name
````

#### 2. Backend Setup

Navigate to the backend project:

```bash
cd Backend
```

Restore the required dependencies:

```bash
dotnet restore
```

Update the database using Entity Framework Core:

```bash
dotnet ef database update
```

Run the API:

```bash
dotnet run
```

#### 3. Frontend Setup

Open a new terminal and navigate to the frontend:

```bash
cd Frontend
```

Install the required dependencies:

```bash
npm install
```

Start the Angular development server:

```bash
ng serve
```

The application will be available at:

```text
http://localhost:4200/
```

---

## 🎯 Project Goals

This project demonstrates practical implementation of:

* Clean client-server architecture.
* RESTful API design and best practices.
* Role-Based Access Control (RBAC).
* JWT authentication and authorization.
* Robust relational database modeling.
* Entity Framework Core integration.
* Middleware-based logging and rate limiting.
* Seamless integration between a C# backend and Angular frontend.

---

## 🔮 Future Enhancements

Potential future improvements include:

* 💳 Payment gateway integration.
* 📊 Advanced raffle analytics dashboard.
* 📧 Automated email notifications for winners.
* 📈 Enhanced reporting and auction statistics.
* 🔔 Real-time notifications for auction events.

---

## 📄 License

This project is intended for educational and demonstration purposes.

```
```
