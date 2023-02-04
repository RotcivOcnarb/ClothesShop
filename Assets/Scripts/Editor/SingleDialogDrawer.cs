using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DialogObject.SingleDialog))]
public class SingleDialogDrawer : PropertyDrawer {

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        SerializedProperty dialogId = property.FindPropertyRelative("dialogId");
        SerializedProperty toolbarIndex = property.FindPropertyRelative("toolbarIndex");
        SerializedProperty toggled = property.FindPropertyRelative("toggled");
        SerializedProperty langToAdd = property.FindPropertyRelative("langToAdd");
        SerializedProperty speechList = property.FindPropertyRelative("speechList");

        List<string> languages = SerializedStringArray(speechList);
        EditorGUI.BeginProperty(position, label, property);

        Rect foldoutRect = position;
        foldoutRect.height = 20;
        //toggled.boolValue = EditorGUI.Foldout(foldoutRect, toggled.boolValue, dialogId.stringValue);

        //if (toggled.boolValue) {
            //ID do texto
            Rect idRect = position;
            idRect.width *= 1/4f;
            idRect.y += 20;
            idRect.height = 20;
            //dialogId.stringValue = 
            EditorGUI.LabelField(idRect, "Linguagem:");

            //Add/remove linguagens
            Rect langText = position;
            langText.width *= 1/2f;
            langText.height = 20;
            langText.x += idRect.width + 5;
            langText.width -= 10;
            langText.y += 20;
            langToAdd.stringValue = EditorGUI.TextField(langText, langToAdd.stringValue);

            Rect addRem = position;
            addRem.width /= 4;
            addRem.height = 20;
            addRem.x += idRect.width + langText.width + 10;
            addRem.y += 20;
            string btnLbl = "Add";
            int idx = StringArrayIndexOf(languages, langToAdd.stringValue);
            if (idx != -1) {
                btnLbl = "Remove";
            }
            if (GUI.Button(addRem, btnLbl)) {
                if (idx != -1) {
                    speechList.DeleteArrayElementAtIndex(idx);
                    toolbarIndex.intValue = 0;
                }
                else {
                    speechList.InsertArrayElementAtIndex(speechList.arraySize);
                    speechList.GetArrayElementAtIndex(speechList.arraySize - 1).FindPropertyRelative("language").stringValue = langToAdd.stringValue;
                }
            }

            //Toolbar de linguagens
            Rect toolbarRect = position;
            toolbarRect.y += 45;
            toolbarRect.height = 20;
            toolbarIndex.intValue = GUI.Toolbar(toolbarRect, toolbarIndex.intValue, languages.ToArray());

            //Lista de textos
            if (speechList.arraySize > 0) {
                SerializedProperty currSpeeches = speechList.GetArrayElementAtIndex(toolbarIndex.intValue).FindPropertyRelative("speeches");

                Rect areasRect = position;
                areasRect.y += 85;
                areasRect.height = EditorGUI.GetPropertyHeight(currSpeeches);

                EditorGUI.PropertyField(areasRect, currSpeeches, true);
            }

        //}

        EditorGUI.EndProperty();


    }

    public int StringArrayIndexOf(List<string> array, string value) {
        for (int i = 0; i < array.Count; i++) {
            if (array[i] == value) return i;
        }
        return -1;
    }

    public List<string> SerializedStringArray(SerializedProperty property) {
        List<string> output = new List<string>();

        for (int i = 0; i < property.arraySize; i++) {
            output.Add(property.GetArrayElementAtIndex(i).FindPropertyRelative("language").stringValue);
        }

        return output;
    }

    public void UpdateSerializedStringArray(SerializedProperty property, List<string> newValue) {
        property.ClearArray();
        for (int i = 0; i < newValue.Count; i++) {
            property.InsertArrayElementAtIndex(i);
            property.GetArrayElementAtIndex(i).stringValue = newValue[i];
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        float height = 0;

        height += 20; //String id;

        SerializedProperty toggled = property.FindPropertyRelative("toggled");
        SerializedProperty speechList = property.FindPropertyRelative("speechList");
        SerializedProperty toolbarIndex = property.FindPropertyRelative("toolbarIndex");


        if (toggled.boolValue) {
            height += 5;
            height += 20; //toolbar;
            height += 5;
            height += 20;
            height += 20;
            if (speechList.arraySize > 0) {
                SerializedProperty currSpeeches = speechList.GetArrayElementAtIndex(toolbarIndex.intValue).FindPropertyRelative("speeches");
                height += EditorGUI.GetPropertyHeight(currSpeeches);

            }

        }

        //Textos

        //return height;
        return 1000;
    }
}
[CustomPropertyDrawer(typeof(DialogObject.SingleSpeech))]
public class SingleSpeechDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        SerializedProperty speech = property.FindPropertyRelative("speech");
        SerializedProperty spriteIcon = property.FindPropertyRelative("spriteIcon");

        Rect r = position;
        r.height = EditorGUI.GetPropertyHeight(speech);
        speech.stringValue = EditorGUI.TextArea(r, speech.stringValue);

        r.y += r.height + 5;
        r.height = EditorGUI.GetPropertyHeight(spriteIcon);
        EditorGUI.PropertyField(r, spriteIcon);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        SerializedProperty speech = property.FindPropertyRelative("speech");
        SerializedProperty spriteIcon = property.FindPropertyRelative("spriteIcon");

        return EditorGUI.GetPropertyHeight(speech) +
            EditorGUI.GetPropertyHeight(spriteIcon) +
            20;
    }
}