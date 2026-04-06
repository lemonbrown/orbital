import { useMutation } from '@tanstack/react-query'
import { api } from '../lib/api'
import type { MembershipStatus } from './useRings'

export interface SubmitApplicationResult {
  membershipId: string
  siteId: string
  status: MembershipStatus
  snippetHtml: string | null
  checkSnippetUrl: string | null
}

export function useSubmitApplication() {
  return useMutation({
    mutationFn: async (data: {
      ringId: string
      siteUrl: string
      siteName: string
      description?: string
      contactEmail?: string
      applicantName?: string
    }) => {
      const res = await api.post<SubmitApplicationResult>('/applications', data)
      return res.data
    },
  })
}
