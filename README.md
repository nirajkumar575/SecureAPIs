# 🔐 SecureAPI Project

This project includes full-stack implementation of authentication, authorization, and location-based features using ASP.NET Core and Entity Framework Core.

## 🚀 Features Implemented

- ✅ **User Registration & Authentication**  
  - Integrated with a database using **Entity Framework Core**
  - Passwords hashed using **BCrypt.Net**
  
- ✅ **Role-Based Authorization**  
  - Secured endpoints using `[Authorize(Roles = "RoleName")]`
  - Implemented **rate limiting** for enhanced security

- ✅ **Location Management**  
  - Integrated with **external location API**
  - Implemented `LocationController` and `LocationService`

- ✅ **Security Enhancements**  
  - Custom middleware for secure headers
  - Password encryption using best practices

- ✅ **Architecture & Code Quality**  
  - Adopted **Generic Repository Pattern**
  - Used **Dependency Injection** for service registration
  - Configured **appsettings.json** for DB & API keys

- ✅ **EF Core Migrations**  
  - Database schema managed via migrations
  - Connection strings and external API keys configurable

## 🛠️ Tech Stack

- ASP.NET Core 7+
- Entity Framework Core
- SQL Server
- BCrypt.Net
- Rate Limiting Middleware
- Swagger (if applicable)

## ⚙️ Setup Instructions

> Make sure you have .NET 7 SDK and SQL Server installed.

1. Clone the repo:
   ```bash
   git clone https://github.com/nirajkumar575/SecureAPIs.git
