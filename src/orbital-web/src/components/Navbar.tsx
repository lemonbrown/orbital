import { Link, NavLink } from 'react-router-dom'
import { useCurrentUser, useLogout } from '../hooks/useAuth'
import { Button } from './Button'

export function Navbar() {
  const user = useCurrentUser()
  const logout = useLogout()

  return (
    <nav className="border-b border-gray-200 bg-white">
      <div className="mx-auto max-w-5xl px-4 flex h-14 items-center justify-between">
        <Link to="/" className="font-bold text-lg text-violet-600 tracking-tight">
          ⬡ Orbital
        </Link>
        <div className="flex items-center gap-4">
          {user ? (
            <>
              <NavLink
                to="/dashboard"
                className={({ isActive }) =>
                  `text-sm font-medium ${isActive ? 'text-violet-600' : 'text-gray-600 hover:text-gray-900'}`
                }
              >
                Dashboard
              </NavLink>
              <NavLink
                to="/sites"
                className={({ isActive }) =>
                  `text-sm font-medium ${isActive ? 'text-violet-600' : 'text-gray-600 hover:text-gray-900'}`
                }
              >
                Sites
              </NavLink>
              <NavLink
                to="/rings"
                className={({ isActive }) =>
                  `text-sm font-medium ${isActive ? 'text-violet-600' : 'text-gray-600 hover:text-gray-900'}`
                }
              >
                Rings
              </NavLink>
              <span className="text-sm text-gray-500">{user.username}</span>
              <Button variant="ghost" size="sm" onClick={logout}>
                Sign out
              </Button>
            </>
          ) : (
            <>
              <Link to="/login" className="text-sm font-medium text-gray-600 hover:text-gray-900">
                Sign in
              </Link>
              <Button size="sm" onClick={() => window.location.href = '/register'}>
                Get started
              </Button>
            </>
          )}
        </div>
      </div>
    </nav>
  )
}
