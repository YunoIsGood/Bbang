using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class EffectManager : MonoBehaviour
{
    public List<Transform> effects;

    public void StartEffect(string effectName)
    {
        for(int i = 0; i < effects.Count; i++)
        {
            if(effects[i].name == effectName)
            {
                effects[i].gameObject.SetActive(true);
                effects[i].gameObject.SetActive(false);
                break;
            }
        }
    }
}
