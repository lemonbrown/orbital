import { Badge } from '../../components/Badge'
import { Button } from '../../components/Button'
import { useApproveMember, useRejectMember, useRemoveMember, type Membership } from '../../hooks/useRings'

interface Props {
  ringId: string
  memberships: Membership[]
}

export function MemberApprovalList({ ringId, memberships }: Props) {
  const approve = useApproveMember()
  const reject = useRejectMember()
  const remove = useRemoveMember()

  const pendingVerification = memberships.filter((m) => m.status === 'PendingVerification')
  const pending = memberships.filter((m) => m.status === 'PendingApproval')
  const approved = memberships.filter((m) => m.status === 'Approved')

  const displayName = (m: Membership) =>
    m.applicantName ?? m.siteName

  return (
    <div className="space-y-6">
      {pendingVerification.length > 0 && (
        <section>
          <h3 className="text-sm font-semibold text-gray-900 mb-2">
            Awaiting snippet verification ({pendingVerification.length})
          </h3>
          <div className="space-y-2">
            {pendingVerification.map((m) => (
              <div key={m.id} className="flex items-center justify-between rounded-lg border border-blue-200 bg-blue-50 px-4 py-3">
                <div>
                  <p className="text-sm font-medium text-gray-900">{displayName(m)}</p>
                  {m.contactEmail && (
                    <p className="text-xs text-gray-500">{m.contactEmail}</p>
                  )}
                  <p className="text-xs text-gray-500">Submitted {new Date(m.joinedAt).toLocaleDateString()}</p>
                </div>
                <div className="flex gap-2">
                  <Button
                    size="sm"
                    loading={approve.isPending && approve.variables?.membershipId === m.id}
                    onClick={() => approve.mutate({ ringId, membershipId: m.id })}
                  >
                    Approve anyway
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

      {pending.length > 0 && (
        <section>
          <h3 className="text-sm font-semibold text-gray-900 mb-2">
            Pending approval ({pending.length})
          </h3>
          <div className="space-y-2">
            {pending.map((m) => (
              <div key={m.id} className="flex items-center justify-between rounded-lg border border-amber-200 bg-amber-50 px-4 py-3">
                <div>
                  <p className="text-sm font-medium text-gray-900">{displayName(m)}</p>
                  {m.contactEmail && (
                    <p className="text-xs text-gray-500">{m.contactEmail}</p>
                  )}
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
                  <div>
                    <p className="text-sm font-medium text-gray-900">{displayName(m)}</p>
                    <p className="text-xs text-gray-400">{m.siteUrl}</p>
                  </div>
                </div>
                <div className="flex items-center gap-2">
                  <Badge variant={m.role === 'Owner' ? 'purple' : 'gray'}>{m.role}</Badge>
                  {m.role !== 'Owner' && (
                    <Button
                      size="sm"
                      variant="danger"
                      loading={remove.isPending && remove.variables?.membershipId === m.id}
                      onClick={() => remove.mutate({ ringId, membershipId: m.id })}
                    >
                      Remove
                    </Button>
                  )}
                </div>
              </div>
            ))}
        </div>
      </section>
    </div>
  )
}
