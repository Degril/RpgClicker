﻿using UnityEngine;
using VContainer.Unity;

public abstract class MonoBehaviourDi : MonoBehaviour
{
    private void Awake()
    {
        if(Application.isPlaying)
            LifetimeScope.Inject(gameObject);
    }
}
