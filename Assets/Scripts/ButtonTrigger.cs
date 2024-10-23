using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonTrigger : MonoBehaviour
{
    public RectTransform[] buttons;
    public float expandedSize = 1.5f;
    public float normalSize = 1.0f;
    [Range(0.01f, 0.09f)]
    public float AnimationSpeed;
    [SerializeField] private VerticalLayoutGroup verticalLayoutGroup;

    private GameObject currentHoveredButton;
    private Vector2 originalSize;

    private void Start()
    {
        originalSize = buttons[0].sizeDelta;
    }

    public void OnButtonHovered(GameObject hoveredButton)
    {
        currentHoveredButton = hoveredButton;
    }

    public void OnButtonExit()
    {
        currentHoveredButton = null;
    }

    private void Update()
    {
        //bool needsRebuild = false;

        for (int i = 0; i < buttons.Length; i++)
        {
            RectTransform button = buttons[i];
            Vector2 targetSize;

            if (button.gameObject == currentHoveredButton)
            {
                targetSize = originalSize * expandedSize;
            }
            else
            {
                targetSize = originalSize;
            }

            Vector2 newSize = Vector2.Lerp(button.sizeDelta, targetSize, AnimationSpeed);

            if (newSize != button.sizeDelta)
            {
                button.sizeDelta = newSize;
            }
        }
    }
}