### MySqlSever Add Migrations 

dotnet ef migrations add Initial  -o Migrations.Application
## Migrations.Application
dotnet ef migrations add UpdateDataBase -c MySqlServerContext -p ./Migrations.MySqlServer/Migrations.MySqlServer.csproj -s ./Despesas.WebApi -o Migrations.Application
dotnet ef migrations add Change-Ids-TypeInt-to-UUID -c MySqlServerContext -p ./Migrations.MySqlServer/Migrations.MySqlServer.csproj -s ./Despesas.WebApi -o Migrations.Application
dotnet ef migrations add GoogleCredentials -c MySqlServerContext -p ./Migrations.MySqlServer/Migrations.MySqlServer.csproj -s ./Despesas.WebApi -o Migrations.Application
dotnet ef database update -c MySqlServerContext -p ./Migrations.MySqlServer/Migrations.MySqlServer.csproj -s ./Despesas.WebApi


### Return to a state creatred by Mingrations
dotnet ef database update 20231221234827_InitialCreate
dotnet ef database update 20231222054303_Changes-Props_Email_Password_To_Value_Objects
dotnet ef database update 20250803203640_GoogleCredentials


dotnet ef database update 20231222054303_Changes-relationship-between-Card-and-CardBrand
dotnet ef database update  -o Migrations.Application 20240517172405_Initial

dotnet ef migrations add UpdateDataBase -o Migrations.Application


dotnet ef migrations add Initial -o Migrations.Application
dotnet ef  database update 20250803203640_GoogleCredentials -o Migrations.Application
