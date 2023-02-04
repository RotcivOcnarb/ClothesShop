using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Linq;

public class TextMarkupParser
{
    enum TokenType { 
        LEFT_DEL,
        RIGHT_DEL,
        TEXT,
    }
    struct Token {
        public string value;
        public TokenType type;
    }

    public class TagData {
        public string name;
        public Dictionary<string, string> pms;
        public string GetValue(string key, string defaultValue)
        {
            if (!pms.ContainsKey(key)) return defaultValue;
            return pms[key];
        }
    }

    public struct TaggedWord
    {
        public string text;
        public List<TagData> data;
    }

    public class TaggedText
    {
        public List<TaggedWord> words;
        public string GetText() {
            string tst = "";
            for(int i = 0; i < words.Count; i++)
            {
                tst += words[i].text;
            }
            return tst;
        }
        public TaggedText(List<TaggedWord> words)
        {
            this.words = words;
        }
        public List<TagData> GetDataFromIndex(int charIndex)
        {
            int acc = 0;
            int cont = 0;
            while(acc <= charIndex)
            {
                acc += words[cont].text.Length;
                cont++;
            }
            cont--;
            return words[cont].data;
        }

        public TagData GetTag(List<TagData> tagData, string name)
        {
            for(int i = 0; i < tagData.Count; i++)
                if(tagData[i].name == name)
                    return tagData[i];
            return null;
        }

        public TagData GetTagFromIndex(int charIndex, string name)
        {
            return GetTag(GetDataFromIndex(charIndex), name);
        }
    }

    public TaggedText Parse(string text) {
        return new TaggedText(ValidateTokens(Tokenize(text)));
    }

    string DelimiterName(string delimiter) { 
        if(delimiter[1] == '/')
        { //é de fechar
            string name = delimiter.Substring(2);
            name = name.Substring(0, name.Length - 1);
            name = name.Split(' ')[0];
            return name;
        }
        else
        {
            string name = delimiter.Substring(1);
            name = name.Substring(0, name.Length - 1);
            name = name.Split(' ')[0];
            return name;
        }
    }

    TagData GetTagData(string tag) {
        string name = DelimiterName(tag);

        Regex paramRegex = new Regex(@"[^\s\<]+=[^\s\>]+");
        List<string> pms = MatchToArray(paramRegex.Matches(tag));

        Dictionary<string, string> keys = new Dictionary<string, string>();

        for(int i = 0; i < pms.Count; i++) {
            keys.Add(pms[i].Split('=')[0], pms[i].Split('=')[1]);
        }
        return new TagData() { name = name, pms = keys};
    }

    List<TaggedWord> ValidateTokens(List<Token> tokens)
    {
        List<TaggedWord> taggedTexts = new List<TaggedWord>();
        Stack<Token> stack = new Stack<Token>();

        for(int i = 0; i < tokens.Count; i++)
        {
            if(tokens[i].type == TokenType.LEFT_DEL)
            {
                stack.Push(tokens[i]);
            }
            else if(tokens[i].type == TokenType.RIGHT_DEL)
            {
                if (stack.Count == 0) throw new System.Exception("Closing tag without opening"); //se eu to tentando fechar algo com o stack vazio, ERRO
                if (DelimiterName(stack.Peek().value) != DelimiterName(tokens[i].value)) throw new System.Exception("Closing tag in wrong order"); //se o q eu to tentando fechar não tá no topo do stack, ERRO
                stack.Pop();
            }
            else if(tokens[i].type == TokenType.TEXT)
            {
                List<TagData> tagDatas = new List<TagData>();
                foreach (Token t in stack) 
                    tagDatas.Add(GetTagData(t.value));

                taggedTexts.Add(new TaggedWord() { text = tokens[i].value, data = tagDatas });
            }
        }
        if (stack.Count > 0) throw new System.Exception(stack.Count + " tags not closed");
        return taggedTexts;
    }

    List<Token> Tokenize(string text)
    {
        Regex leftDelimiter = new Regex(@"\<[^\/].*?\>");
        Regex rightDelimiter = new Regex(@"\<\/.*?\>");
        Regex invTextDelimiter = new Regex(@"(\<[^\/].*?\>|\<\/.*?\>)");

        List<string> allLefts = MatchToArray(leftDelimiter.Matches(text));
        List<string> allRights = MatchToArray(rightDelimiter.Matches(text));
        List<string> allInvTexts = MatchToArray(invTextDelimiter.Matches(text));
        List<string> allTexts = text.Split(allInvTexts.ToArray(), System.StringSplitOptions.RemoveEmptyEntries).ToList<string>();
        
        if(allLefts.Count == 0 && allRights.Count == 0)
        {
            List<Token> lt = new List<Token>();
            lt.Add(new Token() { type = TokenType.TEXT, value = text });
            return lt;
        }

        List<Token> tokens = new List<Token>();

        string processed = text;
        while(processed.Length > 0)
        {
            if(allLefts.Count > 0 && processed.IndexOf(allLefts[0]) == 0)
            {
                processed = processed.Substring(allLefts[0].Length);
                tokens.Add(new Token() { value = allLefts[0], type = TokenType.LEFT_DEL });
                allLefts.RemoveAt(0);
            }
            else if (allRights.Count > 0 && processed.IndexOf(allRights[0]) == 0)
            {
                processed = processed.Substring(allRights[0].Length);
                tokens.Add(new Token() { value = allRights[0], type = TokenType.RIGHT_DEL });
                allRights.RemoveAt(0);
            }
            else if (allTexts.Count > 0 && processed.IndexOf(allTexts[0]) == 0)
            {
                processed = processed.Substring(allTexts[0].Length);
                tokens.Add(new Token() { value = allTexts[0], type = TokenType.TEXT });
                allTexts.RemoveAt(0);
            }
        }
        return tokens;
    }

    List<string> MatchToArray(MatchCollection match)
    {
        List<string> res = new List<string>();
        for (int i = 0; i < match.Count; i++)
            res.Add(match[i].Value);
        return res;
    }
}
