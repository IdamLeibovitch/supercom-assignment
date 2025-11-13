## Full-Stack Developer Home Assignment
### Overview
Develop a web application for managing user tasks.

Each Task should contain:  
Title, description Due Date and priority.  
Also, should include the user details: Full name, Telephone and email address.

Please have validation on all fields
The application will consist of a .NET Core backend, a React frontend, and SQL Server for data
storage.

The application should use Entity Framework for database interactions, and a state
management tool like MobX or Redux in React.

### Requirements:
1. Backend (API) - .NET Core
    - Create a RESTful API using .NET Core.
    - Implement CRUD (Create, Read, Update, Delete) operations for tasks.
    - Use Entity Framework for database operations, ensuring optimal data handling.
2. Frontend - React
    - Develop a React application to interface with the backend API.
    - Implement state management using a tool like MobX or Redux. This is critical for
handling the application&#39;s state efficiently, especially for features like loading states,
error messages, and user input forms.
    - Design a user interface that allows users to view, add, update, and delete tasks,
ensuring a responsive and user-friendly experience.

3. Database - SQL Server
    - Design an appropriate database schema for managing tasks.
    - Create necessary tables and relationships, ensuring data integrity and consistency.
4. Windows Service and Queues RabbitMQ (bonus)
    - Windows Service will pull the tasks and if the due date is over then should insert into
the queue Remainder on the Task.
    - Same service will subscribe to the queue and will log each action comes from that
queue (log will say: “Hi your Task is due {Task xxxxx}”
    - Focus on effectively handling concurrent updates through the queue.

#### Evaluation Criteria

  - Code Quality: High-quality, readable, and maintainable code with appropriate design
patterns.
  - **Functionality: Full implementation of the specified requirements.**  
  - **No bugs are allowed—Testing your application carefully before you submit; basic flow should be flawless.**
  - Entity Framework Usage: Proficient use of Entity Framework for database operations.
  - State Management: Skillful implementation of state management in React.
  - Error Handling: Robust handling of potential issues and exceptions.
  - Testing: Comprehensive testing for both front-end and back-end components.
  - Documentation: Detailed README with setup instructions, architectural overview, and
explanations of key implementations.

#### Submission Guidelines:

  - Submit the project via a Git repository.
  - Include all necessary setup scripts and instructions.
  - Ensure that the application can be easily set up and run in a local environment.