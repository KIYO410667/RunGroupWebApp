# RunGroupWebApp
## Description
This is a web application built using ASP.NET Core MVC. The project follows the traditional server-side rendering (SSR) approach where the backend handles both the business logic and the rendering of the web pages using Razor views.

## Key Features:
- Developed using ASP.NET Core MVC for the backend.
- Razor templates are used to generate dynamic web pages.
- Integrated with Azure for deployment.
- SQL Server is used as the primary database.
- Implements user authentication and role management.
## Tech Stack
- Backend: ASP.NET Core MVC
- Frontend: Razor (server-side rendering)
- Database: MSSQL -> Azure SQL Databases
- Unstructurized NoSQL: Azure Blob
- Deployment: Azure App Service
- Authenticaton: Cookie&Session / OAuth2
- Other Tools: Entity Framework Core, Bootstrap
## Installation
Clone the repository:
bash
Copy code
git clone https://github.com/your-username/your-project-name.git
Navigate to the project folder:
bash
Copy code
cd your-project-name
Restore dependencies:
bash
Copy code
dotnet restore
Apply database migrations (if using Entity Framework):
bash
Copy code
dotnet ef database update
Run the application:
bash
Copy code
dotnet run
# Future Plans (Optional)
- Refactor frontend using React or Vue.js to achieve a front-end and back-end separation.
- Implement a RESTful API to allow the frontend to interact with the backend.

# Screenshots
(Include any relevant screenshots of your application)
