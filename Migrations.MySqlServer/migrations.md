### MySqlSever Migrations 

## Add new Migrations.Application
	> dotnet ef migrations add Initial -o Migrations.Application
	> dotnet ef migrations add Change_ExternalProvider_IsUnique_to_False -o Migrations.Application
	> dotnet ef migrations add Profile_in_Usuario_as_LongText -o Migrations.Application


## UpdateDatabase Migrations.Application
	> dotnet ef database update

## Return to a state creatred by Mingrations
	> dotnet ef database update 20250804062341_Initial.cs
	> dotnet ef database update Change_ExternalProvider_IsUnique_to_False.cs
	> dotnet ef database update Profile_in_Usuario_as_LongText.cs