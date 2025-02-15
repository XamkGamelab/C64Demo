using UnityEngine;

public class TextEffectSettings
{
    public enum TextEffectType { Explode, SinCurve}
    public TextEffectType EffectType { get; set; } = TextEffectType.SinCurve;
    public Vector3 ExplosionPoint { get; set; } = Vector3.zero;
    public float ExplosionSpeed { get; set; } = 50f;
    public float SinCurveSpeed { get; set; } = 2f;
    public float SinCurveMagnitude { get; set; } = 10f;

    public float SinCurveScale { get; set; } = 0.05f;
}
