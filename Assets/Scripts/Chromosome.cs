using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chromosome : MonoBehaviour
{
    [Header("Genes")]
    public float m_Red;
    public float m_Green;
    public float m_Blue;

    [Header("Fitness")]
    public float m_LifeTime;
    private bool m_Alive;

    public void Initialize()
    {
        m_Red = Random.Range(0.0f, 1.0f);
        m_Green = Random.Range(0.0f, 1.0f);
        m_Blue = Random.Range(0.0f, 1.0f);
        Initialize(m_Red, m_Green, m_Blue);
    }

    public void Initialize(float red, float green, float blue)
    {
        m_Red = red;
        m_Green = green;
        m_Blue = blue;
        m_LifeTime = 0.0f;
        m_Alive = true;
        gameObject.SetActive(true);
    }


    public void Kill()
    {
        if (!m_Alive) return;
        m_Alive = false;
        m_LifeTime = GameManager.SimulationTime;
        gameObject.SetActive(false);
    }

    private void OnMouseDown()
    {
        Kill();
    }
}
