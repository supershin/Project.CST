Scaffold-DbContext "Data Source=10.0.10.8;Initial Catalog=ConstructionTracking;User ID=constructiontracking;Password=constructiontracking@2024;TrustServerCertificate=True;" Microsoft.EntityFrameWorkCore.SqlServer -outputdir Data -context ContructionTrackingDbContext -contextdir Data -DataAnnotations -UseDatabaseNames -Force

// Package gen Excel
Install-Package ClosedXML

