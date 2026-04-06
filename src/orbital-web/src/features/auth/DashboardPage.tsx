import { Link } from 'react-router-dom'
import { Card } from '../../components/Card'
import { useMyRings } from '../../hooks/useRings'
import { useMySites } from '../../hooks/useSites'
import { getUser } from '../../lib/auth'

export function DashboardPage() {
  const user = getUser()
  const { data: sites } = useMySites()
  const { data: rings } = useMyRings()

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-gray-900">Welcome, {user?.username}</h1>
        <p className="text-sm text-gray-500">Manage your sites and rings from here.</p>
      </div>

      <div className="grid grid-cols-1 gap-4 sm:grid-cols-3">
        <StatCard
          label="Sites"
          value={sites?.length ?? 0}
          detail={`${sites?.filter((s) => s.verificationStatus === 'Verified').length ?? 0} verified`}
          href="/sites"
        />
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

      <div className="grid grid-cols-1 gap-6 sm:grid-cols-2">
        <Card>
          <div className="flex items-center justify-between mb-3">
            <h2 className="font-semibold text-gray-900">Recent sites</h2>
            <Link to="/sites" className="text-sm text-violet-600 hover:underline">View all</Link>
          </div>
          {!sites?.length && <p className="text-sm text-gray-500">No sites yet. <Link to="/sites" className="text-violet-600 hover:underline">Add one →</Link></p>}
          <ul className="space-y-2">
            {sites?.slice(0, 3).map((s) => (
              <li key={s.id} className="flex items-center justify-between">
                <span className="text-sm text-gray-700 truncate">{s.name}</span>
                <span className={`text-xs ${s.verificationStatus === 'Verified' ? 'text-emerald-600' : 'text-amber-600'}`}>
                  {s.verificationStatus}
                </span>
              </li>
            ))}
          </ul>
        </Card>

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
