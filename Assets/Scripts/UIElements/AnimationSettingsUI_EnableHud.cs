using UnityEngine;
using TMPro;

public class AnimationSettingsUI_EnableHud : MonoBehaviour
{
    private void OnEnable()
    {
        OnValueChanged(GetComponent<TMP_Dropdown>().value);
    }

    public void OnValueChanged(int index)
    {
        string selected = gameObject.GetComponent<TMP_Dropdown>().options[index].text;
        Debug.Log("Selected Enable Hud: " + selected);
        AnimationSettingsUI_Core.SaveEnableHud(selected);
    }
}