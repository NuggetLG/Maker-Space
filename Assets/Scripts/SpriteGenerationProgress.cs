using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SpriteGenerationProgress : MonoBehaviour
{
    public float Progress { get; private set; }
    public bool IsGenerating { get; private set; }
    [SerializeField] private Slider slider;

    private void Start()
    {
        if (slider == null)
        {
            Debug.LogError("Slider is not assigned.");
        }
    }

    public void StartGeneration()
    {
        if (slider == null) return;

        IsGenerating = true;
        Progress = 0f;
        StartCoroutine(SimulateProgress());
    }

    public void CompleteGeneration()
    {
        if (slider == null) return;

        Progress = 1f;
        IsGenerating = false;
    }

    private IEnumerator SimulateProgress()
    {
        while (Progress < 0.9f && IsGenerating)
        {
            Progress = Mathf.Min(Progress + 0.1f * Time.deltaTime, 0.9f);
            yield return null;
        }
    }

    private void Update()
    {
        if (slider != null)
        {
            slider.value = Progress;
        }
    }
}