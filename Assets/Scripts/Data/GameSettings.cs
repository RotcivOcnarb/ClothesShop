using Rotslib.Settings;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using Rotslib.Editor;
#endif

[CustomSettings("_Game Settings", new string[] { }, "GameSettings")]
public class GameSettings : CustomSettingsObject {

#if UNITY_EDITOR

    [Header("Functions")]
    public Texture[] skinTextures;

    [SettingsButton("Create Animation Assets", "Functions")]
    public void CreateAnimationAssets() {

        //Initialize all data from original asset
        string originalTextureGUID =
            AssetDatabase.GUIDFromAssetPath("Assets/Sprites/Character/Skin/White.png").ToString();

        Texture originalTexture =
            AssetDatabase.LoadAssetAtPath<Texture>("Sprites/Character/Skin/White.png");

        RuntimeAnimatorController baseAnimator =
            AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>
            ("Assets/Animations/Character/White/White.controller");

        Sprite[] baseSprites =
            AssetDatabase.LoadAllAssetsAtPath("Assets/Sprites/Character/Skin/White.png")
            .OfType<Sprite>().ToArray();
        baseSprites = baseSprites.OrderBy(s => int.Parse(Regex.Match(s.name, @"([0-9])+").Value)).ToArray();

        string[] baseSpriteGUIDs = baseSprites.Select((sprite) => {
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(sprite, out string guid, out long localid);
            return localid.ToString();
        }
        ).ToArray();

        string[] baseAnimations = baseAnimator.animationClips.Select((a) => a.name.Replace("White_", "") + ".anim").Distinct().ToArray();

        //Starts to create the animation copy
        foreach (Texture skinTexture in skinTextures) {
            if (skinTexture != null) {

                string folderPath = "Animations/Character/" + skinTexture.name;
                Directory.CreateDirectory("Assets/" + folderPath);
                string skinTextureGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(skinTexture));

                //Modify the texture to havesubsprites
                CreateSpriteSheet(skinTexture);

                Sprite[] sprites =
                AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(skinTexture))
                    .OfType<Sprite>().ToArray();
                sprites = sprites.OrderBy(s => int.Parse(Regex.Match(s.name, @"([0-9])+").Value)).ToArray();

                string[] spriteGUIDs = sprites.Select((sprite) => {
                    AssetDatabase.TryGetGUIDAndLocalFileIdentifier(sprite, out string guid, out long localid);
                    return localid.ToString();
                }).ToArray();

                AnimationClip[] newAnimationClips = new AnimationClip[baseAnimations.Length];

                for (int i = 0; i < baseAnimations.Length; i++) {

                    string destinationPath =
                        "Assets/" + folderPath + "/" + skinTexture.name + "_" + baseAnimations[i];
                    //1- Copy the asset file and change the name
                    File.Copy(
                        "Assets/Animations/Character/White/White_" + baseAnimations[i],
                        destinationPath
                        );

                    //2- Navigate over the contents and swap GUID
                    string animationContents = File.ReadAllText(destinationPath);
                    animationContents = animationContents.Replace(originalTextureGUID, skinTextureGUID);
                    animationContents = animationContents.Replace(
                        ("White_" + baseAnimations[i]).Replace(".anim", ""),
                        (skinTexture.name + "_" + baseAnimations[i]).Replace(".anim", "")
                        );
                    for (int j = 0; j < baseSpriteGUIDs.Length; j++) {
                        animationContents = animationContents.Replace(baseSpriteGUIDs[j], spriteGUIDs[j]);
                    }

                    File.WriteAllText(destinationPath, animationContents);
                    AssetDatabase.ImportAsset(destinationPath);

                    newAnimationClips[i] = AssetDatabase.LoadAssetAtPath<AnimationClip>(destinationPath);
                }

                AnimatorOverrideController animator = new AnimatorOverrideController();
                animator.name = skinTexture.name;
                animator.runtimeAnimatorController = baseAnimator;
                List<KeyValuePair<AnimationClip, AnimationClip>> overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
                for (int i = 0; i < baseAnimations.Length; i++) {
                    overrides.Add(new KeyValuePair<AnimationClip, AnimationClip>(
                            baseAnimator.animationClips[i], newAnimationClips[i]
                        ));
                }
                animator.ApplyOverrides(overrides);

                AssetDatabase.CreateAsset(animator, "Assets/" + folderPath + "/" + skinTexture.name + ".overrideController");
                Debug.Log("Created animation for " + skinTexture.name);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("FINISH");

    }
    public void CreateSpriteSheet(Texture texture) {
        TextureImporter imp = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture)) as TextureImporter;
        imp.spriteImportMode = SpriteImportMode.Multiple;
        imp.spritePixelsPerUnit = 32;
        SpriteMetaData[] smd = new SpriteMetaData[12];
        int c = 0;
        for (int j = 3; j >= 0; j--) {
            for (int i = 0; i < 3; i++) {
                if (c == 12) break;
                smd[c] = new SpriteMetaData() {
                    name = texture.name + "_" + c,
                    pivot = new Vector2(0.5f, 0f),
                    rect = new Rect(i * 48, j * 48, 48, 48),
                    alignment = 9
                };
                c++;
            }
        }
        imp.spritesheet = smd;
        imp.SaveAndReimport();
    }

    [SettingsButton("Create Skin SOs", "Functions")]
    public void CreateSkinSOs() {

        string[] allFiles = Directory.GetFiles("Assets/Animations/Character", "*.overrideController", SearchOption.AllDirectories);
        allFiles = allFiles.Select(f => f.Replace("\\", "/")).ToArray();

        RuntimeAnimatorController[] objects = allFiles.Select(f => AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(f)).ToArray();

        Dictionary<string, RuntimeAnimatorController> allAnimators = new Dictionary<string, RuntimeAnimatorController>();
        foreach(RuntimeAnimatorController animator in objects) {
            allAnimators.Add(animator.name, animator);
        }

        foreach (Texture skinTexture in skinTextures) {
            Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(
                AssetDatabase.GetAssetPath(skinTexture))
                .OfType<Sprite>().ToArray();
            sprites = sprites.OrderBy(s => int.Parse(Regex.Match(s.name, @"([0-9])+").Value)).ToArray();


            SkinPiece so = ScriptableObject.CreateInstance(typeof(SkinPiece)) as SkinPiece;
            so.thumbnailIcon = sprites[1];
            so.skinAnimator = allAnimators[skinTexture.name];
            so.displayName = skinTexture.name;

            AssetDatabase.CreateAsset(so, "Assets/Data/" + skinTexture.name + ".asset");
            Debug.Log("Created asset " + skinTexture.name);
        }

        AssetDatabase.SaveAssets();
        Debug.Log("Finish");
    }

#endif

    public static GameSettings GetDefaultSettings() {
        return GetDefaultSettings<GameSettings>();
    }

#if UNITY_EDITOR
    [SettingsProvider]
    public static SettingsProvider CreateCustomSettingsProvider() {
        return CustomSettingsIMGUIRegister.CreateCustomSettingsProvider(typeof(GameSettings));
    }
#endif

}
