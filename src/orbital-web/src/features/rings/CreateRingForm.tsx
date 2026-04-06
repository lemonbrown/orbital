import { useState } from 'react'
import { Button } from '../../components/Button'
import { Input } from '../../components/Input'
import { useCreateRing } from '../../hooks/useRings'
import { useMySites } from '../../hooks/useSites'
import { useNavigate } from 'react-router-dom'

interface Props {
  onClose: () => void
}

export function CreateRingForm({ onClose }: Props) {
  const [form, setForm] = useState({
    name: '',
    description: '',
    ownerSiteId: '',
    visibility: 'Public' as 'Public' | 'Unlisted' | 'Private',
  })
  const { data: sites } = useMySites()
  const createRing = useCreateRing()
  const navigate = useNavigate()

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    createRing.mutate(form, {
      onSuccess: (data) => navigate(`/rings/${data.id}`),
    })
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <h2 className="font-semibold text-gray-900">Create a ring</h2>
      <Input
        id="ring-name"
        label="Ring name"
        value={form.name}
        onChange={(e) => setForm((f) => ({ ...f, name: e.target.value }))}
        placeholder="Indie Web Devs"
        required
      />
      <Input
        id="ring-description"
        label="Description (optional)"
        value={form.description}
        onChange={(e) => setForm((f) => ({ ...f, description: e.target.value }))}
        placeholder="A ring for independent web developers"
      />
      <div className="flex flex-col gap-1">
        <label htmlFor="owner-site" className="text-sm font-medium text-gray-700">
          Your site (ring anchor)
        </label>
        <select
          id="owner-site"
          className="rounded-lg border border-gray-300 px-3 py-2 text-sm text-gray-900 focus:border-violet-500 focus:outline-none"
          value={form.ownerSiteId}
          onChange={(e) => setForm((f) => ({ ...f, ownerSiteId: e.target.value }))}
          required
        >
          <option value="">Select a site…</option>
          {sites?.map((s) => (
            <option key={s.id} value={s.id}>{s.name} — {s.url}</option>
          ))}
        </select>
        {!sites?.length && (
          <p className="text-xs text-amber-600">You need to add a site first before creating a ring.</p>
        )}
      </div>
      <div className="flex flex-col gap-1">
        <label className="text-sm font-medium text-gray-700">Visibility</label>
        <div className="flex gap-4">
          {(['Public', 'Unlisted', 'Private'] as const).map((v) => (
            <label key={v} className="flex items-center gap-1.5 text-sm text-gray-700 cursor-pointer">
              <input
                type="radio"
                value={v}
                checked={form.visibility === v}
                onChange={() => setForm((f) => ({ ...f, visibility: v }))}
                className="accent-violet-600"
              />
              {v}
            </label>
          ))}
        </div>
      </div>
      {createRing.error && (
        <p className="text-sm text-red-600">
          {(createRing.error as any)?.response?.data?.detail ?? 'Failed to create ring.'}
        </p>
      )}
      <div className="flex gap-2 justify-end">
        <Button variant="ghost" type="button" onClick={onClose}>Cancel</Button>
        <Button type="submit" loading={createRing.isPending} disabled={!form.ownerSiteId}>
          Create ring
        </Button>
      </div>
    </form>
  )
}
