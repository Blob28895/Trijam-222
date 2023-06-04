using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PaintingController : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    public Texture2D mouseCursor;
    public int brushSize = 5;

    private Texture2D canvasTexture;
    private RectTransform canvasRect;
    private RawImage rawImage;
    private Vector2 previousPosition;

    private void Start()
    {
        rawImage = GetComponent<RawImage>();
        canvasRect = rawImage.rectTransform;
        canvasTexture = new Texture2D(800, 800);
        rawImage.texture = canvasTexture;

        Cursor.SetCursor(mouseCursor, Vector2.zero, CursorMode.Auto);
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

        DrawLine(startPixelPosition, endPixelPosition, Color.black);

        canvasTexture.Apply();
    }

    // Bresenham's line algorithm (i stole this from the internet)
    private void DrawLine(Vector2Int startPos, Vector2Int endPos, Color color)
    {
        int dx = Mathf.Abs(endPos.x - startPos.x);
        int dy = Mathf.Abs(endPos.y - startPos.y);
        int sx = (startPos.x < endPos.x) ? 1 : -1;
        int sy = (startPos.y < endPos.y) ? 1 : -1;
        int err = dx - dy;

        int halfBrushSize = brushSize / 2;

        while (true)
        {
            for (int x = -halfBrushSize; x <= halfBrushSize; x++)
            {
                for (int y = -halfBrushSize; y <= halfBrushSize; y++)
                {
                    Vector2Int brushPixelPosition = new Vector2Int(startPos.x + x, startPos.y + y);
                    canvasTexture.SetPixel(brushPixelPosition.x, brushPixelPosition.y, color);
                }
            }

            if (startPos.x == endPos.x && startPos.y == endPos.y)
                break;

            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                startPos.x += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                startPos.y += sy;
            }
        }
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
