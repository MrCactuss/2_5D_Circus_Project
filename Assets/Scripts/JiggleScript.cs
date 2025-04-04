using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JiggleScript : MonoBehaviour
{
    [Range(0f, 1f)]
    public float power = 0.1f;

    [Header("Position")]
    public bool jigPosition = true;
    public Vector3 positionJigAmount;
    [Range(0, 100)]
    public float positionFrequency = 8;
    float positionTime;

    [Header("Rotation")]
    public bool jigRotation = true;
    public Vector3 rotationJigAmount;
    [Range(0, 100)]
    public float rotationFrequency = 8;
    float rotationTime;

    [Header("Scale")]
    public bool jigScale = true;
    public Vector3 scaleJigAmount = new Vector3(.1f, -.1f, .1f);
    [Range(0, 100)]
    public float scaleFrequency = 8;
    float scaleTime;

    Vector3 basePostion;
    Quaternion baseRotation;
    Vector3 baseScale;


    void Start()
    {
        basePostion = transform.localPosition;
        baseRotation = transform.localRotation;
        baseScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        // var dt = Time.timeScale; will be paused if used Time.timeScale = 0;
        var dt = Time.unscaledDeltaTime;
        positionTime += dt * positionFrequency;
        rotationTime += dt * rotationFrequency;
        scaleTime += dt * scaleFrequency;

        if (jigPosition)
            transform.localPosition = basePostion + positionJigAmount * Mathf.Sin(positionTime) * power;

        if (jigRotation)
            transform.localRotation =
                baseRotation * Quaternion.Euler(rotationJigAmount * Mathf.Sin(positionTime) * power);

        if (jigScale)
            transform.localScale = baseScale + scaleJigAmount * Mathf.Sin(scaleTime) * power;
    }
}
