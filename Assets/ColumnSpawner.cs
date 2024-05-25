using System.Linq;
using UnityEngine;

public class ColumnSpawner : MonoBehaviour
{
    public Column column;
    public int number;
    public float distance;
    public int startX;
    public float heightVariance;

    private Column[] columns;
    public Column nextColumn { get { return columns.Where(c => c.transform.position.x > -7f).OrderBy(c => c.transform.position.x).FirstOrDefault(); } }

    public AnimationCurve speedCurve;
    public static ColumnSpawner instance;

    public float CalculateSpeed()
    {
        return speedCurve.Evaluate(ScoreManager.instance.score);
    }

    private void Start()
    {
        instance = this;
        columns = new Column[number];
        for (int i = 0; i < number; i++)
        {
            columns[i] = Instantiate(column, new Vector3(startX + i * distance, (Random.value - 0.5f) * heightVariance, 0), Quaternion.identity);
        }
    }

    private void FixedUpdate()
    {
        foreach (var column in columns)
        {
            column.transform.Translate(Vector3.left * CalculateSpeed());

            /*if (!column.passed && column.transform.position.x < -5)
            {
                column.passed = true;
                ScoreManager.instance.IncreaseScore(1);
            }
            if (column.transform.position.x < -20)
            {
                column.transform.position = new Vector3(startX, (Random.value - 0.5f) * heightVariance, 0);
                column.passed = false;
            }*/
        }
    }

    
    private void Update()
    {
        foreach (var column in columns)
        {
            if (!column.passed && column.transform.position.x < -6)
            {
                column.passed = true;
                ScoreManager.instance.IncreaseScore(1);
            }
            if (column.transform.position.x < -20)
            {
                column.transform.position = new Vector3(startX, (Random.value - 0.5f) * heightVariance, 0);
                column.passed = false;
            }
        }
    }

    public void Restart()
    {
        for (int i = 0; i < number; i++)
        {
            columns[i].transform.position = new Vector3(startX + i * distance, (Random.value - 0.5f) * heightVariance, 0);
            columns[i].passed = false;
        }
    }
}
