using UnityEngine;
using TMPro;

public class AnimationSettingsUI_Pace : MonoBehaviour
{
    private void OnEnable()
    {
        OnValueChanged(GetComponent<TMP_Dropdown>().value);
    }
    public void OnValueChanged(int index)
    {
        string selected = gameObject.GetComponent<TMP_Dropdown>().options[index].text;
        Debug.Log("Selected Pace: " + selected);
        AnimationSettingsUI_Core.SavePace(selected);
    }
}