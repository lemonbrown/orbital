import { useState } from 'react'
import { Link } from 'react-router-dom'
import { Button } from '../../components/Button'
import { Card } from '../../components/Card'
import { Input } from '../../components/Input'
import { useRegister } from '../../hooks/useAuth'

export function RegisterPage() {
  const [form, setForm] = useState({ username: '', email: '', password: '' })
  const register = useRegister()

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    register.mutate(form)
  }

  return (
    <div className="flex min-h-[80vh] items-center justify-center">
      <div className="w-full max-w-md">
        <div className="mb-8 text-center">
          <h1 className="text-3xl font-bold text-gray-900">Join Orbital</h1>
          <p className="mt-2 text-gray-500">Create your account and start building web rings.</p>
        </div>
        <Card>
          <form onSubmit={handleSubmit} className="flex flex-col gap-4">
            <Input
              id="username"
              label="Username"
              value={form.username}
              onChange={(e) => setForm((f) => ({ ...f, username: e.target.value }))}
              placeholder="cooldev"
              required
            />
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
              placeholder="At least 8 characters"
              required
            />
            {register.error && (
              <p className="text-sm text-red-600">
                {(register.error as any)?.response?.data?.detail ?? 'Registration failed.'}
              </p>
            )}
            <Button type="submit" loading={register.isPending} className="mt-2">
              Create account
            </Button>
          </form>
        </Card>
        <p className="mt-4 text-center text-sm text-gray-500">
          Already have an account?{' '}
          <Link to="/login" className="text-violet-600 hover:underline">
            Sign in
          </Link>
        </p>
      </div>
    </div>
  )
}
