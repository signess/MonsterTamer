using System.Collections;
using UnityEngine;
using DG.Tweening;

[ExecuteInEditMode]
public class SimpleBlit : MonoBehaviour
{
    public Material TransitionMaterial;
    public bool transitionIsActive = false;
    public bool fadeIn = true;
    public float transitionRate = 0.01f;

    private float cutoffVal;

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        if (TransitionMaterial != null)
            Graphics.Blit(src, dst, TransitionMaterial);
    }

    private void Update()
    {
        if (transitionIsActive)
        {
            if (fadeIn)
            {
                if (cutoffVal < 1f)
                {
                    cutoffVal += transitionRate * Time.deltaTime;
                }
                else
                {
                    cutoffVal = 1f;
                    transitionIsActive = false;
                }
            }
            else
            {
                if (cutoffVal > 0f)
                {
                    cutoffVal -= transitionRate * Time.deltaTime;
                }
                else
                {
                    cutoffVal = 0f;
                    transitionIsActive = false;
                }
            }
            TransitionMaterial.SetFloat("_Cutoff", cutoffVal);
        }
    }

    public void FadeIn()
    {
        transitionIsActive = true;
        fadeIn = true;
    }

    public void FadeOut()
    {
        transitionIsActive = true;
        fadeIn = false;
    }

    public void SetCutoffToZero()
    {
        cutoffVal = 0f;
        TransitionMaterial.SetFloat("_Cutoff", cutoffVal);
    }
}