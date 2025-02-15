using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;
using System;

public static class TextFunctions
{
    //verts[charInfo.vertexIndex + j] = orig + new Vector3(0, Mathf.Sin(Time.time * 2f + orig.x * 0.01f) * 10f, 0);
    public static void TextMeshEffect(TMP_Text textComponent, float startTime, TextEffectSettings textEffectSettings /*Vector3 explosionPoint, float explosionSpeed*/)
    {
        textComponent.ForceMeshUpdate();
        var textInfo = textComponent.textInfo;

        for (int i = 0; i < textInfo.characterCount; ++i)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            if (!charInfo.isVisible)            
                continue;
            
            Vector3[] verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

            for (int j = 0; j < 4; ++j)
            {
                Vector3 orig = verts[charInfo.vertexIndex + j];
                //Switch case text function

                switch (textEffectSettings.EffectType)
                {
                    case TextEffectSettings.TextEffectType.Explode:
                        // code block
                        verts[charInfo.vertexIndex + j] += (orig - textEffectSettings.ExplosionPoint).normalized * (Time.time - startTime) * textEffectSettings.ExplosionSpeed;
                        break;
                    case TextEffectSettings.TextEffectType.SinCurve:
                        // code block
                        verts[charInfo.vertexIndex + j] = orig + new Vector3(0, Mathf.Sin(Time.time * textEffectSettings.SinCurveSpeed + orig.x * textEffectSettings.SinCurveScale) * textEffectSettings.SinCurveMagnitude, 0);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("Not an defined effect type.");                        
                }
            }
        }

        UpdateTextMeshGeom(textComponent, textInfo);        
    }

    public static void UpdateTextMeshGeom(TMP_Text textComp, TMP_TextInfo textInfo)
    {
        for (int i = 0; i < textInfo.meshInfo.Length; ++i)
        {
            TMP_MeshInfo meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = textInfo.meshInfo[i].vertices;
            textComp.UpdateGeometry(meshInfo.mesh, i);
        }
    }        

    public static TMP_Text AddTextMeshProTextComponent(RectTransform addTextTo, string fontAssetName, int fontSize, Color color)
    {
        TMP_Text txt = addTextTo.AddComponent<TextMeshProUGUI>();
        txt.font = GameObject.Instantiate<TMP_FontAsset>(Resources.Load<TMP_FontAsset>(fontAssetName));
        txt.fontSize = fontSize;
        txt.color = color;
        return txt;
    }

}
