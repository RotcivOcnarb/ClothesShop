using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleText : MonoBehaviour
{
    [TextArea]
    public string text;
    public GameObject letterPrefab;
    public Rect area;

    public float characterSpacing = 1;
    public float lineSpacing = 1;

    TextMarkupParser.TaggedText taggedText;


    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(area.center, area.size);
    }

    // Start is called before the first frame update
    void Start()
    {
        TextMarkupParser parser = new TextMarkupParser();
        taggedText = parser.Parse(text);

        float x = 0;
        float y = 0;

        Vector3 rectOrigin = new Vector3(
                area.xMin,
                area.yMax,
                0
            );

        for(int c = 0; c < taggedText.GetText().Length; c++) {
            char ch = taggedText.GetText().ToCharArray()[c];
            
            GameObject lt = Instantiate(letterPrefab, rectOrigin + new Vector3(x, y, 0), Quaternion.identity);
            SingleLetter sl = lt.GetComponent<SingleLetter>();

            float sizeFactor = 1;

            foreach (TextMarkupParser.TagData td in taggedText.GetDataFromIndex(c)) { 
                if(td.name == "size") {
                    sl.size = float.Parse(td.GetValue("value", "1"));
                    sizeFactor = sl.size;
                }
                else if(td.name == "color") {
                    if(ColorUtility.TryParseHtmlString(td.GetValue("value", "white"), out Color color)) {
                        sl.color = color;
                    }
                }
                else if(td.name == "wobble") {
                    sl.wobble = true;
                }
                else if(td.name == "wave") {
                    sl.wave = true;
                }
            }

            TextMesh tm = lt.GetComponent<TextMesh>();
            tm.text = ch.ToString();

            tm.font.GetCharacterInfo(ch, out CharacterInfo characterInfo, tm.fontSize);
            x += characterInfo.advance * characterSpacing * sizeFactor;

            if(x > area.width) {
                x = 0;
                y -= lineSpacing;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
