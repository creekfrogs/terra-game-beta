using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerUIPopUpManager : MonoBehaviour
{
    [Header("Death Pop Up")]
    [SerializeField] GameObject deathPopUpObject;
    [SerializeField] CanvasGroup deathPopUpCanvasGroup;

    public void SendDeathPopUp()
    {
        deathPopUpObject.SetActive(true);
        StartCoroutine(FadeInPopUp(deathPopUpCanvasGroup, 5));
        StartCoroutine(FadeOutPopUp(deathPopUpCanvasGroup, 2, 5));
    }

    private IEnumerator FadeInPopUp(CanvasGroup canvas, float duration)
    {
        if (duration > 0)
        {
            canvas.alpha = 0;
            float timer = 0;

            yield return null;
            
            while (timer < duration)
            {
                timer += Time.deltaTime;
                canvas.alpha = Mathf.Lerp(canvas.alpha, 1, duration * Time.deltaTime);
                yield return null;
            }
        }
    }

    private IEnumerator FadeOutPopUp(CanvasGroup canvas, float duration, float delay)
    {
        if(duration > 0)
        {
            while (delay > 0)
            {
                delay -= Time.deltaTime;
                yield return null;
            }

            canvas.alpha = 1;
            float timer = 0;

            yield return null;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                canvas.alpha = Mathf.Lerp(canvas.alpha, 0, duration * Time.deltaTime);
                yield return null;
            }
        }

        canvas.alpha = 0;
        yield return null;
    }
}
