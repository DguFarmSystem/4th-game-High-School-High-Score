using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptSpeedSetToggle : MonoBehaviour
{
    public void ScriptSpeedSetSlow(bool isOn)
    {
        if (isOn) DataManager.Instance.Player.UpdateScriptSpeed(ScriptSpeedState.Slow);
    }

    public void ScriptSpeedSetNormal(bool isOn)
    {
        if (isOn) DataManager.Instance.Player.UpdateScriptSpeed(ScriptSpeedState.Normal);
    }

    public void ScriptSpeedSetFast(bool isOn)
    {
        if (isOn) DataManager.Instance.Player.UpdateScriptSpeed(ScriptSpeedState.Fast);
    }
}
