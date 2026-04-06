import { useState } from 'react'
import { Button } from '../../components/Button'
import { Input } from '../../components/Input'
import { useAddSite } from '../../hooks/useSites'

interface Props {
  onClose: () => void
}

export function AddSiteForm({ onClose }: Props) {
  const [form, setForm] = useState({ name: '', url: '', description: '' })
  const addSite = useAddSite()

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    addSite.mutate(form, { onSuccess: onClose })
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <h2 className="font-semibold text-gray-900">Add a site</h2>
      <Input
        id="site-name"
        label="Site name"
        value={form.name}
        onChange={(e) => setForm((f) => ({ ...f, name: e.target.value }))}
        placeholder="My Blog"
        required
      />
      <Input
        id="site-url"
        label="URL"
        type="url"
        value={form.url}
        onChange={(e) => setForm((f) => ({ ...f, url: e.target.value }))}
        placeholder="https://myblog.example.com"
        required
      />
      <Input
        id="site-description"
        label="Description (optional)"
        value={form.description}
        onChange={(e) => setForm((f) => ({ ...f, description: e.target.value }))}
        placeholder="A short description of your site"
      />
      {addSite.error && (
        <p className="text-sm text-red-600">
          {(addSite.error as any)?.response?.data?.detail ?? 'Failed to add site.'}
        </p>
      )}
      <div className="flex gap-2 justify-end">
        <Button variant="ghost" type="button" onClick={onClose}>
          Cancel
        </Button>
        <Button type="submit" loading={addSite.isPending}>
          Add site
        </Button>
      </div>
    </form>
  )
}
