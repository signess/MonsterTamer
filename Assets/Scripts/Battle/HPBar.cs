using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField] Image health;
    public void SetHP(float hpNormalized)
    {
        health.fillAmount = hpNormalized;
    }
}
