using UnityEngine;
using UnityEngine.UI;

public class SceneTransitionController : MonoBehaviour
{
    public static SceneTransitionController Instance { get; private set; }
    private Image transitionImage;
    public float transitionSpeed = 1f;
    public Texture2D MosaicBattleTransition;
    public Texture2D VerticalFadeTransition;
    public Texture2D SimpleFadeTransition;

    private bool shouldTransition;

    private void Awake()
    {
        Instance = this;
        transitionImage = GetComponent<Image>();
    }

    void Update()
    {
        if (shouldTransition)
        {
            transitionImage.material.SetFloat("_Cutoff", Mathf.MoveTowards(transitionImage.material.GetFloat("_Cutoff"), 1.1f, transitionSpeed * Time.deltaTime));
        }
        else
        {
            transitionImage.material.SetFloat("_Cutoff", Mathf.MoveTowards(transitionImage.material.GetFloat("_Cutoff"), -0.1f, transitionSpeed * Time.deltaTime));
        }
    }

    public void ToogleTransition(float speed = 1f)
    {
        transitionSpeed = speed;
        shouldTransition = !shouldTransition;
    }

    public void ChangeTransition(Texture2D texture)
    {
        transitionImage.material.SetTexture("_TransitionTex", texture);
    }
}
