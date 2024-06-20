LeetCode Clone with .NET 8.0

Under development....

This project aims to create a scalable, and secure coding challenge platform similar to LeetCode. Utilizing modern technologies to provide a seamless user experience and ensure the safety and integrity of user code submissions.
Features

    IdentityUser: Manages user authentication and profiles securely.
    JWT Authentication: Provides secure token-based authentication.
    PostgreSQL: Reliable and efficient database management.
    Docker: Safe and isolated environments for code compilation and execution.

Technologies Used

    .NET 8.0: Core framework for developing the application.
    IdentityUser: ASP.NET Core Identity for user management.
    JWT: Secure JSON Web Tokens for authentication.
    PostgreSQL: Robust relational database system.
    Docker: Containerization for secure and isolated code execution environments.

how to run:
- navigate into compilation-service directory 
- docker build -t compilation-service . 
- docker run -p 5144:5144 compilation-service 
- should run informaticsge with your IDE 
