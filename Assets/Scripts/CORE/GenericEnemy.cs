using UnityEngine;

public class GenericEnemy : MonoBehaviour
{
    private Vector2 moveSpeed = Vector2.zero;
    public void Init(Vector2 speed)
    {
        moveSpeed = speed;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("ENEMY ON TRIGGER ENTER!");
    }

    private void Update()
    {
        transform.Translate(moveSpeed * Time.deltaTime);
    }
}
