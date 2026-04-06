import { useState } from 'react'
import { Button } from '../../components/Button'
import { Card } from '../../components/Card'
import { ApiKeyManager } from './ApiKeyManager'
import { useUpdateRingSettings, useUpdateActivityConfig, type ApprovalMode, type VerificationMode, type CrawlFrequency, type RingDetail } from '../../hooks/useRings'

interface Props {
  ring: RingDetail
}

export function RingSettingsPanel({ ring }: Props) {
  const update = useUpdateRingSettings()
  const updateActivity = useUpdateActivityConfig()

  const [isPublicJoinEnabled, setIsPublicJoinEnabled] = useState(ring.isPublicJoinEnabled)
  const [isApiOnboardingEnabled, setIsApiOnboardingEnabled] = useState(ring.isApiOnboardingEnabled)
  const [isPublicDirectoryEnabled, setIsPublicDirectoryEnabled] = useState(ring.isPublicDirectoryEnabled)
  const [verificationMode, setVerificationMode] = useState<VerificationMode>(ring.verificationMode)
  const [approvalMode, setApprovalMode] = useState<ApprovalMode>(ring.approvalMode)
  const [saved, setSaved] = useState(false)

  const [activityEnabled, setActivityEnabled] = useState(ring.activityConfig.isEnabled)
  const [crawlingEnabled, setCrawlingEnabled] = useState(ring.activityConfig.crawlingEnabled)
  const [postWeight, setPostWeight] = useState(ring.activityConfig.recentPostWeight)
  const [updateWeight, setUpdateWeight] = useState(ring.activityConfig.recentUpdateWeight)
  const [windowDays, setWindowDays] = useState(ring.activityConfig.activityWindowDays)
  const [crawlFrequency, setCrawlFrequency] = useState<CrawlFrequency>(ring.activityConfig.crawlFrequency)
  const [skipStaleSites, setSkipStaleSites] = useState(ring.activityConfig.skipStaleSites)
  const [staleSiteThresholdDays, setStaleSiteThresholdDays] = useState(ring.activityConfig.staleSiteThresholdDays)
  const [activitySaved, setActivitySaved] = useState(false)

  const publicJoinUrl = `${window.location.origin}/rings/${ring.slug}/join`
  const publicDirectoryUrl = `${window.location.origin}/rings/${ring.slug}/directory`

  async function handleSave() {
    await update.mutateAsync({ ringId: ring.id, isPublicJoinEnabled, isApiOnboardingEnabled, isPublicDirectoryEnabled, verificationMode, approvalMode })
    setSaved(true)
    setTimeout(() => setSaved(false), 2000)
  }

  async function handleActivitySave() {
    await updateActivity.mutateAsync({
      ringId: ring.id,
      isEnabled: activityEnabled,
      crawlingEnabled,
      recentPostWeight: postWeight,
      recentUpdateWeight: updateWeight,
      activityWindowDays: windowDays,
      crawlFrequency,
      skipStaleSites,
      staleSiteThresholdDays,
    })
    setActivitySaved(true)
    setTimeout(() => setActivitySaved(false), 2000)
  }

  return (
    <div className="space-y-6">
      <Card>
        <h2 className="font-semibold text-gray-900 mb-4">Onboarding settings</h2>
        <div className="space-y-4">
          {/* Verification mode */}
          <div>
            <p className="text-sm font-medium text-gray-700 mb-2">Verification mode</p>
            <div className="space-y-2">
              {(['None', 'Widget', 'Auto'] as VerificationMode[]).map((mode) => (
                <label key={mode} className="flex items-start gap-3 cursor-pointer">
                  <input
                    type="radio"
                    className="mt-0.5"
                    name="verificationMode"
                    value={mode}
                    checked={verificationMode === mode}
                    onChange={() => setVerificationMode(mode)}
                  />
                  <div>
                    <p className="text-sm font-medium text-gray-900">
                      {mode === 'None' ? 'None (Trust-based)' : mode === 'Widget' ? 'Widget (Visual proof)' : 'Auto System Verification'}
                    </p>
                    <p className="text-xs text-gray-500">
                      {mode === 'None'
                        ? 'No ownership proof is required to join.'
                        : mode === 'Widget'
                        ? 'The ring owner must verify the widget visually on the applicant site.'
                        : 'The system automatically verifies the widget exists on the site.'}
                    </p>
                  </div>
                </label>
              ))}
            </div>
          </div>

          {/* Approval mode */}
          <div className="pt-4 border-t border-gray-100">
            <p className="text-sm font-medium text-gray-700 mb-2">Approval mode</p>
            <div className="space-y-2">
              {(['Auto', 'Manual'] as ApprovalMode[]).map((mode) => (
                <label key={mode} className="flex items-start gap-3 cursor-pointer">
                  <input
                    type="radio"
                    className="mt-0.5"
                    name="approvalMode"
                    value={mode}
                    checked={approvalMode === mode}
                    onChange={() => setApprovalMode(mode)}
                  />
                  <div>
                    <p className="text-sm font-medium text-gray-900">
                      {mode === 'Auto' ? 'Auto approve' : 'Manual approve'}
                    </p>
                    <p className="text-xs text-gray-500">
                      {mode === 'Auto'
                        ? 'Applications are automatically approved once verified (or immediately if verification is none).'
                        : 'You must manually approve each application even after verification passes.'}
                    </p>
                  </div>
                </label>
              ))}
            </div>
          </div>

          {/* Public join page toggle */}
          <div className="flex items-start justify-between gap-4 pt-4 border-t border-gray-100">
            <div>
              <p className="text-sm font-medium text-gray-900">Public join page</p>
              <p className="text-xs text-gray-500 mt-0.5">
                Anyone can submit their site at a public URL without a Smallorbit account.
              </p>
              {isPublicJoinEnabled && (
                <a
                  href={publicJoinUrl}
                  target="_blank"
                  rel="noopener noreferrer"
                  className="text-xs text-violet-600 hover:underline mt-1 block"
                >
                  {publicJoinUrl}
                </a>
              )}
            </div>
            <button
              type="button"
              onClick={() => setIsPublicJoinEnabled((v) => !v)}
              className={`relative inline-flex h-6 w-11 shrink-0 rounded-full border-2 border-transparent transition-colors focus:outline-none ${
                isPublicJoinEnabled ? 'bg-violet-600' : 'bg-gray-200'
              }`}
            >
              <span
                className={`inline-block h-5 w-5 transform rounded-full bg-white shadow transition-transform ${
                  isPublicJoinEnabled ? 'translate-x-5' : 'translate-x-0'
                }`}
              />
            </button>
          </div>

          {/* Public directory page toggle */}
          <div className="flex items-start justify-between gap-4 pt-4 border-t border-gray-100">
            <div>
              <p className="text-sm font-medium text-gray-900">Public directory page</p>
              <p className="text-xs text-gray-500 mt-0.5">
                Expose a public page showing all approved sites.
              </p>
              {isPublicDirectoryEnabled && (
                <a
                  href={publicDirectoryUrl}
                  target="_blank"
                  rel="noopener noreferrer"
                  className="text-xs text-violet-600 hover:underline mt-1 block"
                >
                  {publicDirectoryUrl}
                </a>
              )}
            </div>
            <button
              type="button"
              onClick={() => setIsPublicDirectoryEnabled((v) => !v)}
              className={`relative inline-flex h-6 w-11 shrink-0 rounded-full border-2 border-transparent transition-colors focus:outline-none ${
                isPublicDirectoryEnabled ? 'bg-violet-600' : 'bg-gray-200'
              }`}
            >
              <span
                className={`inline-block h-5 w-5 transform rounded-full bg-white shadow transition-transform ${
                  isPublicDirectoryEnabled ? 'translate-x-5' : 'translate-x-0'
                }`}
              />
            </button>
          </div>

          {/* API onboarding toggle */}
          <div className="flex items-start justify-between gap-4 pt-4 border-t border-gray-100">
            <div>
              <p className="text-sm font-medium text-gray-900">API onboarding</p>
              <p className="text-xs text-gray-500 mt-0.5">
                Allow ring-scoped API keys to submit applications programmatically.
              </p>
            </div>
            <button
              type="button"
              onClick={() => setIsApiOnboardingEnabled((v) => !v)}
              className={`relative inline-flex h-6 w-11 shrink-0 rounded-full border-2 border-transparent transition-colors focus:outline-none ${
                isApiOnboardingEnabled ? 'bg-violet-600' : 'bg-gray-200'
              }`}
            >
              <span
                className={`inline-block h-5 w-5 transform rounded-full bg-white shadow transition-transform ${
                  isApiOnboardingEnabled ? 'translate-x-5' : 'translate-x-0'
                }`}
              />
            </button>
          </div>

          <div className="flex justify-end pt-2">
            <Button loading={update.isPending} onClick={handleSave}>
              {saved ? 'Saved' : 'Save settings'}
            </Button>
          </div>
        </div>
      </Card>

      {isApiOnboardingEnabled && (
        <Card>
          <h2 className="font-semibold text-gray-900 mb-4">API keys</h2>
          <ApiKeyManager ringId={ring.id} />
        </Card>
      )}

      <Card>
        <h2 className="font-semibold text-gray-900 mb-1">Activity-based navigation</h2>
        <p className="text-sm text-gray-500 mb-4">
          When enabled, sites are ordered by recent activity rather than join order.
        </p>
        <div className="space-y-4">

          {/* Master toggle */}
          <div className="flex items-start justify-between gap-4">
            <div>
              <p className="text-sm font-medium text-gray-900">Enable activity-based navigation</p>
              <p className="text-xs text-gray-500 mt-0.5">Reorder the ring based on site activity scores.</p>
            </div>
            <Toggle value={activityEnabled} onChange={setActivityEnabled} />
          </div>

          {activityEnabled && (
            <>
              {/* Activity window */}
              <div className="pt-4 border-t border-gray-100">
                <label className="text-sm font-medium text-gray-700">
                  Activity window
                </label>
                <div className="flex items-center gap-3 mt-2">
                  <input
                    type="number"
                    min={1}
                    max={365}
                    value={windowDays}
                    onChange={(e) => setWindowDays(Number(e.target.value))}
                    className="w-20 rounded-lg border border-gray-300 px-3 py-1.5 text-sm text-gray-900 focus:border-violet-500 focus:outline-none"
                  />
                  <span className="text-sm text-gray-500">days</span>
                </div>
                <p className="text-xs text-gray-400 mt-1">Only activity within this window counts toward the score.</p>
              </div>

              {/* Scoring weights */}
              <div className="pt-4 border-t border-gray-100">
                <p className="text-sm font-medium text-gray-700 mb-3">Scoring weights</p>
                <div className="space-y-3">
                  <WeightSlider label="New posts" value={postWeight} onChange={setPostWeight} />
                  <WeightSlider label="Site updates" value={updateWeight} onChange={setUpdateWeight} />
                </div>
                <p className="text-xs text-gray-400 mt-2">Higher weight = more influence on a site's position in the ring.</p>
              </div>

              {/* Crawling */}
              <div className="pt-4 border-t border-gray-100 space-y-4">
                <div className="flex items-start justify-between gap-4">
                  <div>
                    <p className="text-sm font-medium text-gray-900">Enable site crawling</p>
                    <p className="text-xs text-gray-500 mt-0.5">
                      Smallorbit will periodically visit member sites to detect new posts and updates.
                    </p>
                  </div>
                  <Toggle value={crawlingEnabled} onChange={setCrawlingEnabled} />
                </div>

                {crawlingEnabled && (
                  <div>
                    <label className="text-sm font-medium text-gray-700">Crawl frequency</label>
                    <select
                      value={crawlFrequency}
                      onChange={(e) => setCrawlFrequency(e.target.value as CrawlFrequency)}
                      className="mt-2 w-full rounded-lg border border-gray-300 px-3 py-2 text-sm text-gray-900 focus:border-violet-500 focus:outline-none"
                    >
                      <option value="Every6Hours">Every 6 hours</option>
                      <option value="Daily">Daily</option>
                      <option value="Every3Days">Every 3 days</option>
                      <option value="Weekly">Weekly</option>
                    </select>
                  </div>
                )}
              </div>

              {/* Skip stale sites */}
              <div className="pt-4 border-t border-gray-100 space-y-4">
                <div className="flex items-start justify-between gap-4">
                  <div>
                    <p className="text-sm font-medium text-gray-900">Skip stale sites</p>
                    <p className="text-xs text-gray-500 mt-0.5">
                      Sites with no detected activity within the threshold are bypassed during navigation.
                    </p>
                  </div>
                  <Toggle value={skipStaleSites} onChange={setSkipStaleSites} />
                </div>

                {skipStaleSites && (
                  <div>
                    <label className="text-sm font-medium text-gray-700">Stale threshold</label>
                    <div className="flex items-center gap-3 mt-2">
                      <input
                        type="number"
                        min={1}
                        max={365}
                        value={staleSiteThresholdDays}
                        onChange={(e) => setStaleSiteThresholdDays(Number(e.target.value))}
                        className="w-20 rounded-lg border border-gray-300 px-3 py-1.5 text-sm text-gray-900 focus:border-violet-500 focus:outline-none"
                      />
                      <span className="text-sm text-gray-500">days without activity</span>
                    </div>
                    <p className="text-xs text-gray-400 mt-1">Sites are still ring members — they rejoin navigation automatically once activity resumes.</p>
                  </div>
                )}
              </div>
            </>
          )}

          <div className="flex justify-end pt-2">
            <Button loading={updateActivity.isPending} onClick={handleActivitySave}>
              {activitySaved ? 'Saved' : 'Save'}
            </Button>
          </div>
        </div>
      </Card>
    </div>
  )
}

function Toggle({ value, onChange }: { value: boolean; onChange: (v: boolean) => void }) {
  return (
    <button
      type="button"
      onClick={() => onChange(!value)}
      className={`relative inline-flex h-6 w-11 shrink-0 rounded-full border-2 border-transparent transition-colors focus:outline-none ${
        value ? 'bg-violet-600' : 'bg-gray-200'
      }`}
    >
      <span
        className={`inline-block h-5 w-5 transform rounded-full bg-white shadow transition-transform ${
          value ? 'translate-x-5' : 'translate-x-0'
        }`}
      />
    </button>
  )
}

function WeightSlider({ label, value, onChange }: { label: string; value: number; onChange: (v: number) => void }) {
  return (
    <div className="flex items-center gap-3">
      <span className="text-sm text-gray-700 w-24 shrink-0">{label}</span>
      <input
        type="range"
        min={0}
        max={10}
        step={0.5}
        value={value}
        onChange={(e) => onChange(Number(e.target.value))}
        className="flex-1 accent-violet-600"
      />
      <span className="text-sm text-gray-500 w-8 text-right">{value}</span>
    </div>
  )
}
