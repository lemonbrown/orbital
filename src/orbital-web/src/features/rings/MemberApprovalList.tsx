import { Badge } from '../../components/Badge'
import { Button } from '../../components/Button'
import { useApproveMember, useRejectMember, type Membership } from '../../hooks/useRings'
import { useMySites } from '../../hooks/useSites'

interface Props {
  ringId: string
  memberships: Membership[]
}

export function MemberApprovalList({ ringId, memberships }: Props) {
  const approve = useApproveMember()
  const reject = useRejectMember()
  const { data: mySites } = useMySites()

  const pending = memberships.filter((m) => m.status === 'Pending')
  const approved = memberships.filter((m) => m.status === 'Approved')

  const getSiteName = (siteId: string) => {
    return mySites?.find((s) => s.id === siteId)?.name ?? siteId.slice(0, 8) + '…'
  }

  return (
    <div className="space-y-6">
      {pending.length > 0 && (
        <section>
          <h3 className="text-sm font-semibold text-gray-900 mb-2">
            Pending approval ({pending.length})
          </h3>
          <div className="space-y-2">
            {pending.map((m) => (
              <div key={m.id} className="flex items-center justify-between rounded-lg border border-amber-200 bg-amber-50 px-4 py-3">
                <div>
                  <p className="text-sm font-medium text-gray-900">{getSiteName(m.siteId)}</p>
                  <p className="text-xs text-gray-500">Requested {new Date(m.joinedAt).toLocaleDateString()}</p>
                </div>
                <div className="flex gap-2">
                  <Button
                    size="sm"
                    loading={approve.isPending && approve.variables?.membershipId === m.id}
                    onClick={() => approve.mutate({ ringId, membershipId: m.id })}
                  >
                    Approve
                  </Button>
                  <Button
                    size="sm"
                    variant="danger"
                    loading={reject.isPending && reject.variables?.membershipId === m.id}
                    onClick={() => reject.mutate({ ringId, membershipId: m.id })}
                  >
                    Reject
                  </Button>
                </div>
              </div>
            ))}
          </div>
        </section>
      )}

      <section>
        <h3 className="text-sm font-semibold text-gray-900 mb-2">
          Members ({approved.length})
        </h3>
        {approved.length === 0 && (
          <p className="text-sm text-gray-500">No approved members yet.</p>
        )}
        <div className="space-y-2">
          {approved
            .sort((a, b) => a.orderIndex - b.orderIndex)
            .map((m) => (
              <div key={m.id} className="flex items-center justify-between rounded-lg border border-gray-200 bg-white px-4 py-3">
                <div className="flex items-center gap-2">
                  <span className="text-xs text-gray-400 font-mono w-5">#{m.orderIndex}</span>
                  <p className="text-sm font-medium text-gray-900">{getSiteName(m.siteId)}</p>
                </div>
                <Badge variant={m.role === 'Owner' ? 'purple' : 'gray'}>{m.role}</Badge>
              </div>
            ))}
        </div>
      </section>
    </div>
  )
}
