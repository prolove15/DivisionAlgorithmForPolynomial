using UnityEngine;

public class DrawScript : MonoBehaviour
{

    //----------------------------------------------- field
    [SerializeField]
    LineRenderer lineRenderer;
    
    //----------------------------------------------- methods
    void Start()
    {
        
    }

    void Update() {
        
    }

    void DrawTriangle(Vector3[] vertexPositions)
    {
        
        lineRenderer.positionCount = 3;
        lineRenderer.SetPositions(vertexPositions);
    }

    void DrawSineWave(Vector3 startPoint, float amplitude, float wavelength)
    {
        float x = 0f;
        float y;
        float k = 2 * Mathf.PI / wavelength;
        lineRenderer.positionCount = 200;
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            x += i * 0.001f;
            y = amplitude * Mathf.Sin(k * x);
            lineRenderer.SetPosition(i, new Vector3(x, y, 0) + startPoint);
        }
    }

    void DrawStandingSineWave(Vector3 startPoint, float amplitude, float wavelength, float waveSpeed)
    {

        float x = 0f;
        float y;
        float k = 2 * Mathf.PI / wavelength;
        float w = k * waveSpeed;
        lineRenderer.positionCount = 400;
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            x += i * 0.0005f;
            y = amplitude * (Mathf.Sin(k * x + w * Time.time) + Mathf.Sin(k * x - w * Time.time));
            lineRenderer.SetPosition(i, new Vector3(x, y, 0)+ startPoint);
        }
    }
}