using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibFXSetToggle : MonoBehaviour
{
    public void ToggleClick(bool isOn)
    {
        if (isOn) DataManager.Instance.Settings.UpdateVibFXSetting(true);
        else      DataManager.Instance.Settings.UpdateVibFXSetting(false);
    }
}
