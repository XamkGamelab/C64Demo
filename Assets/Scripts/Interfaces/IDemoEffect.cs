using System.Collections;

public interface IDemoEffect
{
    public DemoEffectBase Init(float parTime, string tutorialText);
    public IEnumerator Run(System.Action endDemoCallback);    
    public void End(bool dispose);
    public void DoUpdate();
}