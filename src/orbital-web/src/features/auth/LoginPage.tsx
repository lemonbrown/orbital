import { useState } from 'react'
import { Link } from 'react-router-dom'
import { Button } from '../../components/Button'
import { Card } from '../../components/Card'
import { Input } from '../../components/Input'
import { useLogin } from '../../hooks/useAuth'

export function LoginPage() {
  const [form, setForm] = useState({ email: '', password: '' })
  const login = useLogin()

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    login.mutate(form)
  }

  return (
    <div className="flex min-h-[80vh] items-center justify-center">
      <div className="w-full max-w-md">
        <div className="mb-8 text-center">
          <h1 className="text-3xl font-bold text-gray-900">Welcome back</h1>
          <p className="mt-2 text-gray-500">Sign in to manage your web rings.</p>
        </div>
        <Card>
          <form onSubmit={handleSubmit} className="flex flex-col gap-4">
            <Input
              id="email"
              label="Email"
              type="email"
              value={form.email}
              onChange={(e) => setForm((f) => ({ ...f, email: e.target.value }))}
              placeholder="you@example.com"
              required
            />
            <Input
              id="password"
              label="Password"
              type="password"
              value={form.password}
              onChange={(e) => setForm((f) => ({ ...f, password: e.target.value }))}
              placeholder="Your password"
              required
            />
            {login.error && (
              <p className="text-sm text-red-600">
                {(login.error as any)?.response?.data?.detail ?? 'Invalid credentials.'}
              </p>
            )}
            <Button type="submit" loading={login.isPending} className="mt-2">
              Sign in
            </Button>
          </form>
        </Card>
        <p className="mt-4 text-center text-sm text-gray-500">
          Don't have an account?{' '}
          <Link to="/register" className="text-violet-600 hover:underline">
            Sign up
          </Link>
        </p>
      </div>
    </div>
  )
}
