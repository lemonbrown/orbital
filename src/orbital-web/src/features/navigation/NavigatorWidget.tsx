import { useState } from 'react'
import { Button } from '../../components/Button'
import type { Site } from '../../hooks/useSites'

interface Props {
  ringId: string
  memberSites: Site[]
}

export function NavigatorWidget({ ringId, memberSites }: Props) {
  const [selectedSiteId, setSelectedSiteId] = useState(memberSites[0]?.id ?? '')
  const [dimension, setDimension] = useState('default')
  const [copied, setCopied] = useState(false)

  const baseUrl = window.location.origin

  const navUrl = (direction: 'next' | 'prev') =>
    `${baseUrl}/api/navigate?ring=${ringId}&site=${selectedSiteId}&dimension=${dimension}&direction=${direction}`

  const snippet = `<!-- Orbital Web Ring Widget -->
<nav class="orbital-ring" style="display:flex;gap:8px;align-items:center;font-family:sans-serif;">
  <a href="${navUrl('prev')}" title="Previous site in ring">← Prev</a>
  <a href="https://orbital.app/rings/${ringId}" title="Orbital Ring">⬡ Orbital</a>
  <a href="${navUrl('next')}" title="Next site in ring">Next →</a>
</nav>`

  const copySnippet = () => {
    navigator.clipboard.writeText(snippet).then(() => {
      setCopied(true)
      setTimeout(() => setCopied(false), 2000)
    })
  }

  return (
    <div className="space-y-4">
      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
        <div className="flex flex-col gap-1">
          <label className="text-xs font-medium text-gray-500">Your site in this ring</label>
          <select
            className="rounded-lg border border-gray-300 px-3 py-2 text-sm text-gray-900 focus:border-violet-500 focus:outline-none"
            value={selectedSiteId}
            onChange={(e) => setSelectedSiteId(e.target.value)}
          >
            {memberSites.map((s) => (
              <option key={s.id} value={s.id}>{s.name}</option>
            ))}
          </select>
        </div>
        <div className="flex flex-col gap-1">
          <label className="text-xs font-medium text-gray-500">Dimension</label>
          <input
            className="rounded-lg border border-gray-300 px-3 py-2 text-sm text-gray-900 focus:border-violet-500 focus:outline-none"
            value={dimension}
            onChange={(e) => setDimension(e.target.value)}
            placeholder="default"
          />
        </div>
      </div>

      {/* Live preview */}
      <div className="rounded-lg border border-gray-200 bg-gray-50 p-4">
        <p className="text-xs text-gray-500 mb-3">Preview:</p>
        <nav className="flex gap-3 items-center">
          <a
            href={navUrl('prev')}
            target="_blank"
            rel="noopener noreferrer"
            className="rounded-lg border border-gray-300 bg-white px-3 py-1.5 text-sm font-medium text-gray-700 hover:bg-gray-50"
          >
            ← Prev
          </a>
          <span className="text-violet-600 font-bold">⬡ Orbital</span>
          <a
            href={navUrl('next')}
            target="_blank"
            rel="noopener noreferrer"
            className="rounded-lg border border-gray-300 bg-white px-3 py-1.5 text-sm font-medium text-gray-700 hover:bg-gray-50"
          >
            Next →
          </a>
        </nav>
      </div>

      {/* Embed snippet */}
      <div>
        <div className="flex items-center justify-between mb-2">
          <p className="text-xs font-medium text-gray-500">Embed snippet</p>
          <Button size="sm" variant="secondary" onClick={copySnippet}>
            {copied ? '✓ Copied' : 'Copy'}
          </Button>
        </div>
        <pre className="bg-gray-900 text-gray-100 rounded-lg p-4 text-xs overflow-x-auto leading-relaxed">
          {snippet}
        </pre>
      </div>
    </div>
  )
}
