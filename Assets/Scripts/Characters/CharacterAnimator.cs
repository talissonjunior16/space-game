using UnityEngine;
using UnityEngine.AI;

public class CharacterAnimator : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (agent != null && animator != null)
        {
            float speed = agent.velocity.magnitude / agent.speed;

            animator.SetFloat("SpeedPercent", speed, 0.1f, Time.deltaTime);
        }
    }

}
