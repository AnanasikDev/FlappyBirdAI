using UnityEngine;
using UnityEngine.SceneManagement;

// Human-controller bird for gameplay tests
public class Player : MonoBehaviour
{
    private Rigidbody2D rigidbody2d;
    public float jumpIntensity = 1f;

    private void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    public void Jump()
    {
        rigidbody2d.velocity = new Vector2(0, jumpIntensity);
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("ColumnCollider"))
        {
            Die();
        }
    }

    private void Die()
    {
        SceneManager.LoadScene(0);
    }
}
