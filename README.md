# RunGroupWebApp
## Description
This is a web application built using ASP.NET Core MVC. The project follows the traditional server-side rendering (SSR) approach where the backend handles both the business logic and the rendering of the web pages using Razor views.

## Key Features:
- Developed using ASP.NET Core MVC for the backend.
- Razor templates are used to generate dynamic web pages.
- Integrated with Azure for deployment.
- Azure SQL Databases is used as the primary database.
- Implements user authentication and role management.
## Tech Stack
- Backend: ASP.NET Core MVC
- Frontend: Razor (server-side rendering)
- Database: MSSQL -> Azure SQL Databases
- Unstructurized NoSQL: Azure Blob
- Deployment: Azure App Service
- Authenticaton: Cookie & Session, OAuth2
- Other Tools: Entity Framework Core, Bootstrap
## Installation
### Clone the repository:
```bash
git clone https://github.com/KIYO410667/RunGroupWebApp.git
```
### Navigate to the project folder:
```bash
cd RunGroupWebApp
```
### Restore dependencies:
```bash
dotnet restore
```
### Apply database migrations:
```bash
dotnet ef database update
```
### Run the application:
```bash
dotnet run
```
# Future Plans
- Refactor frontend using React or Vue.js to achieve a front-end and back-end separation.
- Implement a RESTful API to allow the frontend to interact with the backend.
- Replace Azure Blob with structured NoSQL
# Screenshots
(Include any relevant screenshots of your application)
