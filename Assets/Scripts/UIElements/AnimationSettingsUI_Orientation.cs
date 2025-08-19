using UnityEngine;
using TMPro;

public class AnimationSettingsUI_Orientation : MonoBehaviour
{
    private void OnEnable()
    {
        OnValueChanged(GetComponent<TMP_Dropdown>().value);
    }
    public void OnValueChanged(int index)
    {
        string selected = gameObject.GetComponent<TMP_Dropdown>().options[index].text;
        Debug.Log("Selected Orientation: " + selected);
        AnimationSettingsUI_Core.SaveOrientation(selected);
    }
}