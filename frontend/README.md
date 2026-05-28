# Frontend

Angular application for the Fundo Loan Management System.

## Prerequisites

- [Node.js](https://nodejs.org/) (LTS recommended)
- The backend API running on `https://localhost:5001` (see `backend/src/README.md`)

## Running the Frontend

Install dependencies:

```sh
npm install
```

Start the development server:

```sh
npm start
```

Open `http://localhost:4200/` in your browser.

---

## Authentication

The app uses JWT Bearer authentication. On first load it redirects to `/login`.

**Default credentials (seeded automatically by the backend):**

| Username | Password  |
|----------|-----------|
| `admin`  | `admin123` |

After a successful login the token is stored in `localStorage` and attached automatically to every API request via an HTTP interceptor. Navigating to `/loans` without a valid token redirects back to `/login`.

---

## Project structure

```
src/app/
├── auth.guard.ts          # Route guard — redirects to /login when unauthenticated
├── auth.interceptor.ts    # HTTP interceptor — attaches Bearer token to every request
├── auth.model.ts          # LoginRequest / TokenResponse interfaces
├── auth.service.ts        # Login, logout, token signal (localStorage-backed)
├── loan.model.ts          # Loan interface
├── loan.service.ts        # HTTP service for /loans endpoints
├── login/                 # Login page component
└── loans/                 # Loan table component (protected route)
```

## Routes

| Path     | Access      | Description            |
|----------|-------------|------------------------|
| `/login` | Public      | Login form             |
| `/loans` | Requires JWT | Loan management table |
| `/`      | —           | Redirects to `/loans`  |
