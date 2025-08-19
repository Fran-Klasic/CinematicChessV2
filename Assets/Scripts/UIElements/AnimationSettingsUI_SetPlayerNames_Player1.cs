using TMPro;
using UnityEngine;

public class AnimationSettingsUI_SetPlayerNames_Player1 : MonoBehaviour
{
    private void OnEnable()
    {
        OnValueChanged(GetComponent<TMP_InputField>().text);
    }
    public void OnValueChanged(string value)
    {
        AnimationSettingsUI_Core.SavePlayer1Name(value);
    }
}