using UnityEngine;

public class GenericBullet : MonoBehaviour
{
    private SpriteRenderer spriteRenderer => GetComponent<SpriteRenderer>();
    private BoxCollider2D boxCollider2D;
    private Rigidbody2D rb;
    private Vector2 moveSpeed = Vector2.zero;
    public GenericBullet Init(Vector2 speed, bool addBoxCollider)
    {
        if (addBoxCollider)
        {
            boxCollider2D = gameObject.AddComponent<BoxCollider2D>();
            boxCollider2D.isTrigger = true;
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.isKinematic = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        moveSpeed = speed;
        return this;
    }

    //WIP. This probably needs more options to dispose bullets
    public void Die()
    {
        Destroy(gameObject);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        
    }

    private void Update()
    {
        transform.Translate(moveSpeed * Time.deltaTime);
    }
}
