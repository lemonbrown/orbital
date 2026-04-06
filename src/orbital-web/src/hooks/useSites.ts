import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { api } from '../lib/api'

export interface Site {
  id: string
  name: string
  url: string
  description: string
  verificationStatus: 'Pending' | 'Verified' | 'Failed'
  verificationToken: string
  createdAt: string
}

export function useMySites() {
  return useQuery({
    queryKey: ['sites', 'mine'],
    queryFn: async () => {
      const res = await api.get<Site[]>('/sites')
      return res.data
    },
  })
}

export function useAddSite() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: async (data: { name: string; url: string; description?: string }) => {
      const res = await api.post<Site>('/sites', data)
      return res.data
    },
    onSuccess: () => qc.invalidateQueries({ queryKey: ['sites'] }),
  })
}

export function useVerifySite() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: async (siteId: string) => {
      const res = await api.post<{ success: boolean; status: string; message: string }>(
        `/sites/${siteId}/verify`
      )
      return res.data
    },
    onSuccess: () => qc.invalidateQueries({ queryKey: ['sites'] }),
  })
}
