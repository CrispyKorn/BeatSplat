using TMPro;
using UnityEngine;

public class TitleColourCycle : MonoBehaviour
{
    private TextMeshProUGUI _titleText;

    private void Awake()
    {
        _titleText = GetComponent<TextMeshProUGUI>();
    }

    private async void OnEnable()
    {
        while (Locator.Instance.BeatManager is null) await Awaitable.NextFrameAsync();

        Locator.Instance.BeatManager.OnBeat += CycleColour;
    }

    private void OnDisable()
    {
        Locator.Instance.BeatManager.OnBeat -= CycleColour;
    }

    private void CycleColour(int beatCount)
    {
        //Debug.Log($"Cycle Beat: {beatCount}");
        _titleText.color = new Color(Random.value, Random.value, Random.value);
    }
}
