### Add Nuget Packages
- Microsoft.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Tools

### Nuget Package Manager Console
Tools > Nuget Package Manager > Package Manager Console
```bash
   Update-Database
   Add-Migration AddCategoryTableToDb

   Update-Database
   Remove-Migration
```

## When default
```bash
# 1. First delete all migration folder (Migrations folder)

# 2. Package Manager Console:
Add-Migration InitialCreate

# 3. Then:
Update-Database
```