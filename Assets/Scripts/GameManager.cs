using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static float SimulationTime;

    [Header("Object")]
    public GameObject m_Chameleon;

    [Header("EA")]
    public int m_PopulationSize = 10;
    public int m_TournamentSize = 2;
    public float m_MutationRate = 0.02f;
    public float m_MaxGenerationTime = 15.0f;

    private int m_Generation = 0;
    private List<Chromosome> m_Population = new List<Chromosome>();

    public bool Alive => m_Population.Count((chromosome) => chromosome.m_Alive) > 0;
    public bool NotAlive => !Alive;
    public bool OverTime => SimulationTime >= m_MaxGenerationTime;

    private void Update()
    {
        SimulationTime += Time.deltaTime;
        if (OverTime || NotAlive)
        {
            m_Population.ForEach((chromosome) => chromosome.Kill());
            NextGeneration();
        }
    }

    private void Start()
    {
        InitializeRandomPopulation();
    }

    public void InitializeRandomPopulation()
    {
        for (int i = 0; i < m_PopulationSize; i++)
        {
            var chameleon = Instantiate(m_Chameleon);
            chameleon.transform.position = GetRandomPosition();

            var chromosome = chameleon.GetComponent<Chromosome>();
            chromosome.Initialize();

            m_Population.Add(chromosome);
        }
    }

    public Vector3 GetRandomPosition()
    {
        Vector3 position = Vector3.zero;
        position.x = Random.Range(-8.0f, 8.0f);
        position.y = Random.Range(-5.0f, 5.0f);
        return position;
    }

    public Chromosome Selection()
    {
        var list = new List<Chromosome>();
        for (int i = 0; i < m_TournamentSize; i++)
        {
            var index = Random.Range(0, m_PopulationSize);
            list.Add(m_Population[index]);
        }

        return list.OrderByDescending(x => x.m_LifeTime).ToList()[0];
    }

    public void Crossover(Chromosome parent1, Chromosome parent2, Chromosome offspring)
    {
        float red = (parent1.m_Red + parent2.m_Red) * 0.5f;
        float green = (parent1.m_Green + parent2.m_Green) * 0.5f;
        float blue = (parent1.m_Blue + parent2.m_Blue) * 0.5f;
        offspring.Initialize(red, green, blue);
    }

    public void Mutation(Chromosome offspring)
    {
        if (Random.Range(0.0f, 1.0f) > m_MutationRate) return;
        offspring.Initialize();
    }

    public float m_ElitismRate = 0.1f;

    public List<Chromosome> Elitism()
    {
        var amount = (int)(m_PopulationSize * m_ElitismRate);
        var population = m_Population.OrderByDescending(x => x.m_LifeTime).ToList();
        var list = new List<Chromosome>();

        for (int i = 0; i < amount; i++)
        {
            var chameleon = Instantiate(m_Chameleon);
            chameleon.transform.position = GetRandomPosition();

            var chromosome = chameleon.GetComponent<Chromosome>();
            var red = population[i].m_Red;
            var green = population[i].m_Green;
            var blue = population[i].m_Blue;
            chromosome.Initialize(red, green, blue);

            list.Add(chromosome);
        }
        return list;
    }

    public void NextGeneration()
    {
        var newPopulation = Elitism();
        while (newPopulation.Count < m_PopulationSize)
        {
            var parent1 = Selection();
            var parent2 = Selection();

            var chameleon = Instantiate(m_Chameleon);
            chameleon.transform.position = GetRandomPosition();

            var offspring = chameleon.GetComponent<Chromosome>();
            Crossover(parent1, parent2, offspring);
            Mutation(offspring);

            newPopulation.Add(offspring);
        }

        for (int i = 0; i < m_PopulationSize; i++)
            Destroy(m_Population[i].gameObject);

        m_Population = newPopulation;
        m_Generation++;
        SimulationTime = 0.0f;
    }
}
