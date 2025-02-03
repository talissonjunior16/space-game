using System.Collections.Generic;
using UnityEngine;

public class WalkableRobot : CharacterMovement
{
    public float detectionRadius = 20f;
    private Enemy targetEnemy;

    private void Update() {
        DetectAndFollowEnemy();
        OnMovementUpdate();
    }

    private void DetectAndFollowEnemy() {
        // If already following a target, check if it's still valid
        if (targetEnemy != null) {
            if (Vector3.Distance(transform.position, targetEnemy.transform.position) > detectionRadius) {
                Debug.Log("Lost target, searching for a new enemy...");
                targetEnemy = null;
                StopFollowingTarget();
            } else {
                return; // Keep following the current enemy
            }
        }

        // If no current target, search for a new one
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius);
        List<Enemy> enemies = new List<Enemy>();

        foreach (Collider col in colliders) {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null) {
                enemies.Add(enemy);
            }
        }

        if (enemies.Count > 0) {
            targetEnemy = GetClosestEnemy(enemies);
            FollowTarget(targetEnemy.transform);
        }
    }


    private Enemy GetClosestEnemy(List<Enemy> enemies) {
        Enemy closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (Enemy enemy in enemies) {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance) {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}