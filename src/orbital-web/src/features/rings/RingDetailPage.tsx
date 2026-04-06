import { useState } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { Badge } from '../../components/Badge'
import { Button } from '../../components/Button'
import { Card } from '../../components/Card'
import { useJoinRing, useRing, useDeleteRing } from '../../hooks/useRings'
import { useMySites } from '../../hooks/useSites'
import { getUser } from '../../lib/auth'
import { MemberApprovalList } from './MemberApprovalList'
import { NavigatorWidget } from '../navigation/NavigatorWidget'
import { RingSettingsPanel } from './RingSettingsPanel'

type Tab = 'overview' | 'settings'

export function RingDetailPage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const { data: ring, isLoading } = useRing(id!)
  const { data: mySites } = useMySites()
  const joinRing = useJoinRing()
  const deleteRing = useDeleteRing()
  const [joinSiteId, setJoinSiteId] = useState('')
  const [tab, setTab] = useState<Tab>('overview')
  const user = getUser()

  if (isLoading) return <p className="text-gray-500">Loading…</p>
  if (!ring) return <p className="text-red-600">Ring not found.</p>

  const isOwner = ring.ownerUserId === user?.userId
  const approvedSiteIds = new Set(
    ring.memberships.filter((m) => m.status === 'Approved').map((m) => m.siteId)
  )
  const myMemberSites = mySites?.filter((s) => approvedSiteIds.has(s.id)) ?? []
  const myNonMemberSites = mySites?.filter((s) => !approvedSiteIds.has(s.id)) ?? []

  const publicJoinUrl = ring.isPublicJoinEnabled
    ? `${window.location.origin}/rings/${ring.slug}/join`
    : null

  return (
    <div className="space-y-6">
      <div>
        <div className="flex items-start justify-between gap-4">
          <div>
            <h1 className="text-2xl font-bold text-gray-900">{ring.name}</h1>
            <p className="text-sm text-gray-500 mt-0.5">{ring.description}</p>
          </div>
          <Badge variant={ring.visibility === 'Public' ? 'green' : ring.visibility === 'Unlisted' ? 'blue' : 'gray'}>
            {ring.visibility}
          </Badge>
        </div>
        <div className="flex flex-wrap gap-4 mt-2 text-sm text-gray-500">
          <span>{ring.memberships.filter((m) => m.status === 'Approved').length} members</span>
          <span>Created {new Date(ring.createdAt).toLocaleDateString()}</span>
          {publicJoinUrl && (
            <a href={publicJoinUrl} target="_blank" rel="noopener noreferrer" className="text-violet-600 hover:underline">
              Public join page
            </a>
          )}
        </div>
      </div>

      {/* Owner tab bar */}
      {isOwner && (
        <div className="flex gap-1 border-b border-gray-200">
          {(['overview', 'settings'] as Tab[]).map((t) => (
            <button
              key={t}
              onClick={() => setTab(t)}
              className={`px-4 py-2 text-sm font-medium capitalize border-b-2 -mb-px transition-colors ${
                tab === t
                  ? 'border-violet-600 text-violet-700'
                  : 'border-transparent text-gray-500 hover:text-gray-700'
              }`}
            >
              {t}
            </button>
          ))}
        </div>
      )}

      {/* Settings tab */}
      {isOwner && tab === 'settings' && (
        <>
          <RingSettingsPanel ring={ring} />
          <Card>
            <h2 className="font-semibold text-gray-900 mb-1">Danger zone</h2>
            <p className="text-sm text-gray-500 mb-4">Permanently delete this ring and all its members and edges. This cannot be undone.</p>
            <Button
              variant="danger"
              loading={deleteRing.isPending}
              onClick={() => {
                if (confirm(`Delete "${ring.name}"? This cannot be undone.`)) {
                  deleteRing.mutate(ring.id, { onSuccess: () => navigate('/rings') })
                }
              }}
            >
              Delete ring
            </Button>
          </Card>
        </>
      )}

      {/* Overview tab (default) */}
      {(!isOwner || tab === 'overview') && (
        <>
          {/* Join ring (authenticated, not owner) */}
          {user && myNonMemberSites.length > 0 && !isOwner && (
            <Card>
              <h2 className="font-semibold text-gray-900 mb-3">Join this ring</h2>
              <div className="flex gap-3">
                <select
                  className="flex-1 rounded-lg border border-gray-300 px-3 py-2 text-sm text-gray-900 focus:border-violet-500 focus:outline-none"
                  value={joinSiteId}
                  onChange={(e) => setJoinSiteId(e.target.value)}
                >
                  <option value="">Select a site to join with…</option>
                  {myNonMemberSites.map((s) => (
                    <option key={s.id} value={s.id}>{s.name}</option>
                  ))}
                </select>
                <Button
                  disabled={!joinSiteId}
                  loading={joinRing.isPending}
                  onClick={() => joinRing.mutate({ ringId: ring.id, siteId: joinSiteId })}
                >
                  Request to join
                </Button>
              </div>
            </Card>
          )}

          {/* Owner: member management */}
          {isOwner && (
            <Card>
              <h2 className="font-semibold text-gray-900 mb-4">Member management</h2>
              <MemberApprovalList ringId={ring.id} memberships={ring.memberships} />
            </Card>
          )}

          {/* Navigator widget */}
          {myMemberSites.length > 0 && (
            <Card>
              <h2 className="font-semibold text-gray-900 mb-4">Navigation widget</h2>
              <NavigatorWidget ringId={ring.id} memberSites={myMemberSites} />
            </Card>
          )}

        </>
      )}
    </div>
  )
}
