import { useState } from 'react'
import { Link } from 'react-router-dom'
import { Badge } from '../../components/Badge'
import { Button } from '../../components/Button'
import { Card } from '../../components/Card'
import { useMyRings, usePublicRings } from '../../hooks/useRings'
import { isAuthenticated } from '../../lib/auth'
import { CreateRingForm } from './CreateRingForm'

export function RingListPage() {
  const { data: publicRings, isLoading: publicLoading } = usePublicRings()
  const { data: myRings } = useMyRings()
  const [showForm, setShowForm] = useState(false)
  const authed = isAuthenticated()

  return (
    <div className="space-y-8">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Web Rings</h1>
          <p className="text-sm text-gray-500">Discover and join multi-dimensional web rings.</p>
        </div>
        {authed && <Button onClick={() => setShowForm(true)}>+ Create ring</Button>}
      </div>

      {showForm && (
        <Card>
          <CreateRingForm onClose={() => setShowForm(false)} />
        </Card>
      )}

      {authed && myRings && myRings.length > 0 && (
        <section>
          <h2 className="text-lg font-semibold text-gray-900 mb-3">My Rings</h2>
          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
            {myRings.map((ring) => (
              <RingCard key={ring.id} ring={ring} />
            ))}
          </div>
        </section>
      )}

      <section>
        <h2 className="text-lg font-semibold text-gray-900 mb-3">Public Rings</h2>
        {publicLoading && <p className="text-gray-500">Loading…</p>}
        {publicRings?.length === 0 && (
          <Card className="text-center py-12">
            <p className="text-gray-500">No public rings yet.</p>
          </Card>
        )}
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
          {publicRings?.map((ring) => (
            <RingCard key={ring.id} ring={ring} />
          ))}
        </div>
      </section>
    </div>
  )
}

function RingCard({ ring }: { ring: { id: string; name: string; description: string; memberCount: number; visibility: string } }) {
  return (
    <Card className="hover:border-violet-300 transition-colors">
      <div className="flex items-start justify-between gap-2">
        <div className="flex-1 min-w-0">
          <Link to={`/rings/${ring.id}`} className="font-semibold text-gray-900 hover:text-violet-600">
            {ring.name}
          </Link>
          {ring.description && (
            <p className="text-sm text-gray-500 mt-0.5 line-clamp-2">{ring.description}</p>
          )}
        </div>
        <Badge variant="purple">{ring.memberCount} sites</Badge>
      </div>
    </Card>
  )
}
