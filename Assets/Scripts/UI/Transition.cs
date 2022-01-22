using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class Transition : MonoBehaviour
{
    private static Transition transition2D;
    private static Transition transition3D;

    public static bool TryGetTransition(in int dimension, out Transition instance)
    {
        instance = null;
        switch (dimension)
        {
            case 2: instance = transition2D; break;
            case 3: instance = transition3D; break;

        }

        if (instance == null)
            return false;

        return true;
    }

    [SerializeField]
    private int Dimension;

    [SerializeField]
    private ParticleSystem Particle;

    [SerializeField]
    private Image image;


    [SerializeField]
    private Transform StartMarker;
    [SerializeField]
    private Transform EndMarker;

    [SerializeField]
    private AnimationCurve moveCurve;
    [SerializeField]
    private Gradient fadeGradient;

    [SerializeField]
    private float runtime;
    [SerializeField]
    private float fadeRuntime;

    [SerializeField]
    private float actionTime;

    [SerializeField]
    private bool testRun;

    private CoroutineWrapper wrapper;

    private void Awake()
    {
        switch (Dimension)
        {
            case 2: transition2D = this; break;
            case 3: transition3D = this; break;
        }

        wrapper = new CoroutineWrapper(this);
    }

    public void Run()
    {
        image.transform.position = StartMarker.position;

        var color = image.color;
        color.a = 1;
        image.color = color;

        wrapper.StartSingleton(RunInternal());
    }

    IEnumerator RunInternal()
    {
        float t = 0;
        bool play = false;
        while (t < runtime)
        {
            if (t >= actionTime && !play)
            {
                Particle.Play();
            }

            image.transform.position = Vector3.Lerp(StartMarker.position, EndMarker.position, moveCurve.Evaluate(t / runtime));
            t += Time.deltaTime;
            yield return null;
        }
        image.transform.position = Vector3.Lerp(StartMarker.position, EndMarker.position, moveCurve.Evaluate(1));

        float fadeTime = 0f;
        while (fadeTime < fadeRuntime)
        {
            image.color = fadeGradient.Evaluate(fadeTime / fadeRuntime);
            fadeTime += Time.deltaTime;
            yield return null;
        }

        image.color = fadeGradient.Evaluate(1);
    }


    //fortest
    void Update()
    {
        if (testRun)
        {
            testRun = false;
            Run();
        }
    }
}