AuthAPI is a .NET 7 project built with Entity Framework Core and SQLite database, providing authentication and authorization functionalities for your application.

Features
1. Sign Up: Allow users to register for an account.
2. Email Confirmation: Verify user email addresses for added security.
3. Login: Enable users to authenticate and access protected resources.
4. Password Reset: Allow users to reset their passwords securely.
5. User and Role Management: Administer user accounts and assign roles.
6. Role-Based Authorization: Control access to resources based on user roles.


Technologies Used
1. .NET 7: Utilize the latest features and improvements in the .NET framework.
2. Entity Framework Core: ORM (Object-Relational Mapper) for interacting with the SQLite database.
3. SQLite: Lightweight and efficient database for storing user data.


Packages used
SendGrid: For sending emails during registration
Cloudinary: for User profile picture upload.


Installation
To run the AuthAPI project locally, follow these steps:
1. Ensure you have .NET 7 SDK installed.
2. Clone this repository to your local machine.
3. Navigate to the project directory.
4. Restore dependencies using dotnet restore.
5. Update the database schema using Entity Framework Core migrations: "dotnet ef database update"
6. Start the application: "dotnet run"


Configuration
Email Configuration: Update the appsettings.json file with your sendgrid apikey

Usage
Refer to the API documentation or Swagger UI for detailed information on each endpoint.
http://localhost:5165/swagger/index.html
