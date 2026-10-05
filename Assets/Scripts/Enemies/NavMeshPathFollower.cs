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

        [Tooltip("Layer mask enemy. Set ke layer 'Enemy' di Inspector.")]
        [SerializeField] private LayerMask enemyLayer;

        [Tooltip("Jarak dari blocker tower agar enemy berhenti (biasanya = radius collider blocker).")]
        [SerializeField] private float stopDistanceFromBlocker = 0.5f;

        private NavMeshAgent agent;
        private EnemyBase enemy;
        private Transform targetDestination;

        // Transform tower yang sedang mem-block enemy ini
        private Transform currentBlocker;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            enemy = GetComponent<EnemyBase>();

            agent.updateRotation = false;
            agent.updateUpAxis = false;

            // Matikan avoidance antar agent — spacing dijaga manual
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;

            if (enemyLayer == 0)
            {
                int layer = LayerMask.NameToLayer("Enemy");
                enemyLayer = (layer >= 0) ? (1 << layer) : Physics2D.AllLayers;
            }
        }

        public void InitializeNavMesh(Transform destination)
        {
            targetDestination = destination;
            if (agent != null && enemy != null && enemy.Data != null)
                agent.speed = enemy.currentSpeed;

            if (agent != null && targetDestination != null)
                agent.SetDestination(targetDestination.position);
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
                agent.isStopped = false;
                agent.SetDestination(blockerTransform.position);
            }
        }

        private void Update()
        {
            if (enemy == null || agent == null) return;

            agent.speed = enemy.currentSpeed;

            // --- Prioritas 1: Sedang di-block oleh tower ---
            if (enemy.IsBlocked)
            {
                if (currentBlocker != null)
                {
                    float distToBlocker = Vector3.Distance(transform.position, currentBlocker.position);

                    if (distToBlocker <= stopDistanceFromBlocker)
                    {
                        // Sudah cukup dekat → freeze total
                        agent.isStopped = true;
                        agent.velocity = Vector3.zero;
                    }
                    else
                    {
                        // Masih jauh → jalan ke arah tower (agar tidak berhenti jauh)
                        agent.isStopped = false;
                        agent.SetDestination(currentBlocker.position);
                    }
                }
                else
                {
                    // Tidak ada info blocker → freeze di tempat
                    agent.isStopped = true;
                    agent.velocity = Vector3.zero;
                }
                return;
            }

            // --- Prioritas 2: Spacing — ada enemy di depan, tunggu ---
            if (IsBlockedByEnemyAhead())
            {
                agent.isStopped = true;
                agent.velocity = Vector3.zero;
                return;
            }

            // --- Jalan normal menuju tujuan akhir ---
            agent.isStopped = false;
            if (agent.destination != targetDestination?.position && targetDestination != null)
                agent.SetDestination(targetDestination.position);

            // Cek apakah sudah sampai tujuan
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f)
            {
                if (agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    enemy.OnReachedEndPath();
                }
            }
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
                if (dot > 0.5f && dist < minimumSpacing)
                    return true;
            }

            return false;
        }
    }
}
