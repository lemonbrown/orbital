import { Link } from 'react-router-dom'
import { Card } from '../../components/Card'
import { useMyRings } from '../../hooks/useRings'
import { getUser } from '../../lib/auth'

export function DashboardPage() {
  const user = getUser()
  const { data: rings } = useMyRings()

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-gray-900">Welcome, {user?.username}</h1>
        <p className="text-sm text-gray-500">Manage your rings from here.</p>
      </div>

      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
        <StatCard
          label="Rings"
          value={rings?.length ?? 0}
          detail="owned by you"
          href="/rings"
        />
        <StatCard
          label="Dimensions"
          value="∞"
          detail="multi-dimensional traversal"
          href="/rings"
        />
      </div>

      <Card>
        <div className="flex items-center justify-between mb-3">
          <h2 className="font-semibold text-gray-900">My rings</h2>
          <Link to="/rings" className="text-sm text-violet-600 hover:underline">View all</Link>
        </div>
        {!rings?.length && <p className="text-sm text-gray-500">No rings yet. <Link to="/rings" className="text-violet-600 hover:underline">Create one →</Link></p>}
        <ul className="space-y-2">
          {rings?.slice(0, 3).map((r) => (
            <li key={r.id}>
              <Link to={`/rings/${r.id}`} className="text-sm text-violet-600 hover:underline">
                {r.name}
              </Link>
              <span className="text-xs text-gray-400 ml-2">{r.memberCount} members</span>
            </li>
          ))}
        </ul>
      </Card>
    </div>
  )
}

function StatCard({ label, value, detail, href }: { label: string; value: number | string; detail: string; href: string }) {
  return (
    <Link to={href}>
      <Card className="hover:border-violet-300 transition-colors cursor-pointer">
        <p className="text-3xl font-bold text-gray-900">{value}</p>
        <p className="text-sm font-medium text-gray-700 mt-1">{label}</p>
        <p className="text-xs text-gray-400 mt-0.5">{detail}</p>
      </Card>
    </Link>
  )
}
