using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMSetToggle : MonoBehaviour
{
    public void ToggleClick(bool isOn)
    {
        if (isOn) DataManager.Instance.Player.UpdateBGMSetting(true);
        else      DataManager.Instance.Player.UpdateBGMSetting(false);
    }
}
