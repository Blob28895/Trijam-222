using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaintingManager : MonoBehaviour
{
    [SerializeField] private RawImage rawImage;
    private Texture2D texture;

    // Start is called before the first frame update
    void Start()
    {
        rawImage.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
        rawImage.rectTransform.anchoredPosition = new Vector2(0, 0);
        rawImage.rectTransform.pivot = new Vector2(0.5f, 0.5f);

        texture = new Texture2D(Screen.width, Screen.height);
        rawImage.texture = texture;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            DrawAtMousePos();
        }
    }

    private void DrawAtMousePos()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        Debug.Log(worldPos);

        texture.SetPixel((int)worldPos.x, (int)worldPos.y, Color.black);
        texture.Apply();
    }
}
