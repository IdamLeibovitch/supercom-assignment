# SuperCom Home Assignment - Task Management Application

This repository contains a .NET Core Web API server and a React client.

## Setup Instructions

### Prerequisites
- [.NET SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/)

1. Navigate to the `server` directory:
   ```bash
   cd server
   ```
2. Update the connection string in `appsettings.json` to point to your database, or simply review it
3. Run the server:
   ```bash
   dotnet run
   ```
1. Navigate to the `server` directory:
   ```bash
   cd server
   ```
2. Run the server:
   ```bash
   dotnet run
   ```

### Client Setup
1. Navigate to the `client` directory:
   ```bash
   cd client
   ```
2. Install dependencies:
   ```bash
   npm install
   ```
3. Start the client:
   ```bash
   npm start
   ```


   

### Core Features
- **User Authentication**: JWT-based authentication with secure token management
- **Task CRUD Operations**: Complete task lifecycle management
- **User Management**: Profile management with editable details
- **Real-time Updates**: SignalR for instant task updates across clients
- **Advanced Filtering**: Sort, search, and filter tasks by priority and users
- **Responsive Design**: Adaptive UI for desktop, tablet, and mobile
- **Form Validation**: Client and server-side validation
- **Pagination**: Efficient data loading with pagination
- **Audit Logging**: Complete audit trail of all operations
- **Robot Images**: Fascinating hashed robot images to feast your eyes on

### Project Structure
- `server/`: .NET Core Web API server
- `client/`: React client

### Maybe Noteworthy
- There's a simple (maybe too simple) user authentication, just for the sake of the scenario
- Tasks are retrieved via pagination and filtering
- A hub updates specific changes; the client determines whether to take action if the changed entity is currently rendered
- Server flow is multi-layered (_controller => BL => DAL_):
   - Controllers use annotated models and hide validation logic, receive DTOs from BL
   - Business Logic layer handles requests and processes them via other services to handle data or save logs
   - DAL encapsulates its own models and prevents unnecessary schema changes, exports DTOs via AutoMapper
- **Error Handling**: Centralized error handling with meaningful error messages
- **Security**: Password hashing, SQL injection prevention, and secure API endpoints

### Testing

#### Server Tests
- **Unit Tests**: Core business logic covered with xUnit tests
- **Integration Tests**: API endpoint testing with in-memory database
- **Test Coverage**: Key components tested for reliability and maintainability

To run the server tests:
1. Navigate to the `server` directory:
   ```bash
   cd server
   ```
2. Run all tests:
   ```bash
   dotnet test
   ```

#### Client Tests
To run the client tests:
1. Navigate to the `client` directory:
   ```bash
   cd client
   ```
2. Run all tests:
   ```bash
   npm test
   ```

If you rad so far you are awesome. thank you.