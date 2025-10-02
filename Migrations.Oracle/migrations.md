### MySqlSever Migrations 

## Add new Migrations.Application
	> dotnet ef migrations add Initial -o Migrations.Application


## UpdateDatabase Migrations.Application
	> dotnet ef database update

## Return to a state creatred by Mingrations
	> dotnet ef database update 20250904145638_Initial.cs
