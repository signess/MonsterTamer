using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TamerController : MonoBehaviour
{
    [SerializeField] new string name;
    [SerializeField] Sprite sprite;
    [SerializeField] Dialog dialog;
    [SerializeField] GameObject exclamation;
    [SerializeField] GameObject fov;

    Character character;

    public string Name { get => name; }
    public Sprite Sprite { get => sprite; }

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Start()
    {
        SetFOVRotation(character.Animator.DefaultDirection);
    }
    public IEnumerator TriggerTrainerBattle(PlayerController player)
    {
        exclamation.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        exclamation.SetActive(false);


        var diff = player.transform.position - transform.position;
        var moveVector = diff - diff.normalized;
        moveVector = new Vector2(Mathf.Round(moveVector.x), Mathf.Round(moveVector.y));
        yield return character.Move(moveVector);

        //ShowDialog
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog, () =>
        {
            Debug.Log("Start battle!");
        })
        );
    }

    public void SetFOVRotation(FacingDirection dir)
    {
        float angle = 0f;
        if (dir == FacingDirection.Right)
            angle = 90f;
        else if (dir == FacingDirection.Up)
            angle = 180f;
        else if (dir == FacingDirection.Left)
            angle = 270f;

        fov.transform.eulerAngles = new Vector3(0f, 0f, angle);
    }
}
