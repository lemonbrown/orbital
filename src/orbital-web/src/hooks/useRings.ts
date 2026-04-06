import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { api } from '../lib/api'

export type RingVisibility = 'Public' | 'Unlisted' | 'Private'
export type MembershipStatus = 'PendingVerification' | 'PendingApproval' | 'Approved' | 'Rejected'
export type MembershipRole = 'Owner' | 'Moderator' | 'Member'
export type VerificationMode = 'None' | 'Widget' | 'Auto'
export type ApprovalMode = 'Auto' | 'Manual'
export type CrawlFrequency = 'Every6Hours' | 'Daily' | 'Every3Days' | 'Weekly'

export interface ActivityConfig {
  isEnabled: boolean
  crawlingEnabled: boolean
  recentPostWeight: number
  recentUpdateWeight: number
  activityWindowDays: number
  crawlFrequency: CrawlFrequency
  skipStaleSites: boolean
  staleSiteThresholdDays: number
}

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
  siteName: string
  siteUrl: string
  role: MembershipRole
  status: MembershipStatus
  orderIndex: number
  joinedAt: string
  applicantName: string | null
  contactEmail: string | null
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
  verificationMode: VerificationMode
  approvalMode: ApprovalMode
  isPublicDirectoryEnabled: boolean
  isPublicJoinEnabled: boolean
  isApiOnboardingEnabled: boolean
  memberships: Membership[]
  edges: Edge[]
  activityConfig: ActivityConfig
}

export interface ApiKey {
  id: string
  label: string
  keyPrefix: string
  createdAt: string
  isRevoked: boolean
  revokedAt: string | null
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

export function useRingBySlug(slug: string) {
  return useQuery({
    queryKey: ['rings', 'slug', slug],
    queryFn: async () => {
      const res = await api.get<RingDetail>(`/rings/slug/${slug}`)
      return res.data
    },
    enabled: !!slug,
  })
}

export interface DirectorySite {
  siteId: string
  name: string
  url: string
  description: string
  orderIndex: number
}

export function useRingDirectory(slug: string) {
  return useQuery({
    queryKey: ['rings', 'slug', slug, 'directory'],
    queryFn: async () => {
      const res = await api.get<DirectorySite[]>(`/rings/slug/${slug}/directory`)
      return res.data
    },
    enabled: !!slug,
  })
}

export function useCreateRing() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: async (data: {
      name: string
      description?: string
      ownerSiteId?: string
      visibility?: RingVisibility    }) => {
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

export function useCheckSnippet() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: async ({ ringId, membershipId }: { ringId: string; membershipId: string }) => {
      const res = await api.post<{ found: boolean; status: MembershipStatus; message: string }>(
        `/rings/${ringId}/memberships/${membershipId}/check-snippet`
      )
      return res.data
    },
    onSuccess: () => qc.invalidateQueries({ queryKey: ['rings'] }),
  })
}

export function useUpdateRingSettings() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: async (data: {
      ringId: string
      isPublicJoinEnabled: boolean
      isApiOnboardingEnabled: boolean
      isPublicDirectoryEnabled: boolean
      verificationMode: VerificationMode
      approvalMode: ApprovalMode
    }) => {
      await api.patch(`/rings/${data.ringId}/settings`, {
        isPublicJoinEnabled: data.isPublicJoinEnabled,
        isApiOnboardingEnabled: data.isApiOnboardingEnabled,
        isPublicDirectoryEnabled: data.isPublicDirectoryEnabled,
        verificationMode: data.verificationMode,
        approvalMode: data.approvalMode,
      })
    },
    onSuccess: (_data, vars) => qc.invalidateQueries({ queryKey: ['rings', vars.ringId] }),
  })
}

export function useApiKeys(ringId: string) {
  return useQuery({
    queryKey: ['rings', ringId, 'api-keys'],
    queryFn: async () => {
      const res = await api.get<ApiKey[]>(`/rings/${ringId}/api-keys`)
      return res.data
    },
    enabled: !!ringId,
  })
}

export function useCreateApiKey() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: async ({ ringId, label }: { ringId: string; label: string }) => {
      const res = await api.post<{ keyId: string; label: string; plainKey: string; createdAt: string }>(
        `/rings/${ringId}/api-keys`,
        { label }
      )
      return res.data
    },
    onSuccess: (_data, vars) => qc.invalidateQueries({ queryKey: ['rings', vars.ringId, 'api-keys'] }),
  })
}

export function useRevokeApiKey() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: async ({ ringId, keyId }: { ringId: string; keyId: string }) => {
      await api.delete(`/rings/${ringId}/api-keys/${keyId}`)
    },
    onSuccess: (_data, vars) => qc.invalidateQueries({ queryKey: ['rings', vars.ringId, 'api-keys'] }),
  })
}

export function useRemoveMember() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: async ({ ringId, membershipId }: { ringId: string; membershipId: string }) => {
      await api.delete(`/rings/${ringId}/memberships/${membershipId}`)
    },
    onSuccess: (_data, vars) => qc.invalidateQueries({ queryKey: ['rings', vars.ringId] }),
  })
}

export function useDeleteRing() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: async (ringId: string) => {
      await api.delete(`/rings/${ringId}`)
    },
    onSuccess: () => qc.invalidateQueries({ queryKey: ['rings'] }),
  })
}

export function useUpdateActivityConfig() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: async (data: { ringId: string } & ActivityConfig) => {
      await api.patch(`/rings/${data.ringId}/activity-config`, {
        isEnabled: data.isEnabled,
        crawlingEnabled: data.crawlingEnabled,
        recentPostWeight: data.recentPostWeight,
        recentUpdateWeight: data.recentUpdateWeight,
        activityWindowDays: data.activityWindowDays,
        crawlFrequency: data.crawlFrequency,
        skipStaleSites: data.skipStaleSites,
        staleSiteThresholdDays: data.staleSiteThresholdDays,
      })
    },
    onSuccess: (_data, vars) => qc.invalidateQueries({ queryKey: ['rings', vars.ringId] }),
  })
}
