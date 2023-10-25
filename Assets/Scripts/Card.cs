using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public bool isSelected = false;
    public bool isDiscarded = false;
    public SO_Card cardProperty;
    public int pokerPosition = -1;

    [SerializeField]
    private GameObject discardPile;

    [SerializeField]
    private GameObject spawnPile;

    private Animation flipAnim;

    private void Start()
    {
        flipAnim = gameObject.GetComponent<Animation>();
    }

    private void OnEnable()
    {
        ButtonFunctions.onDiscard += moveToDiscard;
        ButtonFunctions.onDiscardSelected += checkSelected;
        GameManager.onDestroyCard += destroySelf;
    }

    private void OnDisable()
    {
        ButtonFunctions.onDiscard -= moveToDiscard;
        ButtonFunctions.onDiscardSelected -= checkSelected;
        GameManager.onDestroyCard -= destroySelf;
    }

    public void flipCardAnim()
    {
        flipAnim.Play("Flip_Card");
    }

    private void moveToDiscard()
    {
        isDiscarded = true;
        StartCoroutine(placeCard(gameObject, discardPile, 0f));
    }

    private void moveToSpawn()
    {
        StartCoroutine(placeCard(gameObject, spawnPile, 0f));
    }

    private void checkSelected()
    {
        // print(isSelected);
        // print(pokerPosition);
        if (isSelected)
        {
            moveToDiscard();
            showParticles();
        }
    }

    public void showParticles()
    {
        gameObject.transform.GetChild(1).gameObject.SetActive(isSelected);
        if (isSelected)
            gameObject.transform.localScale = new Vector3(.19f, .19f, .19f);
        else
            gameObject.transform.localScale = new Vector3(.2f, .2f, .2f);
    }

    IEnumerator placeCard(GameObject currentPosition, GameObject targetPosition, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        float dist = Vector3.Distance(
            currentPosition.transform.position,
            targetPosition.transform.position
        );
        float startTime = Time.time;

        while (Mathf.Abs(dist) > 0.01f)
        {
            float distCovered = (Time.time - startTime) * 3f;

            currentPosition.transform.position = Vector3.Lerp(
                currentPosition.transform.position,
                targetPosition.transform.position,
                distCovered / dist
            );

            dist = Vector3.Distance(
                currentPosition.transform.position,
                targetPosition.transform.position
            );
            yield return null;
        }
        if (isDiscarded)
        {
            isSelected = false;
            showParticles();
        }
    }

    private void destroySelf()
    {
        if (isDiscarded)
            Destroy(gameObject);
    }
}
