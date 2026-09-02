![GitHub](https://img.shields.io/badge/GitHub-121011.svg?style=for-the-badge&logo=github&logoColor=white)

# ShieldBox

![GitHub stars](https://img.shields.io/github/stars/DevGn0m3/ShieldBox?style=for-the-badge&logo=github) ![GitHub forks](https://img.shields.io/github/forks/DevGn0m3/ShieldBox?style=for-the-badge&logo=github) ![GitHub issues](https://img.shields.io/github/issues/DevGn0m3/ShieldBox?style=for-the-badge&logo=github) ![Last commit](https://img.shields.io/github/last-commit/DevGn0m3/ShieldBox?style=for-the-badge&logo=github)

## 📑 Table of Contents

- [Description](#description)
- [Tech Stack](#tech-stack)
- [Quick Start](#quick-start)
- [Available Scripts](#available-scripts)
- [Project Structure](#project-structure)
- [Development Setup](#development-setup)
- [Contributors](#contributors)
- [Contributing](#contributing)

## 📝 Description

ShieldBox — a software project built with .NET.

## 🛠️ Tech Stack

![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)

## ⚡ Quick Start

```bash

# 1. Clone the repository
git clone https://github.com/DevGn0m3/ShieldBox.git

# Restore and run
dotnet restore && dotnet run
```

## 📁 Project Structure

```
.
├── Database
│   └── ShieldBox.sql
├── ShieldBox.sln
└── src
    └── ShieldBox
        ├── BE
        │   ├── AuditFilter.cs
        │   ├── Entities.cs
        │   ├── Enums.cs
        │   └── Permissions.cs
        ├── BLL
        │   ├── AuthService.cs
        │   └── ShieldBoxService.cs
        ├── DAL
        │   ├── DemoStore.cs
        │   ├── IUserRepository.cs
        │   └── SqlRepository.cs
        ├── Interfaces
        │   └── Patterns.cs
        ├── Program.cs
        ├── Security
        │   ├── PasswordHasher.cs
        │   └── SessionManager.cs
        ├── Services
        │   ├── ManejadorDeSesion.cs
        │   ├── PasswordHasher.cs
        │   └── SingletonSesion.cs
        ├── ShieldBox.csproj
        ├── ShieldBox.csproj.user
        └── UI
            ├── LoginForm.Designer.cs
            ├── LoginForm.cs
            ├── LoginForm.resx
            ├── MainForm.Designer.cs
            ├── MainForm.cs
            ├── MainForm.resx
            ├── NewRequestDialog.Designer.cs
            ├── NewRequestDialog.cs
            ├── NewRequestDialog.resx
            └── UiKit.cs
```

## 🛠️ Development Setup

### .NET
1. Install the [.NET SDK](https://dotnet.microsoft.com/)
2. `dotnet restore && dotnet run`
