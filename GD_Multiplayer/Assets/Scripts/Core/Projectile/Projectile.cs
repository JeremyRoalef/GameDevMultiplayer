using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    Collider2D projectileCollider;

    [SerializeField]
    Rigidbody2D projectileRb;

    public void Initialize(float velocity)
    {
        projectileRb.linearVelocity = projectileRb.transform.up * velocity;
    }

    public Collider2D GetCollider() => projectileCollider;
}
