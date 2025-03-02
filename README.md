# Lightweight HTTP Server with PHP CGI

## Overview
This project is a lightweight HTTP server designed to improve my understanding of HTTP, server-side execution, and routing. It supports **server-side PHP execution via CGI**, **customizable routing**, and **static file serving**.

## cgi_debug.php Results:
![CgiDebug2](https://github.com/user-attachments/assets/844b7255-823b-4aff-b580-851c9b1b3cf4)

## Features
- **Server-side PHP script execution** via CGI
- **User-defined routing** via `config.json`
- **Serving of static resources**
- **Asynchronous client handling**
  - Tracks running processes for **graceful shutdown**

## Configuration (`config.json`)

### **Server Settings**
- `Host`: The IP or hostname to bind the listener to
- `Port`: The port number to bind the listener to

### **PHP Settings**
- `PhpCgiPath`: Absolute path to your local `php-cgi.exe`
- `DocumentRoot`: Root directory for the server (relative to the **solution directory**)

### **Routing**
- `Enabled`:  
  - `true` → Routes **all incoming requests** using the mappings defined below  
  - `false` → Requests are resolved using the **document root**
- `Routes`: An array of route mappings:
  - `Route`: The expected **incoming URI**
  - `Resource`: The corresponding **local file** to serve

## **Future Updates?**
- **Improve Routing Implementation**  
  - Currently, **CSS and other resources must be explicitly defined** in routes if linked in an HTML document.  
    - Possible solutions:
      - Embed static resources inside the response?
      - Define sections of the `htdocs` folder that **do not** require explicit routes?
  
- **Basic UI for Configuration & Status**  
  - A **WPF-based control panel** (similar to XAMPP) for managing settings and status.

- **Authentication & HTTPS Support**  
  - Investigate requirements to upgrade to **HTTPS** and implement **basic authentication**.
