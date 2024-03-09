using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

// Genetic algorithm of selection chooses best models from current generation,
// copies their genes onto other models with subtle alterations (mutations).
public class GeneticAlgorithm : MonoBehaviour
{
    public GameObject unit; // prefab of a bird model
    public int numberOfUnits = 10; // number of units being trained simultaneously
    public int unitsLeft = 10; // number of units that are still alive in current iteration
    [SerializeField] private BirdModel[] models;

    public AnimationCurve successStrictnessByScore; // portion of the best to be selected for the next generation
    public AnimationCurve geneticStrictnessByMaxTime;
    public float successStrictness = 0.02f; // strictness of success criteria

    private float iterationBeginTime; // time in seconds since begin of current iteration. Used for success evaluation
    public float timeSinceBegin { get { return Time.time - iterationBeginTime; } }

    public static GeneticAlgorithm instance;

    public int iteration = 0;
    public TextMeshProUGUI iterationText;

    public int timeSpeed = 1; // time scale (2-7 recommended)
    private Bird[] birds;
    
    int unitVersionsNumber; // How many units will inherit genes of one unit of previous generation
    int topn; // number of units to be considered top

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
        // Begin new iteration

        Time.timeScale = timeSpeed; // dynamic time scale (for training & showcase)
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
            topn = Mathf.RoundToInt(numberOfUnits * successStrictness); // number of units to be selected as top of their generation
            List<BirdModel> topModels = models.OrderBy(m => ModelSuccessEvaluation(m)).ToList().GetRange(numberOfUnits-topn-1, topn);
            for (int i = 0; i < topModels.Count; i++)
            {
                // move top models configuration to a new list
                // to detach them of the models list
                topModels[i] = topModels[i].Clone();
            }
            float bestTime = timeSinceBegin; // at the end of generation best time is time since begin of that iteration
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
            
            // begin next iteration
            ScoreManager.instance.Restart();
            ColumnSpawner.instance.Restart();
            Begin();
        }
    }
}
