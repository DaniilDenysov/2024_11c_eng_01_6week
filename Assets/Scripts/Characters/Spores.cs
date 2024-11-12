using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spores : MonoBehaviour
{
    [SerializeField] public float _animationLength = 1f;
    [SerializeField] public float _intensity = 4.2f;
    [SerializeField] public float _burstIntensity = 2f;
    [SerializeField] public AnimationCurve animationIntensity;
    [SerializeField] private ParticleSystem sporesParticleSystem;
    private ParticleSystem.Burst burst = new ParticleSystem.Burst(0, 0);

    private void Awake()
    {
        burst.cycleCount = Int32.MaxValue;
        burst.probability = 1;
        burst.repeatInterval = 0.16f;
    }

    private void Start()
    {
        var emissionModule = sporesParticleSystem.emission;
        burst.count = 0f;
        emissionModule.rateOverTime = 0;
        
        sporesParticleSystem.emission.SetBurst(0, burst);
    }

    public void StartAnimation(Vector3 direction, float length)
    {
        float angle = Vector3.Angle(direction, Vector3.up);
        
        transform.rotation = Quaternion.Euler(0, 0, direction.x > 0 ? -angle : angle);
        var startLifeTime = sporesParticleSystem.main.startLifetime;
        startLifeTime = new ParticleSystem.MinMaxCurve();
        startLifeTime.constantMin = 0.5f + 1.5f * length;
        startLifeTime.constantMax = 1f + 1.5f * length;
        StartCoroutine(PlayAnimation());
    }
    
    private IEnumerator PlayAnimation()
    {
        float elapsedTime = 0f;
        var emissionModule = sporesParticleSystem.emission;

        while (elapsedTime < _animationLength)
        {
            float curveCoefficient = animationIntensity.Evaluate(elapsedTime / _animationLength);
            
            emissionModule.rateOverTime = curveCoefficient * _intensity;
            
            burst.count = curveCoefficient * _burstIntensity;
            sporesParticleSystem.emission.SetBurst(0, burst);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
