import { Link } from 'react-router-dom'
import { Button } from '../../components/Button'
import { isAuthenticated } from '../../lib/auth'

export function HomePage() {
  const authed = isAuthenticated()

  return (
    <div className="flex flex-col items-center justify-center text-center min-h-[80vh] gap-8">
      <div>
        <p className="text-violet-600 font-semibold text-sm uppercase tracking-widest mb-4">
          Multi-dimensional web rings
        </p>
        <h1 className="text-5xl font-bold text-gray-900 leading-tight">
          The web is a graph,<br />not a list.
        </h1>
        <p className="mt-4 text-lg text-gray-500 max-w-2xl mx-auto">
          Orbital lets you build and traverse web rings across multiple dimensions —
          topic, mood, depth, and more. Simple links, infinite paths.
        </p>
      </div>

      <div className="flex gap-4">
        {authed ? (
          <Link to="/dashboard">
            <Button size="lg">Go to dashboard →</Button>
          </Link>
        ) : (
          <>
            <Link to="/register">
              <Button size="lg">Get started free</Button>
            </Link>
            <Link to="/rings">
              <Button size="lg" variant="secondary">Browse rings</Button>
            </Link>
          </>
        )}
      </div>

      <div className="grid grid-cols-1 gap-6 sm:grid-cols-3 max-w-3xl w-full mt-4">
        {[
          { icon: '⬡', title: 'Classic ring mode', body: 'Auto-generates next/prev links from member order. Drop-in replacement for any web ring.' },
          { icon: '⎇', title: 'Multiple dimensions', body: 'Same nodes, different paths. Navigate by topic, mood, or any custom dimension you define.' },
          { icon: '✦', title: 'Open graph', body: 'Edges are first-class data. Build exploration paths, recommendations, and discovery flows.' },
        ].map((f) => (
          <div key={f.title} className="rounded-xl border border-gray-200 bg-white p-6 text-left">
            <span className="text-2xl">{f.icon}</span>
            <h3 className="font-semibold text-gray-900 mt-2 mb-1">{f.title}</h3>
            <p className="text-sm text-gray-500">{f.body}</p>
          </div>
        ))}
      </div>
    </div>
  )
}
