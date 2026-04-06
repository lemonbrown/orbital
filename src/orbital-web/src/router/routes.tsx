import { createBrowserRouter } from 'react-router-dom'
import { Layout } from '../components/Layout'
import { AuthGuard } from '../features/auth/AuthGuard'
import { DashboardPage } from '../features/auth/DashboardPage'
import { HomePage } from '../features/auth/HomePage'
import { LoginPage } from '../features/auth/LoginPage'
import { RegisterPage } from '../features/auth/RegisterPage'
import { PublicJoinPage } from '../features/rings/PublicJoinPage'
import { PublicDirectoryPage } from '../features/rings/PublicDirectoryPage'
import { RingDetailPage } from '../features/rings/RingDetailPage'
import { RingListPage } from '../features/rings/RingListPage'

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
      { path: 'rings/:slug/join', element: <PublicJoinPage /> },
      { path: 'rings/:slug/directory', element: <PublicDirectoryPage /> },
      {
        element: <AuthGuard />,
        children: [
          { path: 'dashboard', element: <DashboardPage /> },
        ],
      },
    ],
  },
])
