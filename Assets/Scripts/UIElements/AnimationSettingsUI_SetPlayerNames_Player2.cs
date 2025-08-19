using TMPro;
using UnityEngine;

public class AnimationSettingsUI_SetPlayerNames_Player2 : MonoBehaviour
{
    private void OnEnable()
    {
        OnValueChanged(GetComponent<TMP_InputField>().text);
    }
    public void OnValueChanged(string value)
    {
        AnimationSettingsUI_Core.SavePlayer2Name(value);
    }
}