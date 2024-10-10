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
## Screenshots
### Login Page : Include regular signin / Login functionality and Google Login
![螢幕擷取畫面_10-10-2024_232621_rungroups-dbhmccayeuc2dpg0 eastasia-01 azurewebsites net](https://github.com/user-attachments/assets/256f75e7-f9d9-4a51-b82c-e400f736b9cb)
### Home Page
![螢幕擷取畫面_10-10-2024_232444_rungroups-dbhmccayeuc2dpg0 eastasia-01 azurewebsites net](https://github.com/user-attachments/assets/e92ba99f-fe90-4f83-9139-305eff42a4b7)
### Club List : List all the clubs
![螢幕擷取畫面_10-10-2024_232516_rungroups-dbhmccayeuc2dpg0 eastasia-01 azurewebsites net](https://github.com/user-attachments/assets/a0c5b2cd-0f60-4c8b-a5ca-80ad48e46d9b)
### Club Detail: Demonstrate the deatails of a club
![螢幕擷取畫面_10-10-2024_232939_rungroups-dbhmccayeuc2dpg0 eastasia-01 azurewebsites net](https://github.com/user-attachments/assets/6d6a953f-ff2a-4c68-a5b3-b317e8996a46)
### Person porfile
![螢幕擷取畫面_10-10-2024_232527_rungroups-dbhmccayeuc2dpg0 eastasia-01 azurewebsites net](https://github.com/user-attachments/assets/7d39a3fd-f66f-43b4-b174-f96725791faf)
### Added Clubs List : Show all the clubs that you've attend
![螢幕擷取畫面_10-10-2024_23262_rungroups-dbhmccayeuc2dpg0 eastasia-01 azurewebsites net](https://github.com/user-attachments/assets/a984a007-ba48-4d8b-b077-fa5bb3363947)


