# encrypted-file-sharing-system

A secure web platform for uploading, encrypting, storing, and sharing files between users. The system uses modern cryptographic techniques to protect user data and manage file access securely.

---

## ⚙️ Key Features

- 📝 User registration with validation (email format, unique login, password strength)
- 🔐 JWT-based user authentication (Bearer Token)
- 📤 Uploading and encrypting files using AES
- 🔑 AES key and IV are encrypted using the user's RSA public key
- 📁 Folder creation and management
- 👥 Share files with other users via email or login
- ✅ Data integrity verification

---

## 🧰 Technologies Used

### 📦 Frontend

- **Angular 18** — modern SPA framework with component-based architecture and routing
- **NgRx** — centralized state management using Redux pattern
- **RxJS** — reactive programming with observable streams
- **TailwindCSS** — utility-first CSS framework for fast and responsive UI styling
- **PrimeNG** — rich UI component library for Angular

### 🖥 Backend

- **ASP.NET Core 8** — high-performance framework for building RESTful APIs
- **Entity Framework Core 8** — ORM for working with relational databases
- **MSSQL Server** — relational database used to store metadata and user data

### 🔐 Cryptography & Security

- **AES (Advanced Encryption Standard)** — symmetric encryption of files
- **RSA** — asymmetric encryption of AES keys and IVs
- **SHA-256** — data integrity verification

---

## 📸 Feature Demonstration

### 1. 🔐 User Registration

Users register using their email (or login) and password. The form includes input validation.

![Registration Form](./assets/images/registration-form.jpeg)  
*Registration form on desktop*

![Form Validation](./assets/images/registration-validation.jpeg)  
*Validation of form inputs*

---

### 2. 🔓 User Login

Users log in with their credentials. The platform returns a JWT token, which is used for authenticating future requests.

---

### 3. ☁️ File Upload and Encryption

Files uploaded from the **"My Files"** page are automatically encrypted using AES. The AES key and IV are encrypted with the user's RSA public key and stored securely.

![My Files Page](./assets/images/my-files.jpeg)  
*Main view of uploaded files*

![Upload or Create Folder](./assets/images/create-or-upload.jpeg)  
*Option to upload files or create folders*

![Creating Folder](./assets/images/create-folder.jpeg)  
*Creating a new folder for file storage*

![Uploading Files](./assets/images/upload-file.jpeg)  
*Uploading new encrypted files*

---

### 4. 🤝 File Sharing

Users can share files with others by entering their email or login. Shared files appear in the **"Files Shared With Me"** section. Files the user has shared appear in the **"Shared Files"** section.

---

## 🎬 Video Demo

[![Watch the video](./assets/images/video-placeholder.jpeg)](https://youtu.be/G6BO-GAtm4I?si=FOM3L9bMkk4ILy_3)

---

## 🚀 Getting Started

### Start Backend

```bash
cd src/services
dotnet run
```

### Start Frontend

```bash
cd src/ui
npm install
npm start
```

---

## 👨‍💻 Author

**Roman Melnychuk**  
📧 Email: roma.melnychuk2013@gmail.com  
[Telegram](https://t.me/roman_melnychuk17)
