using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gold : MonoBehaviour
{
    [SerializeField]
    private GameObject goldLocation;

    [SerializeField]
    private AudioClip goldSoundClip;

    private void Start()
    {
        StartCoroutine(placeCard(gameObject, goldLocation, 0f));
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
            float distCovered = (Time.time - startTime) * 5f;

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
        SoundFXManager.soundFXManager.PlaySoundFXClip(goldSoundClip, goldLocation.transform, 1f);
        Destroy(gameObject);
    }
}
