using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CrySolilo;

public class Test : MonoBehaviour
{
    public string bgKey, charaKey, charaFaceKey, fontkey, bgmKey, bgsKey, seKey;
    public Vector2 charaPos, txtboxsizeDelta, txtboxposition, txtSizeDelta;
    public int seIndex;
    public string scenarioKey;
    public string tagString;
    public string textTagString;
    public string nameTagString;
    public string jumpTarget;
    [Space]
    public string buttonKey;
    public Vector2 buttonPosition;
    public Vector2 buttonSize;
    public Color buttonfontColor;
    public int buttonFontSize;
    public bool buttonNativeSize = false;
    public string buttonText;
    public string buttonEnterKey;

    [TextArea]
    public string text;

    [ContextMenu("Show BG")]
    public void ShowBG()
    {
        CRY_SOLILO.System.uiManager.ShowBg(bgKey);
    }

    [ContextMenu("Show Chara")]
    public void ShowChara()
    {
        CRY_SOLILO.System.uiManager.ShowCaracter(charaKey, charaFaceKey, charaPos);
    }

    [ContextMenu("Clear BG")]
    public void ClearBG()
    {
        CRY_SOLILO.System.uiManager.ClearBG();
    }

    [ContextMenu("Hide Chara")]
    public void HideChara()
    {
        CRY_SOLILO.System.uiManager.HideCharacter(charaKey);
    }

    [ContextMenu("Show TextBox")]
    public void ShowTextBox()
    {
        CRY_SOLILO.System.uiManager.ShowTextBox();
    }

    [ContextMenu("Show Text")]
    public void ShowText()
    {
        CRY_SOLILO.System.uiManager.ShowText(text);
    }

    [ContextMenu("Show Text(add)")]
    public void ShowTextAdd()
    {
        CRY_SOLILO.System.uiManager.ShowText(text, true);
    }

    [ContextMenu("Skip Text")]
    public void SkipText()
    {
        CRY_SOLILO.System.uiManager.SkipText();
    }


    [ContextMenu("Hide TextBox")]
    public void HideTextBox()
    {
        CRY_SOLILO.System.uiManager.HideTextBox();
    }

    [ContextMenu("Play BGM ")]
    public void PlayBgm()
    {
        CRY_SOLILO.System.audioManager.PlayBGM(bgmKey, true, 1.0f, 1.0f, 1.0f, 0.2f, 1.0f);
    }

    [ContextMenu("Stop BGM ")]
    public void StopBgm()
    {
        CRY_SOLILO.System.audioManager.StopBGM(1.0f);
    }

    [ContextMenu("Play BGS ")]
    public void PlayBgs()
    {
        CRY_SOLILO.System.audioManager.PlayBGS(bgsKey, true, 1.0f, 1.0f, 1.0f, 0.2f, 1.0f);
    }

    [ContextMenu("Stop BGS ")]
    public void StopBgs()
    {
        CRY_SOLILO.System.audioManager.StopBGS(1.0f);
    }

    [ContextMenu("Play SE ")]
    public void PlaySe()
    {
        CRY_SOLILO.System.audioManager.PlaySE(seKey, seIndex, false, 1.0f, 1.0f);
    }

    [ContextMenu("Stop SE")]
    public void StopSe()
    {
        CRY_SOLILO.System.audioManager.StopSE(seIndex);
    }

    [ContextMenu("Load Scenario")]
    public void LoadScenario()
    {
        CRY_SOLILO.System.scenarioManager.LoadScenario(scenarioKey);
    }

    [ContextMenu("Create Tag")]
    public void CreateTag()
    {
        Tag tag = Tag.CreateTag(tagString);

        Debug.Log(tag.ToString());
    }

    [ContextMenu("Create Text Tag")]
    public void CreateTextTag()
    {
        Tag tag = Tag.CreateTextTag(textTagString);

        Debug.Log(tag.ToString());
    }

    [ContextMenu("Create Name Tag")]
    public void CreateNameTag()
    {
        Tag tag = Tag.CreateNameTag(nameTagString);
        Debug.Log(tag.ToString());
    }

    [ContextMenu("Execute Scenario")]
    public void ExecuteScenario()
    {
        CRY_SOLILO.System.scenarioManager.ExecuteScenario();
    }

    [ContextMenu("Jump Scenario")]
    public void Jump()
    {
        CRY_SOLILO.System.scenarioManager.Jump(jumpTarget);
    }

    [ContextMenu("Create Button")]
    public void CreateButton()
    {
        Button.ButtonClickedEvent e = new Button.ButtonClickedEvent();
        e.AddListener(() => { Debug.Log("おはよう！"); });

        CRY_SOLILO.System.uiManager.CreateButton(buttonKey, buttonPosition, buttonSize, buttonfontColor, buttonFontSize, buttonNativeSize, buttonText, buttonEnterKey, e);
    }

}
