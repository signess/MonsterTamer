using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimator
{
    private SpriteRenderer spriteRenderer;
    private List<Sprite> frames;
    public List<Sprite> Frames { get => frames; }
    private float frameRate;

    private int currentFrame;
    private float timer;

    public SpriteAnimator(List<Sprite> frames, SpriteRenderer spriteRenderer, float frameRate = 0.16f)
    {
        this.frames = frames;
        this.spriteRenderer = spriteRenderer;
        this.frameRate = frameRate;
    }

    public void Start()
    {
        currentFrame = 0;
        timer = 0f;
        spriteRenderer.sprite = frames[0];
    }

    public void HandleUpdate()
    {
        timer += Time.deltaTime;
        if (timer > frameRate)
        {
            currentFrame = (currentFrame + 1) % frames.Count;
            spriteRenderer.sprite = frames[currentFrame];
            timer -= frameRate;
        }
    }
}
