// 🆕 เพิ่มจากบทก่อน: using UnityEngine.EventSystems (ตรวจว่าเมาส์อยู่เหนือ UI)
// 🆕 เพิ่มจากบทก่อน: using URTS_GPT.MovementSystem (อ้างถึง UnitMoveAgent)
using UnityEngine;
using UnityEngine.AI;

namespace URTS_GPT.MovementSystem
{
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class UnitMoveAgent : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private NavMeshAgent agent;

        [Header("Tuning")]
        [SerializeField] private float stoppingDistance = 0.2f;
        [SerializeField] private bool rotateTowardsVelocity = true;
        [SerializeField] private float debugGizmoRadius = 0.15f;

        private Vector3? lastDestination;

        private void Reset()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.stoppingDistance = stoppingDistance;
            agent.autoBraking = true;
            agent.angularSpeed = 720f;
            agent.acceleration = 40f;
            agent.updateRotation = true;
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        }

        private void Awake()
        {
            if (agent == null) agent = GetComponent<NavMeshAgent>();
            agent.stoppingDistance = stoppingDistance;
            agent.updateRotation = rotateTowardsVelocity;
        }

        public void MoveTo(Vector3 worldPosition)
        {
            lastDestination = worldPosition;
            if (agent.isOnNavMesh)
            {
                agent.SetDestination(worldPosition);
            }
            else
            {
                Debug.LogWarning($"{name}: NavMeshAgent is not on a NavMesh.");
            }
        }

        public void Stop()
        {
            lastDestination = null;
            if (agent.isOnNavMesh)
            {
                agent.ResetPath();
            }
        }

        public bool IsMoving()
        {
            if (!agent.isOnNavMesh) return false;
            if (agent.pathPending) return true;
            if (agent.remainingDistance > Mathf.Max(stoppingDistance, 0.01f)) return true;
            return agent.velocity.sqrMagnitude > 0.01f;
        }

        private void OnDrawGizmosSelected()
        {
            if (lastDestination.HasValue)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(lastDestination.Value, debugGizmoRadius);
            }
        }
    }
}