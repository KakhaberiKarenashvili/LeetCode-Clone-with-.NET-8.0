# LeetCode Clone with .NET 8.0

Project Status: This project is in active development and is not production-ready.  
Breaking changes are expected until the first stable release.

This is a LeetCode inspired coding challenge platform built with .NET 8.0.  
It allows users to solve coding problems and have their code executed securely inside Docker containers.

## Why Docker

This platform must run inside Docker because the code execution service runs in a Linux-based container with:
- Python (for Python challenges)
- GCC (for C/C++ challenges)
- Secure, isolated execution to prevent malicious code from affecting the host machine.

## Features

### User Management
- ASP.NET Core Identity for user authentication and profile management.
- JWT Authentication for secure token-based login.

### Data Layer
- PostgreSQL as the relational database.
- Flexible database configuration.

### Code Execution
- Docker-based sandboxing for safe compilation and execution.


## Technologies Used
- .NET 8.0
- ASP.NET Core Identity
- JWT (JSON Web Tokens)
- PostgreSQL
- Docker & Docker Compose

## Architecture
![Architecture Diagram](docs/architecture.jpeg)

## Getting Started

### Prerequisites
- Install Docker and Docker Compose
- Install .NET 8 SDK (only required if you plan to develop outside Docker)

### Setup Instructions

Step 1 – Clone the repository
```bash
  git clone https://github.com/username/leetcode-clone.git
  cd leetcode-clone
```

Step 2 – (Optional) Configure database and message broker
By default, PostgreSQL (database) and RabbitMQ (message broker) are configured to run as services inside Docker using docker-compose.yml.
No changes are needed to run the project as is.

If you want to use your own database or message broker instance, update the connection strings in `appsettings.json`:
```json
"ConnectionStrings": {
  "DBConnection": "Host=host.docker.internal;Port=5432;Database=postgres;Username=postgres;Password=postgres;"
}
```

```json
"MessageBroker": {
  "Host": "amqp://rabbitmq:5672",
  "UserName": "user",
  "Password": "password"
}
```

Step 3 – Start the application
```bash
  docker-compose up --build
```
Notes

- The project must run in Docker to ensure safe code execution.

-  The execution container runs Linux with Python and GCC installed.

- This project is under heavy development and may change frequently.

