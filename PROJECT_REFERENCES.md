# Project Dependencies & References

**Date:** February 17, 2026
**Project:** Chuks Kitchen Food Ordering System

---

## 🎯 CLEAN ARCHITECTURE DEPENDENCY FLOW

```
┌─────────────────────────────────────────────────────────────┐
│                    API Layer (Presentation)                   │
│  ChuksKitchen.API                                            │
│  └── Depends on: Application, Persistence, Infrastructure   │
└─────────────────────────────────────────────────────────────┘
                             ↓
┌─────────────────────────────────────────────────────────────┐
│               Application Layer (Business Logic)              │
│  ChuksKitchen.Application                                    │
│  └── Depends on: Domain Layer ONLY                          │
│  └── Defines Interfaces for outer layers to implement        │
└─────────────────────────────────────────────────────────────┘
                             ↓
┌─────────────────────────────────────────────────────────────┐
│             Persistence & Infrastructure Layers               │
│  ChuksKitchen.Persistence                                   │
│  ChuksKitchen.Infrastructure                                │
│  └── Depend on: Application + Domain                        │
│  └── Implement: Application Layer Interfaces                │
└─────────────────────────────────────────────────────────────┘
                             ↓
┌─────────────────────────────────────────────────────────────┐
│                    Domain Layer (Entities)                   │
│  ChuksKitchen.Domain                                         │
│  └── No Dependencies (Core Business Entities)               │
└─────────────────────────────────────────────────────────────┘
```

---

## 📦 PROJECT REFERENCES CONFIGURATION

### **1. Domain Project** ✅
**File:** `ChuksKitchen.Domain/ChuksKitchen.Domain.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <!-- No project references - Core layer -->

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
</Project>
```

**Dependencies:** None (Core layer with entities)

---

### **2. Application Project** ✅
**File:** `ChuksKitchen.Application/ChuksKitchen.Application.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <ItemGroup>
    <ProjectReference Include="..\ChuksKitchen.Domain\ChuksKitchen.Domain.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
    <PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="8.0.0" />
    <PackageReference Include="Microsoft.Extensions.Configuration.Abstractions" Version="8.0.0" />
    <PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="7.1.2" />
    <PackageReference Include="Microsoft.IdentityModel.Tokens" Version="7.1.2" />
  </ItemGroup>

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
```

**Dependencies:**
- ✅ Domain (for entities)
- ❌ NO Persistence (correct - dependency inversion)
- ❌ NO Infrastructure (correct - dependency inversion)

**Contains:**
- Service interfaces (`Services/Interfaces/`)
- Service implementations (`Services/`)
- Repository interfaces (`Repositories/Interfaces/`)
- DTOs (`DTOs/Requests/`, `DTOs/Responses/`)

---

### **3. Persistence Project** ✅
**File:** `ChuksKitchen.Persistence/ChuksKitchen.Persistence.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <ItemGroup>
    <ProjectReference Include="..\ChuksKitchen.Application\ChuksKitchen.Application.csproj" />
    <ProjectReference Include="..\ChuksKitchen.Domain\ChuksKitchen.Domain.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.0">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="8.0.0" />
  </ItemGroup>

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
```

**Dependencies:**
- ✅ Application (for repository interfaces)
- ✅ Domain (for entities)

**Contains:**
- Repository implementations (`Repositories/`)
- DbContext (`Data/AppDbContext`)

**Namespaces Used:**
```csharp
using ChuksKitchen.Application.Repositories.Interfaces;  // For repository interfaces
using ChuksKitchen.Domain.Entities;                      // For entities
```

---

### **4. Infrastructure Project** ✅
**File:** `ChuksKitchen.Infrastructure/ChuksKitchen.Infrastructure.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <ItemGroup>
    <ProjectReference Include="..\ChuksKitchen.Application\ChuksKitchen.Application.csproj" />
    <ProjectReference Include="..\ChuksKitchen.Domain\ChuksKitchen.Domain.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="8.0.0" />
  </ItemGroup>

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
```

**Dependencies:**
- ✅ Application (for service interfaces like IEmailService, ISmsService)
- ✅ Domain (for entities if needed)

**Contains:**
- External service implementations (`Services/EmailService`, `Services/SmsService`)

**Namespaces Used:**
```csharp
using ChuksKitchen.Application.Services.Interfaces;  // For service interfaces
```

---

### **5. API Project** ✅
**File:** `ChuksKitchen.API/ChuksKitchen.API.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <ItemGroup>
    <ProjectReference Include="..\ChuksKitchen.Application\ChuksKitchen.Application.csproj" />
    <ProjectReference Include="..\ChuksKitchen.Persistence\ChuksKitchen.Persistence.csproj" />
    <ProjectReference Include="..\ChuksKitchen.Infrastructure\ChuksKitchen.Infrastructure.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.0" />
    <PackageReference Include="Microsoft.AspNetCore.Mvc.Versioning" Version="5.1.0" />
    <PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="8.0.0" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
  </ItemGroup>

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
```

**Dependencies:**
- ✅ Application (for services and DTOs)
- ✅ Persistence (for DbContext)
- ✅ Infrastructure (for external services)

**Contains:**
- Controllers
- Middleware (Global Exception Handler)
- Program.cs (DI configuration)

**Namespaces Used:**
```csharp
using ChuksKitchen.Application.Repositories.Interfaces;  // For repository interfaces
using ChuksKitchen.Application.Services.Interfaces;     // For service interfaces
using ChuksKitchen.Application.Services;                // For service implementations
using ChuksKitchen.Persistence.Data;                     // For DbContext
using ChuksKitchen.Persistence.Repositories;            // For repository implementations
```

---

## ✅ DEPENDENCY RULES VERIFICATION

### **✅ Correct Dependencies:**
1. **Domain** → No dependencies ✅
2. **Application** → Domain only ✅
3. **Persistence** → Application + Domain ✅
4. **Infrastructure** → Application + Domain ✅
5. **API** → Application + Persistence + Infrastructure ✅

### **✅ Dependency Direction:**
- ✅ **Inward Only**: Dependencies point toward the center
- ✅ **No Circular Dependencies**: Clean unidirectional flow
- ✅ **Dependency Inversion**: Application defines interfaces, outer layers implement

### **✅ Clean Architecture Compliance:**
- ✅ Application layer is independent of Persistence and Infrastructure
- ✅ Repository interfaces in Application, implementations in Persistence
- ✅ Service interfaces in Application, implementations in Application or Infrastructure
- ✅ Domain entities used by all layers but depend on nothing

---

## 🔧 FIXED BUILD ERRORS

### **Issue:**
```
error CS0234: The type or namespace name 'Application' does not exist in the namespace 'ChuksKitchen'
```

### **Root Cause:**
- Persistence and Infrastructure projects were missing project references to Application project
- They couldn't find the interfaces they needed to implement

### **Solution Applied:**
1. ✅ Added `ProjectReference` to Application in Persistence.csproj
2. ✅ Added `ProjectReference` to Application in Infrastructure.csproj
3. ✅ Removed circular reference from Application to Persistence

### **Build Status:**
- ✅ All project references correctly configured
- ✅ No circular dependencies
- ✅ Proper Clean Architecture dependency flow

---

**Status: READY TO BUILD ✅**

**All project dependencies are now correctly configured following Clean Architecture principles!** 🏆
