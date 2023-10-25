using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollapsePanel : MonoBehaviour
{
    private Vector3 collapsed;
    private Vector3 extend;
    private bool isCollapsed;

    private void Start()
    {
        extend = gameObject.transform.position;

        collapsed = new Vector3(
            gameObject.transform.position.x - 5f,
            gameObject.transform.position.y,
            gameObject.transform.position.z
        );

        StartCoroutine(slideTo(gameObject, collapsed, 3f));
    }

    public void invertCollapse()
    {
        isCollapsed = !isCollapsed;
        if (!isCollapsed)
            StartCoroutine(slideTo(gameObject, collapsed, 0f));
        else
        {
            StartCoroutine(slideTo(gameObject, extend, 0f));
        }
    }

    IEnumerator slideTo(GameObject currentPosition, Vector3 targetPosition, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        float dist = Vector3.Distance(currentPosition.transform.position, targetPosition);
        float startTime = Time.time;

        while (Mathf.Abs(dist) > 0.01f)
        {
            float distCovered = (Time.time - startTime) * 1.5f;

            currentPosition.transform.position = Vector3.Lerp(
                currentPosition.transform.position,
                targetPosition,
                distCovered / dist
            );

            dist = Vector3.Distance(currentPosition.transform.position, targetPosition);
            yield return null;
        }
    }
}
