using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Scroll_With_Controller : MonoBehaviour
{
    [SerializeField] GameObject currentSelected;
    [SerializeField] GameObject previousSelected;
    [SerializeField] RectTransform currentSelectTransform;

    [SerializeField] RectTransform contentTransform;
    [SerializeField] ScrollRect scrollRect;

    private void Update()
    {
        currentSelected = EventSystem.current.currentSelectedGameObject;
        if (currentSelected != null && currentSelected != previousSelected)
        {
            previousSelected = currentSelected;
            currentSelectTransform = currentSelected.GetComponent<RectTransform>();
            SnapTo(currentSelectTransform);
        }
    }

    private void SnapTo(RectTransform target)
    {
        Canvas.ForceUpdateCanvases();

        Vector2 newPosition = (Vector2)scrollRect.transform.InverseTransformPoint(contentTransform.position) - (Vector2)scrollRect.transform.InverseTransformPoint(target.position);

        newPosition.x = 0;

        contentTransform.anchoredPosition = newPosition;
    }
}
