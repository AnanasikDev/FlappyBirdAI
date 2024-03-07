using UnityEngine;

public class Bird : MonoBehaviour
{
    private Rigidbody2D rigidbody2d;
    public float jumpIntensity = 1f;
    public BirdModel model;

    private float horizontalDistance;
    private float distanceToEscape;
    private float height;
    private float verticalSpeed;

    public float value;

    private void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    public void Jump()
    {
        rigidbody2d.velocity = new Vector2(0, jumpIntensity);
    }

    public bool Decide()
    {
        value = Mathf.Pow(height, model.weights[0]) * model.weights[1] *
                      Mathf.Pow(verticalSpeed, model.weights[2]) * model.weights[3];// *
                      //Mathf.Pow(horizontalDistance, model.weights[4]) * model.weights[5] *
                      //Mathf.Pow(distanceToEscape, model.weights[6]) * model.weights[7];
        return value > 10;
    }

    private void Update()
    {
        height = transform.position.y + 5; // Always positive unless dead
        verticalSpeed = rigidbody2d.velocity.y;

        horizontalDistance = Vector3.Distance(transform.position, new Vector3(ColumnSpawner.instance.nextColumn.transform.position.x, transform.position.y));
        distanceToEscape = Vector3.Distance(transform.position, ColumnSpawner.instance.nextColumn.transform.position);

        if (Decide())
            Jump();
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
        model.r_time = GeneticAlgorithm.instance.timeSinceBegin;
        model.r_score = ScoreManager.instance.score;
        GeneticAlgorithm.instance.unitsLeft--;
        Destroy(gameObject);
    }
}

[System.Serializable]
public class BirdModel
{
    public float[] weights = new float[4];
    public float r_time;
    public int r_score;

    public void Randomize()
    {
        for (int i = 0; i < weights.Length; i++)
        {
            weights[i] = (Random.value - 0.5f) * 2;
        }
    }
    public void Inherit(float[] _weights)
    {
        for (int i = 0; i < weights.Length; i++)
        {
            weights[i] = _weights[i];
        }
    }
    public void Alter(float strictness)
    {
        for (int w = 0; w < weights.Length; w++)
        {
            weights[w] *= Random.Range(1f - strictness, 1f + strictness);
        }
    }
}