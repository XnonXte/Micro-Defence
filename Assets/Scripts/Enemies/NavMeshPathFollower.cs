using System.Collections;
using System.Collections.Generic;
using MicroDefense.Core;
using UnityEngine;
using UnityEngine.AI;

namespace MicroDefense.Enemies
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(EnemyBase))]
    public class NavMeshPathFollower : MonoBehaviour
    {
        [Header("Queue / Spacing Configuration")]
        [Tooltip("Jarak minimum antar enemy agar tidak numpuk. Sesuaikan dengan ukuran sprite.")]
        [SerializeField] private float minimumSpacing = 0.7f;

        [Tooltip("Radius deteksi enemy di depan. Harus >= minimumSpacing.")]
        [SerializeField] private float spacingCheckRadius = 1.0f;
        
        [Tooltip("Saat sedang menyerang Macro, abaikan spacing check agar bisa menempel lebih dekat.")]
        [SerializeField] private bool ignoreSpacingWhenBlocked = true;

        [Tooltip("Layer mask enemy. Set ke layer 'Enemy' di Inspector.")]
        [SerializeField] private LayerMask enemyLayer;

        [Tooltip("Jarak dari blocker tower agar enemy berhenti (biasanya = radius collider blocker).")]
        [SerializeField] private float stopDistanceFromBlocker = 0f;
        
        [Tooltip("Jarak minimum saat blocked agar virus bisa sangat dekat ke Macro (override stop distance).")]
        [SerializeField] private float blockedStopDistance = 0f;
        
        [Tooltip("Saat blocked, paksa enemy mendekat ke Macro meski ada spacing check (handled by priority).")]
        [SerializeField] private bool tightenStopWhenBlocked = true;

        private NavMeshAgent agent;
        private EnemyBase enemy;
        private Transform targetDestination;
        private Vector3 navTargetPosition;
        private bool hasNavTarget;
        private bool useFallbackMovement;
        private List<Transform> waypoints = new List<Transform>();
        private int currentWaypointIndex = 0;

        // Transform tower yang sedang mem-block enemy ini
        private Transform currentBlocker;
        
        // FIX: Track pause state untuk stop NavMeshAgent
        private GameState lastGameState = GameState.WaveInProgress;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            enemy = GetComponent<EnemyBase>();

            agent.updateRotation = false;
            agent.updateUpAxis = false;
            agent.stoppingDistance = 0f;

            // Matikan avoidance antar agent — spacing dijaga manual
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;

            if (enemyLayer == 0)
            {
                int layer = LayerMask.NameToLayer("Enemy");
                if (layer >= 0)
                {
                    enemyLayer = (1 << layer);
                }
                else
                {
                    // Jika layer Enemy tidak ada, gunakan layer Default (0) saja
                    // JANGAN gunakan AllLayers karena akan detect semua collider termasuk plasma drop
                    enemyLayer = (1 << 0); // Layer Default saja untuk enemy spacing check
                    Debug.LogWarning("[NavMeshPathFollower] Layer 'Enemy' not found. Using Default layer for spacing check.");
                }
            }
        }

        public void InitializeNavMesh(Transform destination)
        {
            targetDestination = destination;
            StartCoroutine(InitializeNavMeshWhenReady());
        }

        public void SetWaypoints(List<Transform> newWaypoints)
        {
            if (newWaypoints.Count > 0)
            {
                waypoints = new List<Transform>(newWaypoints);
                currentWaypointIndex = 0;
                targetDestination = waypoints[0];
                StartCoroutine(InitializeNavMeshWhenReady());
            }
        }

        private Transform GetCurrentTarget()
        {
            if (waypoints.Count > 0)
            {
                return waypoints[currentWaypointIndex];
            }
            return targetDestination;
        }

        private void AdvanceToNextWaypoint()
        {
            if (waypoints.Count == 0) return;
            
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Count)
            {
                // Reached final waypoint, head to final target
                targetDestination = waypoints[waypoints.Count - 1];
            }
            else
            {
                targetDestination = waypoints[currentWaypointIndex];
            }
        }

        private IEnumerator InitializeNavMeshWhenReady()
        {
            yield return null;

            if (agent == null || targetDestination == null) yield break;
            if (!agent.isOnNavMesh)
            {
                NavMeshHit spawnHit;
                if (NavMesh.SamplePosition(transform.position, out spawnHit, 5f, NavMesh.AllAreas))
                {
                    agent.Warp(spawnHit.position);
                }
            }

            if (!agent.isOnNavMesh)
            {
                useFallbackMovement = true;
                Debug.LogWarning($"[NavMeshPathFollower] Spawn position {transform.position} is not on NavMesh.");
                yield break;
            }

            yield return StartCoroutine(SetupNavMeshPathWhenReady());
        }

        private IEnumerator SetupNavMeshPathWhenReady()
        {
            for (int attempt = 0; attempt < 30; attempt++)
            {
                if (TrySetupNavMeshPath()) yield break;
                yield return new WaitForSeconds(0.1f);
            }

            useFallbackMovement = true;
            agent.isStopped = true;
            Debug.LogWarning($"[NavMeshPathFollower] No complete route after waiting for NavMesh. Using direct fallback movement from {transform.position}.");
        }

        private bool TrySetupNavMeshPath()
        {
            if (agent == null || targetDestination == null) return false;
            if (agent != null && enemy != null && enemy.Data != null)
                agent.speed = enemy.currentSpeed;

            if (agent != null && targetDestination != null)
            {
                Vector3 targetPos = targetDestination.position;
                NavMeshHit hit;

                if (NavMesh.SamplePosition(targetPos, out hit, 5f, NavMesh.AllAreas))
                {
                    targetPos = hit.position;
                }
                else
                {
                    return false;
                }

                NavMeshPath path = new NavMeshPath();
                if (!agent.CalculatePath(targetPos, path) || path.status != NavMeshPathStatus.PathComplete)
                {
                    return false;
                }

                useFallbackMovement = false;
                navTargetPosition = targetPos;
                hasNavTarget = true;
                agent.SetPath(path);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Resume pathfinding ke original destination
        /// </summary>
        public void ResumePathfinding()
        {
            if (agent == null || targetDestination == null) return;
            
            hasNavTarget = false; // Force recalculate
            agent.isStopped = false;
        }

        /// <summary>
        /// Copy target dan waypoints dari NavMeshPathFollower lain
        /// Digunakan oleh minion yang di-spawn oleh boss agar follow path yang sama
        /// </summary>
        public void CopyTargetFrom(NavMeshPathFollower source)
        {
            if (source == null) return;
            
            // Copy waypoints jika ada
            if (source.waypoints != null && source.waypoints.Count > 0)
            {
                waypoints = new List<Transform>(source.waypoints);
                currentWaypointIndex = source.currentWaypointIndex;
            }
            
            // Copy target destination
            targetDestination = source.targetDestination;
            
            // Initialize dengan target baru
            if (targetDestination != null)
            {
                StartCoroutine(InitializeNavMeshWhenReady());
            }
        }

        /// <summary>
        /// Dipanggil oleh MacrophageTower saat mulai/berhenti mem-block enemy ini.
        /// Kirim transform tower agar enemy berjalan tepat ke posisinya, bukan berhenti jauh.
        /// </summary>
        public void SetBlockedBy(bool blocked, Transform blockerTransform = null)
        {
            currentBlocker = blocked ? blockerTransform : null;

            if (blocked && blockerTransform != null && agent != null)
            {
                // Arahkan agent ke posisi blocker agar enemy jalan maju ke tower
                if (agent.isOnNavMesh) agent.isStopped = false;
                agent.stoppingDistance = 0f;
                agent.SetDestination(blockerTransform.position);
            }
            else if (!blocked && agent != null)
            {
                // Resume pathfinding ke original target saat unblock
                agent.stoppingDistance = 0f;
                ResumePathfinding();
            }
        }

        private void Update()
        {
            if (enemy == null || agent == null) return;

            agent.speed = enemy.currentSpeed;

            if (useFallbackMovement)
            {
                MoveDirectlyToTarget();
                return;
            }

            // Cek apakah agent masih valid di NavMesh
            if (!agent.isOnNavMesh)
            {
                useFallbackMovement = true;
                return;
            }

            // --- Prioritas 1: Sedang di-block oleh tower ---
            if (enemy.IsBlocked)
            {
                if (currentBlocker != null)
                {
                    // SIMPLEST SOLUTION: LANGSUNG SNAP KE POSISI TOWER, TANPA JARAK!
                    // Disable NavMeshAgent sepenuhnya
                    if (agent.enabled)
                    {
                        agent.enabled = false;
                    }
                    
                    // LANGSUNG SET POSISI = POSISI TOWER (NO DISTANCE CHECK)
                    Vector3 towerPosition = currentBlocker.position;
                    towerPosition.z = transform.position.z; // Keep Z
                    transform.position = towerPosition; // SNAP LANGSUNG, TIDAK ADA JARAK!
                }
                else
                {
                    // Tidak ada blocker → freeze
                    if (agent.enabled)
                    {
                        agent.isStopped = true;
                        agent.velocity = Vector3.zero;
                    }
                }
                return;
            }
            
            // Re-enable NavMeshAgent kalau tidak blocked lagi
            if (!agent.enabled && agent.isOnNavMesh)
            {
                agent.enabled = true;
            }

            // --- Prioritas 2: Spacing — ada enemy di depan, tunggu ---
            // SKIP spacing check saat enemy sedang blocked oleh tower!
            if (!enemy.IsBlocked && IsBlockedByEnemyAhead())
            {
                if (agent.enabled && agent.isOnNavMesh)
                {
                    agent.isStopped = true;
                    agent.velocity = Vector3.zero;
                }
                return;
            }

            // --- Jalan normal menuju tujuan akhir ---
            if (agent.isOnNavMesh)
            {
                agent.isStopped = false;
            }

            if (targetDestination != null && (!hasNavTarget || (navTargetPosition - targetDestination.position).sqrMagnitude > 0.04f))
            {
                Transform currentTarget = GetCurrentTarget();
                if (currentTarget == null) currentTarget = targetDestination;
                
                NavMeshHit hit;
                if (NavMesh.SamplePosition(currentTarget.position, out hit, 5f, NavMesh.AllAreas))
                {
                    NavMeshPath path = new NavMeshPath();
                    if (agent.isOnNavMesh && agent.CalculatePath(hit.position, path) && path.status == NavMeshPathStatus.PathComplete)
                    {
                        navTargetPosition = hit.position;
                        hasNavTarget = true;
                        agent.SetPath(path);
                    }
                }
            }

            // Cek apakah sudah sampai waypoint saat ini
            if (agent.isOnNavMesh && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f)
            {
                if (agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    // Only mark as reached if we're actually close to the target, not just blocked by other enemies
                    float distToTarget = Vector3.Distance(transform.position, GetCurrentTarget().position);
                    if (distToTarget > 1.5f) return; // Too far to be actual endpoint, just blocked by enemy
                    
                    // Jika ada waypoints dan belum sampai akhir, lanjut ke waypoint berikutnya
                    if (waypoints.Count > 0 && currentWaypointIndex < waypoints.Count - 1)
                    {
                        AdvanceToNextWaypoint();
                        hasNavTarget = false; // Force recalculate path ke waypoint berikutnya
                    }
                    else
                    {
                        // Sampai ke tujuan akhir
                        enemy.OnReachedEndPath();
                    }
                }
            }
        }

        private void MoveDirectlyToTarget()
        {
            if (targetDestination == null) return;

            Vector3 offset = targetDestination.position - transform.position;
            offset.z = 0f;
            float distance = offset.magnitude;
            if (distance <= stopDistanceFromBlocker)
            {
                enemy.OnReachedEndPath();
                return;
            }

            transform.position += offset.normalized * enemy.currentSpeed * Time.deltaTime;
        }

        private bool IsBlockedByEnemyAhead()
        {
            Vector3 moveDir = agent.desiredVelocity.magnitude > 0.01f
                ? agent.desiredVelocity.normalized
                : (targetDestination != null
                    ? (targetDestination.position - transform.position).normalized
                    : Vector3.zero);

            if (moveDir == Vector3.zero) return false;

            Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, spacingCheckRadius, enemyLayer);

            foreach (Collider2D col in nearby)
            {
                if (col.gameObject == gameObject) continue;

                Vector3 toOther = col.transform.position - transform.position;
                float dist = toOther.magnitude;
                if (dist < 0.001f) continue;

                float dot = Vector3.Dot(moveDir, toOther.normalized);
                // Only block if enemy is directly in front (dot > 0.5) AND within minimum spacing
                if (dot > 0.5f && dist < minimumSpacing)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
