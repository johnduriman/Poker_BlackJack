using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeIn : MonoBehaviour
{
    public GameObject panel;
    private float alpha;
    private Image _image;

    private void Awake()
    {
        _image = panel.GetComponent<Image>();
    }

    private void OnEnable()
    {
        alpha = 1.0f;
        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, alpha);
        StartCoroutine(LowerAlpha());
    }

    IEnumerator LowerAlpha()
    {
        while (Mathf.Abs(alpha) > 0.1f)
        {
            alpha -= .05f;
            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, alpha);
            yield return null;
        }
        panel.SetActive(false);
    }
}
