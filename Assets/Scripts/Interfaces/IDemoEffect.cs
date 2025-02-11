using System.Collections;

public interface IDemoEffect
{
    public DemoEffectBase Init();
    public IEnumerator Run(System.Action endDemoCallback);    
    public void End(bool dispose);
    public void DoUpdate();
}