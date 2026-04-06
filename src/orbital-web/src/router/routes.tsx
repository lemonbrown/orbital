import { createBrowserRouter } from 'react-router-dom'
import { Layout } from '../components/Layout'
import { AuthGuard } from '../features/auth/AuthGuard'
import { DashboardPage } from '../features/auth/DashboardPage'
import { HomePage } from '../features/auth/HomePage'
import { LoginPage } from '../features/auth/LoginPage'
import { RegisterPage } from '../features/auth/RegisterPage'
import { RingDetailPage } from '../features/rings/RingDetailPage'
import { RingListPage } from '../features/rings/RingListPage'
import { SiteListPage } from '../features/sites/SiteListPage'

export const router = createBrowserRouter([
  {
    path: '/',
    element: <Layout />,
    children: [
      { index: true, element: <HomePage /> },
      { path: 'login', element: <LoginPage /> },
      { path: 'register', element: <RegisterPage /> },
      { path: 'rings', element: <RingListPage /> },
      { path: 'rings/:id', element: <RingDetailPage /> },
      {
        element: <AuthGuard />,
        children: [
          { path: 'dashboard', element: <DashboardPage /> },
          { path: 'sites', element: <SiteListPage /> },
        ],
      },
    ],
  },
])
