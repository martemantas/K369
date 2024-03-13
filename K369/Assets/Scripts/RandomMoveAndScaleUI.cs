using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RandomMoveAndScaleUI : MonoBehaviour
{
    public RectTransform[] uiElements;
    public float duration = 0.5f;

    public void OnButtonClick()
    {
        foreach (RectTransform uiElement in uiElements)
        {
            StartCoroutine(MoveAndScaleElement(uiElement));
        }
    }

    private IEnumerator MoveAndScaleElement(RectTransform uiElement)
    {
        Vector2 originalPosition = uiElement.anchoredPosition;
        Vector3 originalScale = uiElement.localScale;

        float distance = Random.Range(50, 151);
        Vector2 direction = Random.insideUnitCircle.normalized;
        Vector2 targetPosition = originalPosition + direction * distance;

        float scaleFactor = Random.Range(0.8f, 1.2f);
        Vector3 targetScale = new Vector3(scaleFactor, scaleFactor, 1);

        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            uiElement.anchoredPosition = Vector2.Lerp(originalPosition, targetPosition, elapsedTime / duration);
            uiElement.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        uiElement.anchoredPosition = targetPosition;
        uiElement.localScale = targetScale;
    }
}