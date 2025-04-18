/**
 *  Since AnimatorController/Animation workflow is tedious and extremely error prone, it's
 *  easier to handle simple sprite animations with a simple animation class.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

[Serializable]
[RequireComponent(typeof(SpriteRenderer))]
public class SimpleSpriteAnimator : MonoBehaviour
{
    [Tooltip("List of sprites (animation keyframes) to use in the animation")]
    public List<Sprite> Sprites = new List<Sprite>();
    [Tooltip("Delay per frame")]
    public float AnimationFrameDelay = .1f;
    [Tooltip("Start from frame")]
    public int StartFromFrame = 0;
    [Tooltip("Set delay before animation starts")]
    public float SetStartDelay = 0f;
    [Tooltip("Random delay before animation starts (plus set delay)")]
    public float RandomStartDelay = 0f;
    [Tooltip("Set delay after every loop")]
    public float SetLoopDelay = 0f;
    [Tooltip("Random delay after every loop (plus set delay)")]
    public float RandomLoopDelay = 0f;
    [Tooltip("Play frames in random order")]
    public bool PlayRandomFrames = false;
    [Tooltip("Play animation reversed")]
    public bool Reversed = false;
    [Tooltip("Disable GameObject after the last loop")]
    public bool DisableAfterLoops = false;
    [Tooltip("Destroy GameObject after the last loop")]
    public bool DestroyAfterLoops = false;
    [Tooltip("Stop animation to last frame after the last loop")]
    public bool StopToLastFrame = false;
    [Tooltip("Start the animation only after this sprite is visible to camera")]
    public bool StartWhenInCamViewport = false;
    [Tooltip("Destroy the animation object when it's no more visible to camera")]
    public bool DestroyWhenLeavesCamViewport = false;
    [Tooltip("Don't start playing animation automatically")]
    public bool DontAutoPlay = false;
    [Tooltip("Dont' just animation on disable")]
    public bool IgnoreDisable = false;

    [Tooltip("Amount of loops to play. Zero = infinite")]
    public int Loops = 0;

    public bool IsAnimating { get; private set; } = false;
    public SpriteRenderer spriteRenderer => GetComponent<SpriteRenderer>();

    public ReactiveProperty<Sprite> CurrentSprite = new ReactiveProperty<Sprite>();
    private int totalFrames = 0;
    private int currentFrame = 0;
    
    private Image image;
    private int loopsPlayed;    
    private Dictionary<string, Sprite> currentRenderSpriteSheet;        //Current sprite sheet to render animation with

    //Values that need to be reset to initial values on Reset
    
    private bool initPlayRandomFrames;
    private bool initReversed;
    private bool initDisableAfterLoops;
    private bool initDestroyAfterLoops;
    private bool initStopToLastFrame;
    private bool initStartWhenInCamViewport;
    private bool initDontAutoPlay;    
    private int initLoops;
    
    private Action completedCallBack;

    private bool hasStarted = false;
    

    private void Start()
    {
        //set init values for reset
        
        initPlayRandomFrames = PlayRandomFrames;
        initReversed = Reversed;
        initDisableAfterLoops = DisableAfterLoops;
        initDestroyAfterLoops = DestroyAfterLoops;
        initStopToLastFrame = StopToLastFrame;
        initStartWhenInCamViewport = StartWhenInCamViewport;
        initDontAutoPlay = DontAutoPlay;        
        initLoops = Loops;

        //if GameObject has UI Image component, update image sprite:
        image = GetComponent<Image>();
        
        totalFrames = Sprites.Count;

        if (Reversed)        
            Sprites.Reverse(); //<-- reverse list, reverse animation            
        
        hasStarted = true;
    }

    public void ToggleReverse()
    {
        Reversed = !Reversed;
        Sprites.Reverse(); //<-- reverse list, reverse animation        
    }

    private void OnEnable()
    {
        Initialize();
    }

    private void OnDisable()
    {
        if (IgnoreDisable)
            return;

        Play(false);
    }

    private void Initialize()
    {
        //Start coroutine in OnEnable, so that coroutine resets for PlatformerObjects that are disabled (e.g. Coin)
        if (!StartWhenInCamViewport && !DontAutoPlay)        
            Play(true);
        
        loopsPlayed = 0;
        currentFrame = StartFromFrame;
    }

    private void LateUpdate()
    {
        if (StartWhenInCamViewport && !IsAnimating && CameraFunctions.IsPointWithinViewport(Camera.main, Camera.main.transform.position, transform.position))
            Play(true);

        if (DestroyWhenLeavesCamViewport && !CameraFunctions.IsRendererBoundsWithinViewport(Camera.main, Camera.main.transform.position, GetComponent<SpriteRenderer>().bounds))
            Destroy(gameObject);

        //Render sprite from current sprite sheet:
        if (currentRenderSpriteSheet != null)
        {            
            if (image == null)
                spriteRenderer.sprite = currentRenderSpriteSheet[spriteRenderer.sprite.name];
            else
                image.sprite = currentRenderSpriteSheet[spriteRenderer.sprite.name];   
        }
    }

    /**
    * Set new amount of loops to play. You can use this on runtime to stop continuous animation at some point.
    * @param _loops New amount of loops to play.
    * @param _resetToStartFrame Reset animation to StartFromFrame.
    */
    public void SetLoops(int _loops, bool _resetToStartFrame)
    {
        if (_resetToStartFrame)
        {
            currentFrame = 0;
            SetCurrentSprite();
        }
        loopsPlayed = 0;
        Loops = _loops;

        Play(true);
    }

    /**
    * Start or stop playing animation.
    * @param _play Play or stop the animation.
    */
    public void Play(bool _play, int _startFromFrame = 0, bool _resetLoops = false)
    {
        IsAnimating = _play;
        currentFrame = _startFromFrame;
        
        if (_resetLoops)
            loopsPlayed = 0;

        if (IsAnimating && gameObject.activeInHierarchy)
        {
            StopAllCoroutines();
            StartCoroutine(Animation());
        }
        else
            StopAllCoroutines();
    }

    /**
    * Start or stop playing animation and set if started, set completedCallBack.
    * @param _play Play or stop the animation.
    * @param _completedCallback Action to call after animation is finished.
    */
    public void Play(bool _play, Action _completedCallback, int _startFromFrame = 0, bool _resetLoops = false)
    {
        //If externally started (true) with Play, set completedCallBack value:
        if (_play)
        {
            completedCallBack = _completedCallback;            
        }
        else
            completedCallBack = null;

        Play(_play, _startFromFrame, _resetLoops);
    }

    /**
    * Switch sprite sheet in runtime. Continue rendering the animation with changed sprite sheet.
    * @param _newSpriteSheet Character to kill.
    */
    public void SwitchRuntimeSpriteSheet(Dictionary<string, Sprite> _newSpriteSheet)
    {
        currentRenderSpriteSheet = _newSpriteSheet;
    }

    IEnumerator Animation()
    {
        //Set delay in start
        yield return new WaitForSeconds(SetStartDelay);
        //Random delay in start
        yield return new WaitForSeconds(UnityEngine.Random.Range(0, RandomStartDelay));

        while (Loops == 0 || loopsPlayed < Loops)
        {
            totalFrames = Sprites.Count;

            if (PlayRandomFrames)
            {
                currentFrame = UnityEngine.Random.Range(0, totalFrames);
            }
            else
            {
                if (currentFrame < totalFrames - 1)
                {
                    currentFrame++;
                }
                else
                {
                    loopsPlayed++;
                    if (gameObject.name == "Blood_big_1")
                        Debug.Log(gameObject.name + " loops: " + loopsPlayed);

                    if (loopsPlayed < Loops || (!StopToLastFrame && !DestroyAfterLoops && !DisableAfterLoops))
                        currentFrame = 0;
                }
            }

            SetCurrentSprite();

            if (currentFrame == 0)
            {
                //Set delay after every loop
                yield return new WaitForSeconds(UnityEngine.Random.Range(0, SetLoopDelay));
                //Random delay after every loop
                yield return new WaitForSeconds(UnityEngine.Random.Range(0, RandomLoopDelay));
            }

            if (loopsPlayed == Loops && (StopToLastFrame || DestroyAfterLoops || DisableAfterLoops))
                yield return null;
            else
                yield return new WaitForSeconds(AnimationFrameDelay);
        }

        //Call completed callback Action, if set in Play
        if (completedCallBack != null)
        {
            completedCallBack();            
        }
        
        if (DisableAfterLoops)
            gameObject.SetActive(false);
        else if (DestroyAfterLoops)
            Destroy(gameObject);        
    }

    public void JumpToFrame(int _frameIndex)
    {
        currentFrame = _frameIndex;
        SetCurrentSprite();
    }

    public int GetCurrentFrameIndex()
    {
        return currentFrame;
    }

    private void SetCurrentSprite()
    {
        //Update and notify current sprite
        try
        {
            CurrentSprite.Value = Sprites[currentFrame];
        }
        catch (Exception e)
        {
            Debug.LogError("Excetion: " + e.Message + " | Gameobject " + gameObject.name);
        }

        if (image == null)
        {
            if (spriteRenderer != null)            
                spriteRenderer.sprite = CurrentSprite.Value;            
            else            
                Debug.LogError(gameObject.name + " is missing SpriteRenderer!");                
        }
        else
            image.sprite = CurrentSprite.Value;
    }

    public void ResetState(int setFrame = -1)
    {
        //No need to reset, if object is Destroyed after n loops
        //also don't try to reset objects that have never been enabled:
        if (DestroyAfterLoops || !hasStarted)
            return;

        Play(false);

        PlayRandomFrames = initPlayRandomFrames;
        
        //If reversed in runtime, restore state
        if (Reversed != initReversed)
            ToggleReverse();

        //TODO: obsolete, but check that nothing relies on this bug:
        //Reversed = initReversed;
        DisableAfterLoops = initDisableAfterLoops;
        DestroyAfterLoops = initDestroyAfterLoops;
        StopToLastFrame = initStopToLastFrame;
        StartWhenInCamViewport = initStartWhenInCamViewport;
        DontAutoPlay = initDontAutoPlay;
        Loops = initLoops;

        if (setFrame >= 0)
            JumpToFrame(setFrame);

        Initialize();
    }
}
