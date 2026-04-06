import { useState } from 'react'
import { Badge } from '../../components/Badge'
import { Button } from '../../components/Button'
import { Card } from '../../components/Card'
import { useMySites, useVerifySite, type Site } from '../../hooks/useSites'
import { AddSiteForm } from './AddSiteForm'

function verificationBadge(status: Site['verificationStatus']) {
  if (status === 'Verified') return <Badge variant="green">Verified</Badge>
  if (status === 'Failed') return <Badge variant="red">Failed</Badge>
  return <Badge variant="yellow">Pending</Badge>
}

export function SiteListPage() {
  const { data: sites, isLoading } = useMySites()
  const verify = useVerifySite()
  const [showForm, setShowForm] = useState(false)
  const [expanded, setExpanded] = useState<string | null>(null)

  if (isLoading) return <p className="text-gray-500">Loading sites…</p>

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">My Sites</h1>
          <p className="text-sm text-gray-500">Manage the sites you own in Orbital.</p>
        </div>
        <Button onClick={() => setShowForm(true)}>+ Add site</Button>
      </div>

      {showForm && (
        <Card>
          <AddSiteForm onClose={() => setShowForm(false)} />
        </Card>
      )}

      {sites?.length === 0 && (
        <Card className="text-center py-12">
          <p className="text-gray-500">No sites yet. Add your first site above.</p>
        </Card>
      )}

      <div className="space-y-4">
        {sites?.map((site) => (
          <Card key={site.id}>
            <div className="flex items-start justify-between gap-4">
              <div className="flex-1 min-w-0">
                <div className="flex items-center gap-2">
                  <h2 className="font-semibold text-gray-900 truncate">{site.name}</h2>
                  {verificationBadge(site.verificationStatus)}
                </div>
                <a
                  href={site.url}
                  target="_blank"
                  rel="noopener noreferrer"
                  className="text-sm text-violet-600 hover:underline truncate block"
                >
                  {site.url}
                </a>
                {site.description && (
                  <p className="text-sm text-gray-500 mt-1">{site.description}</p>
                )}
              </div>
              <div className="flex gap-2 shrink-0">
                {site.verificationStatus !== 'Verified' && (
                  <Button
                    variant="secondary"
                    size="sm"
                    loading={verify.isPending && verify.variables === site.id}
                    onClick={() => verify.mutate(site.id)}
                  >
                    Verify
                  </Button>
                )}
                <Button
                  variant="ghost"
                  size="sm"
                  onClick={() => setExpanded(expanded === site.id ? null : site.id)}
                >
                  {expanded === site.id ? 'Hide' : 'Details'}
                </Button>
              </div>
            </div>

            {expanded === site.id && (
              <div className="mt-4 pt-4 border-t border-gray-100">
                <p className="text-xs font-medium text-gray-500 mb-2">Verification instructions</p>
                <p className="text-sm text-gray-600 mb-2">
                  Add this meta tag to your site's <code className="bg-gray-100 px-1 rounded">&lt;head&gt;</code>:
                </p>
                <pre className="bg-gray-50 border border-gray-200 rounded-lg p-3 text-xs overflow-x-auto">
                  {`<meta name="orbital-verify" content="${site.verificationToken}">`}
                </pre>
              </div>
            )}
          </Card>
        ))}
      </div>
    </div>
  )
}
