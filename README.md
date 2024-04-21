# 24 Hours Challenge App

## Backend Setup

1. **Change Connection String:** 
   - Open the `appsettings.Development.json` file in the `Backend` folder.
   - Locate the `ConnectionStrings` section and update the `DefaultConnection` value with your database connection string.

2. **Run Entity Framework Migrations:**
   - Open a `Package Manager Console` terminal or command prompt on your machine.
   - Navigate to the Infrastructure Layer Folder where your DbContext class and Migrations folder are locate
   - Run the following command to apply the migrations to your database:
     ```
     dotnet ef database update
     ```
   - Review the Output: The command will output information about the migration process, including any errors or warnings encounter
   - Check the Database: After running the command, check your database to ensure that the schema changes have been applied successfully.


## Frontend Setup

1. **Install Dependencies:**
   - Navigate to the `Front-End` folder in your terminal.
   - Run the following command to install the dependencies:
     ```
     npm install
     ```

2. **Run the Frontend:**
   - After installing the dependencies, run the following command to start the frontend application:
     ```
     npm run start
     ```

3. **Access the Application:**
   - Open your web browser and navigate to `http://localhost:4200` to view the frontend of your application.

## Notes
- Make sure to run the backend before starting the frontend.
- You may need to configure the frontend to connect to the correct backend URL if it's different from the default.
