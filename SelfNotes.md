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
- Delete migration folder all files 
```bash
   Drop-Database
   Update-Database
   Add-Migration AddCategoryTableToDb
   Update-Database
```