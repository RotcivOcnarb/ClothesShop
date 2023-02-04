using RotsLib.Popup;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static TextMarkupParser;

public class DialogPopup : PopupWindow {

    public static DialogPopup Instance;

    //References
    public Image characterLeft;
    public GameObject arrowImage;

    public TextMeshProUGUI textBox;

    //Parameters
    public float defaultLetterPerSecond = 15;

    //Internal
    int currentDialogIndex = 0;
    int letters = 0;
    float dialogTimer = -0.5f;
    bool closing;

    TextMarkupParser parser;
    TaggedText taggedText;
    List<DialogObject.SingleSpeech> speeches;

    protected override void Awake() {
        base.Awake();
        Instance = this;
    }

    public void OpenDialog(DialogObject dialogObject) {
        this.speeches = dialogObject.dialog.GetSpeech("en-us").speeches;
        parser = new TextMarkupParser();
        taggedText = parser.Parse(speeches[0].speech);

        characterLeft.gameObject.SetActive(true);
        characterLeft.sprite = speeches[0].spriteIcon;

    }

    public void CopyTransform(RectTransform from, RectTransform to) {

        to.pivot = from.pivot;
        to.anchorMin = from.anchorMin;
        to.anchorMax = from.anchorMax;
        to.offsetMin = from.offsetMin;
        to.offsetMax = from.offsetMax;

    }

    private void Update() {
        if(isOpen)
            dialogTimer += Time.unscaledDeltaTime;
        if (taggedText == null) return;

        string text = taggedText.GetText();
        float lettersPerSecond = defaultLetterPerSecond;
        if (letters < text.Length) {
            TagData lpsOverride = taggedText.GetTagFromIndex(letters, "lps");
            if (lpsOverride != null) {
                lettersPerSecond = float.Parse(lpsOverride.GetValue("value", defaultLetterPerSecond.ToString()));
            }
        }

        if (dialogTimer > 1 / lettersPerSecond) {
            dialogTimer -= 1 / lettersPerSecond;
            letters++;
            if (letters < text.Length) {

                TagData waitTime = taggedText.GetTagFromIndex(letters, "wait");
                if (waitTime != null) {
                    float wait = float.Parse(waitTime.GetValue("value", "0"));
                    if (wait > 0) {
                        dialogTimer = -wait;
                    }
                }
            }
        }

        letters = Mathf.Min(letters, text.Length);
        textBox.text = GetTaggedSubstring(letters);

        arrowImage.SetActive(letters >= text.Length);
    }

    public void SkipDialog() {
        if (closing) return;
        if (letters >= taggedText.GetText().Length) {
            NextDialog();
        }
        else {
            letters = taggedText.GetText().Length;
        }
    }

    public void NextDialog() {
        if (letters >= taggedText.GetText().Length) {
            dialogTimer = 0;
            currentDialogIndex++;

            if (currentDialogIndex >= speeches.Count) {
                closing = true;
                ClosePopup();
            }
            else {
                taggedText = parser.Parse(speeches[currentDialogIndex].speech);

                if (speeches[currentDialogIndex].spriteIcon != null) {
                    characterLeft.gameObject.SetActive(true);
                    characterLeft.sprite = speeches[currentDialogIndex].spriteIcon;
                }

            }
            letters = 0;
        }
    }


    public string GetTaggedSubstring(int length) {
        string output = "";
        bool startedWave = false;
        bool startedWiggle = false;
        bool startedColor = false;

        for (int i = 0; i < Mathf.Min(length, taggedText.GetText().Length); i++) {

            //Wave effect
            if (taggedText.GetTagFromIndex(i, "wave") != null) {
                if (!startedWave) {
                    startedWave = true;
                    output += "<font=\"Bebas SDF\" material=\"Bebas Wave\">";
                }
            }
            else {
                if (startedWave) {
                    startedWave = false;
                    output += "</font>";
                }
            }

            //Wiggle effect
            if (taggedText.GetTagFromIndex(i, "wiggle") != null) {
                if (!startedWiggle) {
                    startedWiggle = true;
                    output += "<font=\"Bebas SDF\" material=\"Bebas Wiggle\">";
                }
            }
            else {
                if (startedWiggle) {
                    startedWiggle = false;
                    output += "</font>";
                }
            }

            //Color Effect
            if (taggedText.GetTagFromIndex(i, "color") != null) {
                if (!startedColor) {
                    startedColor = true;
                    output += "<color=" + taggedText.GetTagFromIndex(i, "color").GetValue("value", "black") + ">";
                }
            }
            else {
                if (startedColor) {
                    startedColor = false;
                    output += "</color>";
                }
            }

            output += taggedText.GetText().Substring(i, 1);
        }

        if (startedWave) {
            output += "</font>";
        }
        if (startedWiggle) {
            output += "</font>";
        }
        if (startedColor) {
            output += "</color>";
        }

        return output;
    }

}
