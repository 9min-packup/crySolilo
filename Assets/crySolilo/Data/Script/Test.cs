using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CrySolilo;

public class Test : MonoBehaviour
{
    public string bgKey, charaKey, charaFaceKey, fontkey, bgmKey, bgsKey, seKey;
    public Vector2 charaPos, txtboxsizeDelta, txtboxposition, txtSizeDelta;

    [ContextMenu("Test Show BG")]
    public void ShowBG()
    {
        CRY_SOLILO.System.uiManager.ShowBg(bgKey);
    }

    [ContextMenu("Test Show Chara")]
    public void ShowChara()
    {
        CRY_SOLILO.System.uiManager.ShowCaracter(charaKey, charaFaceKey, charaPos);
    }

    [ContextMenu("Test Clear BG")]
    public void ClearBG()
    {
        CRY_SOLILO.System.uiManager.ClearBG();
    }

    [ContextMenu("Test Hide Chara")]
    public void HideChara()
    {
        CRY_SOLILO.System.uiManager.HideCharacter(charaKey);
    }

    [ContextMenu("Test Show TextBox")]
    public void ShowTextBox()
    {
        CRY_SOLILO.System.uiManager.ShowTextBox();
    }
    [ContextMenu("Test Hide TextBox")]
    public void HideTextBox()
    {
        CRY_SOLILO.System.uiManager.HideTextBox();
    }

    [ContextMenu("Test BGM ")]
    public void PlayBgm()
    {
        CRY_SOLILO.System.audioManager.PlayBGM(bgmKey, true, 1.0f, 1.0f, 1.0f, 0.2f, 1.0f);
    }

    [ContextMenu("Test BGS ")]
    public void PlayBgs()
    {
        CRY_SOLILO.System.audioManager.PlayBGS(bgsKey, true, 1.0f, 1.0f, 1.0f, 0.2f, 1.0f);
    }

    [ContextMenu("Test SE ")]
    public void PlaySe()
    {
        CRY_SOLILO.System.audioManager.PlaySE(seKey, false, 1.0f, 1.0f);
    }

}
