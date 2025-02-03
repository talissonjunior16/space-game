using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CharacterMovement : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    protected Transform currentTarget { get; private set; }

    private void Awake() {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        OnMovementUpdate();
    }

    protected void OnMovementUpdate()
    {
        if (currentTarget != null)
        {
            // Follow the target
            navMeshAgent.SetDestination(currentTarget.position);

            // Rotate towards the target
            RotateTowards(currentTarget.position);
        }
        else if (navMeshAgent.hasPath)
        {
            // Rotate towards the destination
            RotateTowards(navMeshAgent.steeringTarget);
        }
    }
   
    public void MoveToPoint(Vector3 point)
    {
        StopFollowingTarget();
        navMeshAgent.SetDestination(point);
    }

    public void FollowTarget(Transform newTarget)
    {
        currentTarget = newTarget;
        navMeshAgent.stoppingDistance = 5f;
        navMeshAgent.updateRotation = false;
    }

    public void StopFollowingTarget()
    {
        currentTarget = null;
        navMeshAgent.stoppingDistance = 0f;
        navMeshAgent.updateRotation = true;
    }

    protected void RotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction.magnitude >= 0.1f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * navMeshAgent.angularSpeed / 100f);
        }
    }
}
