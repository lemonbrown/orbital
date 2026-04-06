import { useMutation } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { api } from '../lib/api'
import { clearAuth, getUser, setToken, setUser, type StoredUser } from '../lib/auth'

interface AuthResponse {
  userId: string
  username: string
  email: string
  token: string
}

export function useCurrentUser(): StoredUser | null {
  return getUser()
}

export function useRegister() {
  const navigate = useNavigate()
  return useMutation({
    mutationFn: async (data: { username: string; email: string; password: string }) => {
      const res = await api.post<AuthResponse>('/auth/register', data)
      return res.data
    },
    onSuccess: (data) => {
      setToken(data.token)
      setUser({ userId: data.userId, username: data.username, email: data.email })
      navigate('/dashboard')
    },
  })
}

export function useLogin() {
  const navigate = useNavigate()
  return useMutation({
    mutationFn: async (data: { email: string; password: string }) => {
      const res = await api.post<AuthResponse>('/auth/login', data)
      return res.data
    },
    onSuccess: (data) => {
      setToken(data.token)
      setUser({ userId: data.userId, username: data.username, email: data.email })
      navigate('/dashboard')
    },
  })
}

export function useLogout() {
  const navigate = useNavigate()
  return () => {
    clearAuth()
    navigate('/login')
  }
}
