# Rhythmic Gymnastics Tracking - Frontend

This is a React + Vite frontend application designed specifically to interface with the C# .NET API.

## 🚀 How to Launch the Application

To run the complete system, you must start BOTH the backend API and the frontend React application.

### 1. Start the Backend API (C#)
1. Open your terminal or IDE (Visual Studio / Rider).
2. Navigate to the API folder or launch the `CompetitionsTracking` profile.
3. If using terminal:
   ```bash
   cd "C:\Users\filip\OneDrive\Desktop\2 term\CourseWork\CompetitionsTracking\CompetitionsTracking"
   dotnet run --launch-profile "https"
   ```
   **Important:** Ensure the API is running on `https://localhost:7286`.

### 2. Start the Frontend Application (React)
1. Open a new terminal.
2. Navigate to the `frontend` folder:
   ```bash
   cd "C:\Users\filip\OneDrive\Desktop\2 term\CourseWork\CompetitionsTracking\frontend"
   ```
3. Install dependencies (if you haven't already):
   ```bash
   npm install
   ```
4. Run the development server:
   ```bash
   npm run dev
   ```
5. Open your browser and go to `http://localhost:5173`.

### 🔐 Authentication
The frontend connects to the `/api/Auth/login` endpoint. To test it:
*   Log in with a mocked User or Admin email and password that exists in your database or run your Database Seeding logic first.

## Modern UI Design
The styling uses pure Vanilla CSS following a strict Premium Dark/White/Grey theme. The CSS is modular, located directly beside the React JSX components in `src/pages` and `src/components`.

## Error Handling
Validations and network errors are globally caught and displayed as sleek toasts in the top right corner. The frontend distinguishes between unauthenticated (`401`), forbidden access (`403`), and bad validation input (`400`).
