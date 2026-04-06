import { useState } from 'react'
import { Badge } from '../../components/Badge'
import { Button } from '../../components/Button'
import { Input } from '../../components/Input'
import {
  useApiKeys,
  useCreateApiKey,
  useRevokeApiKey,
  type ApiKey,
} from '../../hooks/useRings'

interface Props {
  ringId: string
}

export function ApiKeyManager({ ringId }: Props) {
  const { data: keys, isLoading } = useApiKeys(ringId)
  const create = useCreateApiKey()
  const revoke = useRevokeApiKey()
  const [label, setLabel] = useState('')
  const [newKey, setNewKey] = useState<string | null>(null)

  async function handleCreate(e: React.FormEvent) {
    e.preventDefault()
    const result = await create.mutateAsync({ ringId, label })
    setNewKey(result.plainKey)
    setLabel('')
  }

  if (isLoading) return <p className="text-sm text-gray-500">Loading keys…</p>

  return (
    <div className="space-y-4">
      {newKey && (
        <div className="rounded-lg border border-green-200 bg-green-50 p-4">
          <p className="text-sm font-medium text-green-800 mb-2">
            API key created. Copy it now — it won't be shown again.
          </p>
          <pre className="text-xs bg-white border border-green-200 rounded p-2 overflow-x-auto break-all">
            {newKey}
          </pre>
          <Button variant="ghost" size="sm" className="mt-2" onClick={() => setNewKey(null)}>
            Dismiss
          </Button>
        </div>
      )}

      <form onSubmit={handleCreate} className="flex gap-2">
        <Input
          placeholder="Key label (e.g. my-site-form)"
          value={label}
          onChange={(e) => setLabel(e.target.value)}
          required
        />
        <Button type="submit" loading={create.isPending}>
          Generate
        </Button>
      </form>

      {keys && keys.length > 0 ? (
        <div className="space-y-2">
          {keys.map((key: ApiKey) => (
            <div
              key={key.id}
              className={`flex items-center justify-between rounded-lg border px-4 py-3 ${
                key.isRevoked ? 'border-gray-100 bg-gray-50 opacity-60' : 'border-gray-200 bg-white'
              }`}
            >
              <div>
                <div className="flex items-center gap-2">
                  <p className="text-sm font-medium text-gray-900">{key.label}</p>
                  {key.isRevoked && <Badge variant="red">Revoked</Badge>}
                </div>
                <p className="text-xs text-gray-400 font-mono mt-0.5">{key.keyPrefix}…</p>
                <p className="text-xs text-gray-400">
                  Created {new Date(key.createdAt).toLocaleDateString()}
                  {key.revokedAt && ` · Revoked ${new Date(key.revokedAt).toLocaleDateString()}`}
                </p>
              </div>
              {!key.isRevoked && (
                <Button
                  variant="danger"
                  size="sm"
                  loading={revoke.isPending && revoke.variables?.keyId === key.id}
                  onClick={() => revoke.mutate({ ringId, keyId: key.id })}
                >
                  Revoke
                </Button>
              )}
            </div>
          ))}
        </div>
      ) : (
        <p className="text-sm text-gray-500">No API keys yet.</p>
      )}

      <div className="rounded-lg border border-gray-200 bg-gray-50 p-3 text-xs text-gray-500">
        <p className="font-medium text-gray-700 mb-1">Using the API</p>
        <p>Send submissions to <code className="bg-white border rounded px-1">POST /api/applications</code> with header:</p>
        <pre className="mt-1 bg-white border rounded p-2 overflow-x-auto">X-Ring-Api-Key: sk_…</pre>
        <p className="mt-1">Ring ID is inferred from the key. The body is the same as the public join form.</p>
      </div>
    </div>
  )
}
