Application Project

Packages
Microsoft.EntityFrameworkCore

Infrastructure Project

Packages
Npgsql.EntityFrameworkCore.PostgreSQL
EFCore.NamingConventions


```
dotnet ef migrations add InitialCreate --startup-project .\Modules\Bookings\src\Bookings.API --project .\Modules\Bookings\src\Bookings.Infrastructure --output-dir .\Persistance\Migrations

```
```
dotnet ef database update --startup-project .\Modules\Bookings\src\Bookings.API --project .\Modules\Bookings\src\Bookings.Infrastructure
```


Packages
Stripe.net
