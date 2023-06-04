using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PaintingController : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    private Texture2D canvasTexture;
    private RectTransform canvasRect;
    private RawImage rawImage;
    private Vector2 previousPosition;

    private void Start()
    {
        rawImage = GetComponent<RawImage>();
        canvasRect = rawImage.rectTransform;
        canvasTexture = new Texture2D(256, 256);
        rawImage.texture = canvasTexture;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        previousPosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 currentPosition = eventData.position - new Vector2(canvasRect.position.x, canvasRect.position.y);
        DrawOnCanvas(previousPosition, currentPosition);
        previousPosition = currentPosition;
    }

    private void DrawOnCanvas(Vector2 startPosition, Vector2 endPosition)
    {
        Vector2Int startPixelPosition = WorldToPixelCoordinates(startPosition);
        Vector2Int endPixelPosition = WorldToPixelCoordinates(endPosition);

        Vector2Int pixelSize = endPixelPosition - startPixelPosition;
        Vector2Int direction = pixelSize;
        int steps = Mathf.CeilToInt(pixelSize.magnitude);

        for (int i = 0; i <= steps; i++)
        {
            Vector2Int pixelPosition = startPixelPosition + (direction * i);
            canvasTexture.SetPixel(pixelPosition.x, pixelPosition.y, Color.black);
        }

        canvasTexture.Apply();
    }

    private Vector2Int WorldToPixelCoordinates(Vector2 worldPosition)
    {
        Vector2 localPosition = worldPosition - new Vector2(canvasRect.position.x, canvasRect.position.y);
        return new Vector2Int(
            Mathf.FloorToInt(localPosition.x / canvasRect.rect.width * canvasTexture.width),
            Mathf.FloorToInt(localPosition.y / canvasRect.rect.height * canvasTexture.height)
        );
    }
}
