using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProbChoose
{
    public static int Choose(float[] Probs)
    {
        float total = 0;

        foreach (float elem in Probs)
        {
            total += elem;
        }

        float randomPoint = Random.value * total;

        for (int i = 0; i < Probs.Length; i++)
        {
            if (randomPoint < Probs[i])
            {
                return i;
            }
            else
            {
                randomPoint -= Probs[i];
            }
        }
        return Probs.Length - 1;
    }
}

