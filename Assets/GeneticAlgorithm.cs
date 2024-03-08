using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GeneticAlgorithm : MonoBehaviour
{
    public GameObject unit;
    public int numberOfUnits = 10;
    public int unitsLeft = 10;
    [SerializeField] private BirdModel[] models;

    public AnimationCurve successStrictnessByScore; // portion of the best to be selected for the next generation
    public AnimationCurve geneticStrictnessByMaxTime;
    public float successStrictness { get
        {
            return 0.02f;
            float v = successStrictnessByScore.Evaluate(ScoreManager.instance.maxScore);
            if (v > 0.375f) return 0.5f;
            if (v > 0.225f) return 0.25f;
            if (v > 0.15f) return 0.2f;
            if (v > 0.075f) return 0.1f;
            if (v > 0.035f) return 0.05f;
            if (v > 0.015f) return 0.02f;
            if (v > 0.005f) return 0.01f;
            return 0.5f;
        } }
    private int unitVersionsNumber;

    private float iterationBeginTime;
    public float timeSinceBegin { get { return Time.time - iterationBeginTime; } }

    public static GeneticAlgorithm instance;

    public int iteration = 0;
    public TextMeshProUGUI iterationText;

    public int timeSpeed = 1;
    private Bird[] birds;
    int topn; // number of units to be considered top

    private int mutationi = 0; // counting until next big mutation
    public int score25; // score gathered in 25 iterations in total

    private void Start()
    {
        Time.timeScale = timeSpeed;
        unitVersionsNumber = (int)(1f / successStrictness);
        instance = this;
        models = new BirdModel[numberOfUnits];
        birds = new Bird[numberOfUnits];
        for (int i = 0; i < numberOfUnits; i++)
        {
            birds[i] = Instantiate(unit).GetComponent<Bird>();
            birds[i].Init();
            models[i] = new BirdModel();
            models[i].Randomize();
            birds[i].model = models[i];
        }

        Begin();
    }
    private void Begin()
    {
        Time.timeScale = timeSpeed;
        iteration++;
        iterationText.text = $"Iteration: {iteration}";
        unitsLeft = numberOfUnits; 
        iterationBeginTime = Time.time;
        foreach (var bird in birds)
        {
            bird.Revive();
        }
    }

    private float ModelSuccessEvaluation(BirdModel m)
    {
        return m.r_success;
    }
    private void Update()
    {
        if (unitsLeft <= 0)
        {
            topn = Mathf.RoundToInt(numberOfUnits * successStrictness);
            List<BirdModel> topModels = models.OrderBy(m => ModelSuccessEvaluation(m)).ToList().GetRange(numberOfUnits-topn-1, topn);
            for (int i = 0; i < topModels.Count; i++)
            {
                // move top models configuration to a new list
                // to detach them of the models list
                topModels[i] = topModels[i].Clone();
            }
            float bestTime = timeSinceBegin;
            /*mutationi++;
            score25 += ScoreManager.instance.score;
            float mutation = 1;
            if (mutationi >= 25)
            {
                mutationi = 0;
                if (score25 < 40)
                    mutation = 5;
                score25 = 0;
            }*/
            for (int p = 0; p < topn; p++)
            {
                // for each top unit produce unitVersionsNumber clones with slightly different parameters
                for (int i = 0; i < unitVersionsNumber; i++)
                {
                    var m = models[p * unitVersionsNumber + i];
                    m.Inherit(topModels[p].weights);
                    m.Alter(geneticStrictnessByMaxTime.Evaluate(ScoreManager.instance.maxTime));
                }
            }
            ScoreManager.instance.UpdateRecord(ScoreManager.instance.score, bestTime);
            ScoreManager.instance.Restart();
            ColumnSpawner.instance.Restart();
            Begin();
        }
    }
}
