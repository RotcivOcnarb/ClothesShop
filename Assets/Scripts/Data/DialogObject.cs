using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialog Object", menuName = "Dialog")]
public class DialogObject : ScriptableObject {

    public SingleDialog dialog;

    [System.Serializable]
    public class SingleDialog {
        public string dialogId;
        public SpeechLanguageMap[] speechList;

        //Editor
        public int toolbarIndex;
        public bool toggled;
        public string langToAdd;

        public SpeechLanguageMap GetSpeech(string language) {
            for (int i = 0; i < speechList.Length; i++) {
                if (speechList[i].language == language)
                    return speechList[i];
            }

            return null;
        }
    }


    [System.Serializable]
    public class SingleSpeech {
        [TextArea]
        public string speech;
        public Sprite spriteIcon;
    }

    [System.Serializable]
    public class SpeechLanguageMap {
        public string language;
        public List<SingleSpeech> speeches;
    }
}
