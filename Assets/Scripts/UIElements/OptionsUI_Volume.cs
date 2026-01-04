using UnityEngine;
using UnityEngine.UI;

public class OptionsUI_Volume : MonoBehaviour
{
    public void OnValueChanged()
    {
        Slider slider = gameObject.GetComponent<Slider>();
        AudioManager.Volume = slider.value;
        Debug.Log($"Volume is: {slider.value}");
    }
}