Scaffold-DbContext "Data Source=10.0.20.14;Initial Catalog=ConstructionTracking_20241206;User ID=constructiontracking;Password=constructiontracking@2024;TrustServerCertificate=True;" Microsoft.EntityFrameWorkCore.SqlServer -outputdir Data -context ContructionTrackingDbContext -contextdir Data -DataAnnotations -UseDatabaseNames -Force

// Package gen Excel
Install-Package ClosedXML

