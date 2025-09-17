# dotNet5785_-5602_4857

This repository contains a **.NET solution** built with Visual Studio.  
It includes multiple projects organized into layers to demonstrate clean architecture and modular design.  

This project is a **volunteer and call management system** built in C# with a WPF interface.  
It allows managers to register calls, assign them to volunteers, and track their progress in real time.  

- Volunteers are stored in XML with details like availability and maximum response distance.  
- Calls can be created, assigned, canceled, or tracked.  
- The system notifies and updates volunteers based on their eligibility.  
- Managers can view and manage volunteer lists, calls, and assignments through the WPF interface.  
- The architecture is split into **DAL**, **BL**, and **PL** layers, making it modular and easy to test.  

---

## Tech Stack
- .NET Framework / .NET Core (adapt depending on your target)  
- C#  
- Visual Studio 2022 (recommended IDE)  
- Additional libraries (update as relevant, e.g., Entity Framework, Avalonia, WPF, ASP.NET, etc.)

---

## Solution Structure

```plaintext
dotNet5785_-5602_4857.sln
│
├── BL/          # Business Logic Layer
├── BLTest/      # Tests for BL
├── DalFacade/   # DAL Interfaces
├── DalList/     # In-memory DAL Implementation
├── DalXml/      # XML-based DAL Implementation
├── DalTest/     # Tests for DAL
├── PL/          # Presentation Layer (WPF with MVVM)
├── WpfLibrary1/ # Shared WPF components
├── xml/         # Sample XML data
└── Stage0/      # Initial setup
```

---

## Getting Started

### Prerequisites
- Visual Studio with .NET workload installed  
- .NET SDK (matching the project version)

---

### Installation

1. **Clone the repository**:
   ```bash
   git clone https://github.com/ChayaRubin/dotNet5785_-5602_4857.git
   ```

2. **Open the solution in Visual Studio**:
   ```bash
   dotnet sln dotNet5785_-5602_4857.sln
   ```

3. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

---

### Running

- From Visual Studio: Press **F5** to build and run, or press Green arrow after choosing PL.
- From CLI:
   ```bash
   dotnet build
   dotnet run --project PL
   ```

---

## Features
- Modular architecture (separation of concerns between DAL, BL, and UI layers)  
- Extensible and testable design  
- Example CRUD operations / data models (adjust with your project’s real features)

---

## About
C# project  

---

## Releases
- 8 tags  
- Create a new release  

---

## Packages
No packages published  
Publish your first package  

---

## Languages
- **C#** 100%
