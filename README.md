# LeetCode Clone with .NET 8.0

Under development....

This project aims to create a scalable and secure coding challenge platform similar to LeetCode. Utilizing modern technologies to provide a seamless user experience and ensure the safety and integrity of user code submissions.

## Features

- **IdentityUser**: Manages user authentication and profiles securely.
- **JWT Authentication**: Provides secure token-based authentication.
- **PostgreSQL**: Reliable and efficient database management.
- **Docker**: Safe and isolated environments for code compilation and execution.

## Technologies Used

- **.NET 8.0**: Core framework for developing the application.
- **IdentityUser**: ASP.NET Core Identity for user management.
- **JWT**: Secure JSON Web Tokens for authentication.
- **PostgreSQL**: Robust relational database system.
- **Docker**: Containerization for secure and isolated code execution environments.

## How to Run

1. **Navigate into the `compilation-service` directory**

    ```bash
    cd compilation-service
    ```

2. **Build the Docker image**

    ```bash
    docker build -t compilation-service .
    ```

3. **Run the Docker container**

    ```bash
    docker run -p 5144:5144 compilation-service
    ```

4. **Run the project with your IDE**

    Open your IDE and run the project as usual.
