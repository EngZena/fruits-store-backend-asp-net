# Fruits-store-backend-asp-net

Welcome to the **fruits-store-backend-asp-net** project! This is a simple REST API built with C# and ASP.NET Core.

## Table of Contents

- [Introduction](#introduction)
- [Features](#features)
- [Getting Started](#getting-started)
- [Usage](#usage)
- [API Endpoints](#api-endpoints)
- [Contributing](#contributing)

## Introduction

This project is a RESTful API designed to manage a collection of fruits. It demonstrates the use of ASP.NET Core to build a robust and scalable web service.

## Features

- CRUD operations for resources
- Authentication and Authorization
- Error handling and validation
- Swagger documentation

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio Code](https://code.visualstudio.com/) or any other C# IDE

### Installation

1. Clone the repository:
    ```bash
    git clone https://github.com/EngZena/fruits-store-backend-asp-net.git
    cd fruits-store-backend-asp-net
    ```

2. Restore the dependencies:
    ```bash
    dotnet restore
    ```

3. Build the project:
    ```bash
    dotnet build
    ```

4. Run the application:
    ```bash
    dotnet run
    ```

## Usage

Once the application is running, you can access the API at `http://localhost:5242`. Use tools like [Postman](https://www.postman.com/) or [curl](https://curl.se/) to interact with the API.

## API Endpoints

### Authentication Endpoints

- **POST** `/auth/login` - Authenticate a user and retrieve a token
- **POST** `/auth/register` - Register a new user user account using some details
- **POST** `/auth/SignUp` - Register a new user using an email and password
- **POST** `/auth/RequestResetPassword` - Returns a GUID to submit new password
- **POST** `/auth/SubmitNewPassword/{userGuid}/{userEmail}` - Submit New Password using secret GUID


### Fruits Endpoints

- **GET** `/Fruit/GetFruits` - Retrieve all fruits
- **GET** `/Fruit/GetFruitsByType/{FruitType}` - Retrieves fruits by Fruit Type
- **GET** `/Fruit/GetSingleFruit/{FruitId}` - Retrieve a specific fruit by ID
- **GET** `/Fruit/GetFruitsCreatedByUserId/{UserId}` - Retrieves all fruits added by User Id
- **GET** `/Fruit/GetFruitsCreatedByCurrentUser` - Retrieves all fruits added by Current User
- **POST** `/Fruit/AddFruit` - Create a new fruit
- **PUT** `/Fruit/EditFruit` - Update an existing fruit

### Authorization Endpoints
- **DELETE** `/Fruit/DeleteFruit/{FruitId}` - Delete a fruit added by the current user using the fruit ID.

## Contributing

Contributions are welcome! Please fork the repository and submit a pull request with your changes. Ensure your code follows the project's coding standards and includes appropriate tests.

---

Thank you for checking out **fruits-store-backend-asp-net**! If you have any questions or need further assistance, feel free to open an issue.
