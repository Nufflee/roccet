
using UnityEngine;
using UnityEngine.UI;

public class TimeScale : MonoBehaviour
{
    public Slider TimeScaleSlider;
    public Text TimeScaleValueText;

    public PlayPause pp;

    void Update()
    {
        if (pp.isPaused)
            return;
        float value = TimeScaleSlider.value / 10f;
        Time.timeScale = value;
        TimeScaleValueText.text = value.ToString() + "x";
    }
}
