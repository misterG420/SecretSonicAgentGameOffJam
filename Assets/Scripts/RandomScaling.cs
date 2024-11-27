using System.Collections;
using UnityEngine;

public class RandomScaling : MonoBehaviour
{

    [SerializeField] private Vector2 scaleRange = new Vector2(0.9f, 1.1f);
    [SerializeField] private Vector2 durationRange = new Vector2(1f, 3f);


    private Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale;
        StartCoroutine(ScaleRandomly());
    }

    private IEnumerator ScaleRandomly()
    {
        while (true)
        {

            float targetScale = Random.Range(scaleRange.x, scaleRange.y);
            float duration = Random.Range(durationRange.x, durationRange.y);

            Vector3 targetVector = originalScale * targetScale;
            Vector3 startScale = transform.localScale;

            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                transform.localScale = Vector3.Lerp(startScale, targetVector, elapsedTime / duration);
                yield return null;
            }


            yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
        }
    }
}
