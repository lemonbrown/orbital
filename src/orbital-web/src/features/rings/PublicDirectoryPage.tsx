import { useParams } from 'react-router-dom'
import { Card } from '../../components/Card'
import { useRingDirectory, useRingBySlug } from '../../hooks/useRings'

export function PublicDirectoryPage() {
  const { slug } = useParams<{ slug: string }>()
  const { data: ring, isLoading: isRingLoading } = useRingBySlug(slug!)
  const { data: directory, isLoading: isDirLoading, error } = useRingDirectory(slug!)

  if (isRingLoading || isDirLoading) {
    return <p className="text-gray-500 text-center mt-16">Loading directory…</p>
  }

  if (!ring) {
    return <p className="text-red-600 text-center mt-16">Ring not found.</p>
  }

  if (error) {
    return (
      <div className="max-w-3xl mx-auto mt-16 text-center">
        <h1 className="text-2xl font-bold text-gray-900">{ring.name}</h1>
        <p className="text-red-600 mt-4">
          This directory is not public or does not exist.
        </p>
      </div>
    )
  }

  return (
    <div className="max-w-4xl mx-auto mt-10 space-y-8 px-4">
      <div className="text-center space-y-2">
        <h1 className="text-3xl font-extrabold text-gray-900 tracking-tight">
          {ring.name} Directory
        </h1>
        <p className="text-gray-500">
          Discover all {directory?.length || 0} approved sites in this ring.
        </p>
      </div>

      {!directory || directory.length === 0 ? (
        <Card className="text-center py-12">
          <p className="text-gray-500">No sites have been approved for this ring yet.</p>
        </Card>
      ) : (
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
          {directory.map((site) => (
            <Card key={site.siteId} className="flex flex-col">
              <div className="flex-1">
                <a
                  href={site.url}
                  target="_blank"
                  rel="noopener noreferrer"
                  className="text-lg font-bold text-violet-600 hover:underline line-clamp-1"
                >
                  {site.name}
                </a>
                <a
                  href={site.url}
                  target="_blank"
                  rel="noopener noreferrer"
                  className="text-xs text-gray-400 hover:text-gray-600 block mt-1 line-clamp-1"
                >
                  {site.url}
                </a>
                {site.description && (
                  <p className="text-sm text-gray-700 mt-3 line-clamp-3">
                    {site.description}
                  </p>
                )}
              </div>
            </Card>
          ))}
        </div>
      )}
    </div>
  )
}
