import { useState } from 'react'
import { useParams } from 'react-router-dom'
import { Badge } from '../../components/Badge'
import { Button } from '../../components/Button'
import { Card } from '../../components/Card'
import { useJoinRing, useRing } from '../../hooks/useRings'
import { useMySites } from '../../hooks/useSites'
import { getUser } from '../../lib/auth'
import { MemberApprovalList } from './MemberApprovalList'
import { NavigatorWidget } from '../navigation/NavigatorWidget'

export function RingDetailPage() {
  const { id } = useParams<{ id: string }>()
  const { data: ring, isLoading } = useRing(id!)
  const { data: mySites } = useMySites()
  const joinRing = useJoinRing()
  const [joinSiteId, setJoinSiteId] = useState('')
  const user = getUser()

  if (isLoading) return <p className="text-gray-500">Loading…</p>
  if (!ring) return <p className="text-red-600">Ring not found.</p>

  const isOwner = ring.ownerUserId === user?.userId
  const approvedSiteIds = new Set(
    ring.memberships.filter((m) => m.status === 'Approved').map((m) => m.siteId)
  )
  const myMemberSites = mySites?.filter((s) => approvedSiteIds.has(s.id)) ?? []
  const myNonMemberSites = mySites?.filter((s) => !approvedSiteIds.has(s.id)) ?? []

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
        <div className="flex gap-4 mt-2 text-sm text-gray-500">
          <span>{ring.memberships.filter((m) => m.status === 'Approved').length} members</span>
          <span>{ring.edges.length} edges</span>
          <span>Created {new Date(ring.createdAt).toLocaleDateString()}</span>
        </div>
      </div>

      {/* Join ring */}
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

      {/* Edges overview */}
      {ring.edges.length > 0 && (
        <Card>
          <h2 className="font-semibold text-gray-900 mb-4">Edge graph</h2>
          <div className="space-y-2">
            {Array.from(new Set(ring.edges.map((e) => e.dimension))).map((dim) => (
              <div key={dim}>
                <p className="text-xs font-medium text-gray-500 mb-1">Dimension: {dim}</p>
                <div className="flex flex-wrap gap-1">
                  {ring.edges
                    .filter((e) => e.dimension === dim && e.label === 'next')
                    .map((e) => (
                      <span key={e.id} className="text-xs bg-gray-100 rounded px-2 py-0.5 text-gray-600">
                        {e.fromSiteId.slice(0, 6)} → {e.toSiteId.slice(0, 6)}
                      </span>
                    ))}
                </div>
              </div>
            ))}
          </div>
        </Card>
      )}
    </div>
  )
}
