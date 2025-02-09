using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;

public static class TextFunctions
{
    public static void ExplodeTextMesh(TMP_Text textComponent, float startTime, Vector3 explosionPoint, float explosionSpeed)
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
                verts[charInfo.vertexIndex + j] += (orig - explosionPoint).normalized * (Time.time - startTime) * explosionSpeed;
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
        txt.font = Object.Instantiate(Resources.Load<TMP_FontAsset>(fontAssetName));
        txt.fontSize = fontSize;
        txt.color = color;
        return txt;
    }
}
