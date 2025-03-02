using UnityEngine;
using UniRx;

public class GenericEnemy : MonoBehaviour
{
    public ReactiveProperty<Vector3?> DeathPosition = new ReactiveProperty<Vector3?>(null);
    private SpriteRenderer spriteRenderer => GetComponent<SpriteRenderer>();
    private BoxCollider2D boxCollider2D;
    private Rigidbody2D rb;
    private Vector2 moveSpeed = Vector2.zero;
    private bool dieOnBulletCollision;
    public GenericEnemy Init(Vector2 speed, bool addBoxCollider, bool dieOnBullet)
    {
        if (addBoxCollider)
        {
            boxCollider2D = gameObject.AddComponent<BoxCollider2D>();
            boxCollider2D.isTrigger = true;
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.isKinematic = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        dieOnBulletCollision = dieOnBullet;
        moveSpeed = speed;
        return this;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        GenericBullet bullet = collision.GetComponent<GenericBullet>();
        if (dieOnBulletCollision && bullet != null)
        {
            bullet.Die();
            Die(true);
        }
            
    }

    public virtual void Die(bool destroyGO)
    {
        DeathPosition.Value = transform.position;
        if (destroyGO)
            Destroy(gameObject);
    }

    protected virtual void Update()
    {
        transform.Translate(moveSpeed * Time.deltaTime);
    }
}
