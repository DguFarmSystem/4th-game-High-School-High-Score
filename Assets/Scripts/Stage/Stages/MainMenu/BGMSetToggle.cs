using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMSetToggle : MonoBehaviour
{
    public void ToggleClick(bool isOn)
    {
        if (isOn) DataManager.Instance.Settings.UpdateBGMSetting(true);
        else      DataManager.Instance.Settings.UpdateBGMSetting(false);
    }
}
