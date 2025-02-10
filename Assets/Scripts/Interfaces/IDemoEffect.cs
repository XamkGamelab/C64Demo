using System.Collections;

public interface IDemoEffect
{
    public DemoEffectBase Init();
    public IEnumerator Run(System.Action callbackEnd);    
    public void End(System.Action callbackEnd);
    public void DoUpdate();

}