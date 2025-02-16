using UnityEngine;

public class GenericBullet : MonoBehaviour
{
    private SpriteRenderer spriteRenderer => GetComponent<SpriteRenderer>();
    private BoxCollider2D boxCollider2D;
    private Vector2 moveSpeed = Vector2.zero;
    public GenericBullet Init(Vector2 speed, bool addBoxCollider)
    {
        if (addBoxCollider)
        {
            boxCollider2D = gameObject.AddComponent<BoxCollider2D>();
        }

        moveSpeed = speed;
        return this;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("BULLET ON TRIGGER ENTER!");   
    }

    private void Update()
    {
        transform.Translate(moveSpeed * Time.deltaTime);
    }
}
