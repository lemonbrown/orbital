import { useState } from 'react'
import { useParams } from 'react-router-dom'
import { Badge } from '../../components/Badge'
import { Button } from '../../components/Button'
import { Card } from '../../components/Card'
import { Input } from '../../components/Input'
import { useSubmitApplication, type SubmitApplicationResult } from '../../hooks/useApplications'
import { useCheckSnippet } from '../../hooks/useRings'
import { useRingBySlug } from '../../hooks/useRings'
import { Link } from 'react-router-dom'

export function PublicJoinPage() {
  const { slug } = useParams<{ slug: string }>()
  const { data: ring, isLoading } = useRingBySlug(slug!)

  const submit = useSubmitApplication()
  const checkSnippet = useCheckSnippet()

  const [form, setForm] = useState({
    siteUrl: '',
    siteName: '',
    description: '',
    contactEmail: '',
    applicantName: '',
  })
  const [result, setResult] = useState<SubmitApplicationResult | null>(null)
  const [snippetChecked, setSnippetChecked] = useState(false)
  const [snippetMessage, setSnippetMessage] = useState('')

  if (isLoading) return <p className="text-gray-500 text-center mt-16">Loading…</p>
  if (!ring) return <p className="text-red-600 text-center mt-16">Ring not found.</p>

  if (!ring.isPublicJoinEnabled) {
    return (
      <div className="max-w-lg mx-auto mt-16 text-center">
        <Card>
          <h1 className="text-xl font-bold text-gray-900 mb-2">{ring.name}</h1>
          <p className="text-gray-500">This ring is not accepting public applications.</p>
        </Card>
      </div>
    )
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    const data = await submit.mutateAsync({
      ringId: ring!.id,
      siteUrl: form.siteUrl,
      siteName: form.siteName,
      description: form.description || undefined,
      contactEmail: form.contactEmail || undefined,
      applicantName: form.applicantName || undefined,
    })
    setResult(data)
  }

  async function handleCheckSnippet() {
    if (!result) return
    const data = await checkSnippet.mutateAsync({
      ringId: ring!.id,
      membershipId: result.membershipId,
    })
    setSnippetChecked(data.found)
    setSnippetMessage(data.message)
  }

  if (result) {
    return (
      <div className="max-w-lg mx-auto mt-10 space-y-6">
        <Card>
          <div className="flex items-center gap-2 mb-4">
            <h1 className="text-xl font-bold text-gray-900">Application submitted</h1>
            <Badge variant={result.status === 'Approved' || snippetChecked ? 'green' : 'yellow'}>
              {snippetChecked ? 'Approved' : result.status}
            </Badge>
          </div>

          {result.status === 'PendingApproval' && (
            <p className="text-sm text-gray-600">
              Your application is pending review by the ring owner. You'll be contacted at{' '}
              {form.contactEmail || 'the email you provided'} if approved.
            </p>
          )}

          {result.status === 'PendingVerification' && !snippetChecked && (
            <div className="space-y-4">
              <p className="text-sm text-gray-600">
                {ring.verificationMode === 'Widget' 
                  ? "Add this snippet to your site. The ring owner will visually verify it before approving your application."
                  : "Add this snippet to your site, then click Verify to confirm."}
              </p>
              
              {ring.verificationMode === 'Auto' && (
                <>
                  {snippetMessage && (
                    <p className={`text-sm ${snippetChecked ? 'text-green-600' : 'text-amber-600'}`}>
                      {snippetMessage}
                    </p>
                  )}
                  <Button
                    loading={checkSnippet.isPending}
                    onClick={handleCheckSnippet}
                  >
                    Verify snippet
                  </Button>
                </>
              )}
            </div>
          )}
          {(result.status === 'Approved' || snippetChecked) && (
            <p className="text-sm text-green-700 font-medium">
              Your site is now a member of {ring.name}.
            </p>
          )}

          {result.snippetHtml && (
            <div className={`space-y-4 pt-4 border-t border-gray-100`}>
              <p className="text-sm font-semibold text-gray-900">Your Web Ring Widget</p>
              {(result.status === 'Approved' || result.status === 'PendingApproval') && (
                <p className="text-sm text-gray-600">
                  Embed this HTML anywhere on your site to display your web ring connectivity.
                </p>
              )}
              <pre className="bg-gray-50 border border-gray-200 rounded-lg p-3 text-xs overflow-x-auto whitespace-pre-wrap break-all">
                {result.snippetHtml}
              </pre>
            </div>
          )}
        </Card>
      </div>
    )
  }

  return (
    <div className="max-w-lg mx-auto mt-10 space-y-6">
      <div className="text-center">
        <h1 className="text-2xl font-bold text-gray-900">{ring.name}</h1>
        {ring.description && (
          <p className="text-gray-500 mt-1">{ring.description}</p>
        )}
        <div className="flex justify-center gap-3 mt-2 text-sm text-gray-500">
          <span>{ring.memberships.filter((m) => m.status === 'Approved').length} members</span>
          <Badge variant={ring.verificationMode === 'Auto' ? 'blue' : 'gray'}>
            {ring.verificationMode === 'Auto' ? 'Auto-verified' : ring.verificationMode === 'Widget' ? 'Widget required' : 'Trust-based'}
          </Badge>
          <Badge variant={ring.approvalMode === 'Auto' ? 'green' : 'gray'}>
            {ring.approvalMode === 'Auto' ? 'Auto-approved' : 'Manual review'}
          </Badge>
        </div>
        {ring.isPublicDirectoryEnabled && (
          <div className="mt-4">
            <Link to={`/rings/${ring.slug}/directory`} className="text-sm font-medium text-violet-600 hover:text-violet-500">
              View member directory &rarr;
            </Link>
          </div>
        )}
      </div>

      <Card>
        <h2 className="font-semibold text-gray-900 mb-4">Apply to join</h2>
        <form onSubmit={handleSubmit} className="space-y-4">
          <Input
            label="Your name (optional)"
            value={form.applicantName}
            onChange={(e) => setForm({ ...form, applicantName: e.target.value })}
            placeholder="Jane Smith"
          />
          <Input
            label="Site URL"
            required
            type="url"
            value={form.siteUrl}
            onChange={(e) => setForm({ ...form, siteUrl: e.target.value })}
            placeholder="https://example.com"
          />
          <Input
            label="Site name"
            required
            value={form.siteName}
            onChange={(e) => setForm({ ...form, siteName: e.target.value })}
            placeholder="My Blog"
          />
          <Input
            label="Description (optional)"
            value={form.description}
            onChange={(e) => setForm({ ...form, description: e.target.value })}
            placeholder="What is your site about?"
          />
          <Input
            label="Contact email"
            type="email"
            value={form.contactEmail}
            onChange={(e) => setForm({ ...form, contactEmail: e.target.value })}
            placeholder="you@example.com"
          />
          {submit.error && (
            <p className="text-sm text-red-600">
              {(submit.error as Error).message}
            </p>
          )}
          <Button type="submit" loading={submit.isPending} className="w-full">
            Apply to join
          </Button>
        </form>
      </Card>
    </div>
  )
}
