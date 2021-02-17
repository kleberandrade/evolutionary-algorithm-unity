﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static float SimulationTime;
    public GameObject m_Chameleon;
    public int m_PopulationSize = 10;
    public List<Chromosome> m_Population = new List<Chromosome>();

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

    public int m_TournamentSize = 2;

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

    public float m_MutationRate = 0.02f;

    public void Mutation(Chromosome offspring)
    {
        if (Random.Range(0.0f, 1.0f) > m_MutationRate) return;
        offspring.Initialize();
    }

    public void NextGeneration()
    {
        var newPopulation = new List<Chromosome>();

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
    }
}