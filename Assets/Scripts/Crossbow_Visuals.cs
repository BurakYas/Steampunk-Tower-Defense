using System.Collections;
using UnityEngine;

public class Crossbow_Visuals : MonoBehaviour
{
    private Tower_Crossbow myTower;

    [SerializeField] private LineRenderer attackVisuals;
    [SerializeField] private float attackVisualsDuration = .1f;

    [Header("Glowing Visuals")]
    [SerializeField] private MeshRenderer meshRenderer;
    private Material material;

    [Space]
    [SerializeField] private float maxIntensity = 150;
    private float currentIntensity;
    [Space]
    [SerializeField] private Color startColor;
    [SerializeField] private Color endColor;

    [Header("Rotor Visuals")]
    [SerializeField] private Transform rotor;
    [SerializeField] private Transform rotorUnloaded;
    [SerializeField] private Transform rotorLoaded;

    [Header("Front Glow String")]
    [SerializeField] private LineRenderer frontString_L;
    [SerializeField] private LineRenderer frontString_R;

    [Space]

    [SerializeField] private Transform frontStartPoint_L;
    [SerializeField] private Transform frontStartPoint_R;
    [SerializeField] private Transform frontEndPoint_L;
    [SerializeField] private Transform frontEndPoint_R;

    [Header("Back Glow String")]
    [SerializeField] private LineRenderer backString_L;
    [SerializeField] private LineRenderer backString_R;

    [Space]

    [SerializeField] private Transform backStartPoint_L;
    [SerializeField] private Transform backStartPoint_R;
    [SerializeField] private Transform backEndPoint_L;
    [SerializeField] private Transform backEndPoint_R;

    [SerializeField] private LineRenderer[] lineRenderers;


    private void Awake()
    {
        myTower = GetComponent<Tower_Crossbow>();

        material = new Material(meshRenderer.material);

        meshRenderer.material = material; // Set the material to the new material

        UpdateMaterailsOnlineRenderers();

        StartCoroutine(ChangeEmission(1));
    }

    private void UpdateMaterailsOnlineRenderers()
    {
        foreach (var lr in lineRenderers)
        {
            lr.material = material;
        }
    }

    private void Update()
    {
        UpdateEmissionColor();

        UpdateStrings();
    }

    private void UpdateStrings()
    {
        UpdateStringVisual(frontString_L, frontStartPoint_L, frontEndPoint_L);
        UpdateStringVisual(frontString_R, frontStartPoint_R, frontEndPoint_R);

        UpdateStringVisual(backString_L, backStartPoint_L, backEndPoint_L);
        UpdateStringVisual(backString_R, backStartPoint_R, backEndPoint_R);
    }

    private void UpdateEmissionColor()
    {
        Color emissionColor = Color.Lerp(startColor, endColor, currentIntensity / maxIntensity); // Lerp between the start and end color based on the current intensity value

        emissionColor = emissionColor * Mathf.LinearToGammaSpace(currentIntensity); // Convert the color to gamma space

        material.SetColor("_EmissionColor", emissionColor); // Set the emission color of the material
    }

    public void PlayReoladFX(float duration)
    {
        float newDuration = duration / 2;

        StartCoroutine(ChangeEmission(newDuration));
        StartCoroutine(UpdateRotorPosition(newDuration));
    }

    public void PlayAttackVFX(Vector3 startPoint, Vector3 endPoint)
    {
        StartCoroutine(VFXCoroutione(startPoint,endPoint));
    }

    private IEnumerator VFXCoroutione(Vector3 startPoint, Vector3 endPoint)
    {
        myTower.EnableRotation(false);

        attackVisuals.enabled = true;

        attackVisuals.SetPosition(0, startPoint); // Set the start point of the attack visuals
        attackVisuals.SetPosition(1, endPoint); // Set the end point of the attack visuals

        yield return new WaitForSeconds(attackVisualsDuration);
        attackVisuals.enabled = false;

        myTower.EnableRotation(true);
    }

    private IEnumerator ChangeEmission(float duration)
    {
        float startTime = Time.time;
        float startIntensity = 3;

        while (Time.time - startTime < duration)
        {
            float tVlaue = (Time.time - startTime) / duration;
            currentIntensity = Mathf.Lerp(startIntensity, maxIntensity, tVlaue); // Lerp between the start and max intensity value over time
            yield return null;
        }

        currentIntensity = maxIntensity;
    }

    private IEnumerator UpdateRotorPosition(float duration)
    {
        float startTime = Time.time;

        while (Time.time - startTime < duration)
        {
            float tValue = (Time.time - startTime) / duration;
            rotor.position = Vector3.Lerp(rotorUnloaded.position, rotorLoaded.position, tValue); // Lerp between the unloaded and loaded position of the rotor
            yield return null;
        }

        rotor.position = rotorLoaded.position;
    }

    private void UpdateStringVisual(LineRenderer lineRenderer, Transform startPoint, Transform endPoint)
    {
        lineRenderer.SetPosition(0, startPoint.position);
        lineRenderer.SetPosition(1, endPoint.position);
    }
}
