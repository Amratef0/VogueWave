# E-Commerce Website - ASP.NET MVC

## Overview

This project is an E-Commerce website developed using ASP.NET MVC. It serves as an online store for selling clothes and accessories, featuring robust user authentication and role management. The application allows administrators to manage products and categories, while regular users can browse and purchase items.

## Features

* User Authentication and Authorization (Login, Registration, Password Management)
* Role-based Access Control (Admin, User)
* Product Management (Add, Edit, Delete, View)
* Category Management
* Shopping Cart and Checkout Process
* Email Notifications (SMTP Integration)
* Blog Integration
* Profile Management
* Order and Payment Management

## Project Structure

### Controllers

* AccountController, BlogController, MainController, PaymentController, ProfileController, RoleController, ServiceController

### Models

* User: ApplicationUser, LoginUserViewModel, RegisterViewModel
* Products: Product, ProductImage, Category
* Orders: Order, OrderProduct, OrderStatus
* Misc: CartItem, EmailSender, SmtpSettings

### Views

* Account: ForgotPassword, ForgotPasswordConfirm, Login, Register, ResetPassword
* Blog: Product List
* Main: Home, About, Product Details, Cart, Checkout, Manage Products, Categories
* Profile: User Profile Management
* Shared: Layout and Error Pages

## Mocking and Testing

During development, mocking techniques are used to isolate components and ensure reliable testing without the need for real data sources. For example:

* Mocking email sending during the registration process to verify user accounts without actual SMTP interactions.
* Mocking email sending to avoid actual SMTP interactions.

## Technologies Used

* ASP.NET MVC
* Entity Framework (Code First)
* SQL Server
* SMTP for Email Notifications
* HTML, CSS, Bootstrap for Front-End
* jQuery for Interactivity

## Setup Instructions

1. Clone the repository.
2. Update SMTP settings in appsettings.json.
3. Run database migrations.
4. Start the application using Visual Studio.

## License

This project is licensed under the MIT License.

## Contact

For any inquiries or issues, please reach out to Amr at [Amr45409@gmail.com](mailto:Amr45409@gmail.com).
