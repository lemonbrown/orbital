import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { api } from '../lib/api'

export type RingVisibility = 'Public' | 'Unlisted' | 'Private'
export type MembershipStatus = 'Pending' | 'Approved' | 'Rejected'
export type MembershipRole = 'Owner' | 'Moderator' | 'Member'

export interface RingListItem {
  id: string
  name: string
  slug: string
  description: string
  visibility: RingVisibility
  memberCount: number
  createdAt: string
}

export interface Membership {
  id: string
  siteId: string
  role: MembershipRole
  status: MembershipStatus
  orderIndex: number
  joinedAt: string
}

export interface Edge {
  id: string
  fromSiteId: string
  toSiteId: string
  dimension: string
  label: string
  weight: number
}

export interface RingDetail extends RingListItem {
  ownerUserId: string
  memberships: Membership[]
  edges: Edge[]
}

export function usePublicRings() {
  return useQuery({
    queryKey: ['rings', 'public'],
    queryFn: async () => {
      const res = await api.get<RingListItem[]>('/rings/public')
      return res.data
    },
  })
}

export function useMyRings() {
  return useQuery({
    queryKey: ['rings', 'mine'],
    queryFn: async () => {
      const res = await api.get<RingListItem[]>('/rings/mine')
      return res.data
    },
  })
}

export function useRing(id: string) {
  return useQuery({
    queryKey: ['rings', id],
    queryFn: async () => {
      const res = await api.get<RingDetail>(`/rings/${id}`)
      return res.data
    },
    enabled: !!id,
  })
}

export function useCreateRing() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: async (data: {
      name: string
      description?: string
      ownerSiteId: string
      visibility?: RingVisibility
    }) => {
      const res = await api.post<{ id: string; name: string; slug: string; description: string }>('/rings', data)
      return res.data
    },
    onSuccess: () => qc.invalidateQueries({ queryKey: ['rings'] }),
  })
}

export function useJoinRing() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: async ({ ringId, siteId }: { ringId: string; siteId: string }) => {
      const res = await api.post(`/rings/${ringId}/join`, { siteId })
      return res.data
    },
    onSuccess: () => qc.invalidateQueries({ queryKey: ['rings'] }),
  })
}

export function useApproveMember() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: async ({ ringId, membershipId }: { ringId: string; membershipId: string }) => {
      const res = await api.post(`/rings/${ringId}/memberships/${membershipId}/approve`)
      return res.data
    },
    onSuccess: () => qc.invalidateQueries({ queryKey: ['rings'] }),
  })
}

export function useRejectMember() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: async ({ ringId, membershipId }: { ringId: string; membershipId: string }) => {
      await api.post(`/rings/${ringId}/memberships/${membershipId}/reject`)
    },
    onSuccess: () => qc.invalidateQueries({ queryKey: ['rings'] }),
  })
}
