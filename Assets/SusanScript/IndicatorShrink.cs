using UnityEngine;

public class IndicatorShrink : MonoBehaviour
{
    public float shrinkTime = 2f;
    private Vector3 originalScale;
    private float timer;

    void Start()
    {
        originalScale = transform.localScale;
        timer = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime;
        float t = 1 - (timer / shrinkTime);
        transform.localScale = originalScale * t;

        if (timer >= shrinkTime)
        {
            Destroy(gameObject); // ³¬Ê±Ïú»Ù
        }
    }
}
