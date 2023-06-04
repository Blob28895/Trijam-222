using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PaintingController : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Texture2D mouseCursor;
    [SerializeField] private int brushSize = 5;
    [SerializeField] private Color brushColor = Color.black;

    private Texture2D canvasTexture;
    private RectTransform canvasRect;
    private RawImage rawImage;
    private Vector2 previousPosition;

    private int textureWidth = 800;
    private int textureHeight = 800;

    private void Start()
    {
        rawImage = GetComponent<RawImage>();
        canvasRect = rawImage.rectTransform;
        canvasTexture = new Texture2D(textureWidth, textureHeight);
        rawImage.texture = canvasTexture;

        mainCamera = Camera.main;

        Cursor.SetCursor(mouseCursor, Vector2.zero, CursorMode.Auto);

        DrawObstaclesOnTexture();
    }

    private void DrawObstaclesOnTexture()
    {
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");

        foreach (GameObject obstacle in obstacles)
        {
            // Retrieve the visual representation of the obstacle (e.g., sprite or texture)
            SpriteRenderer obstacleRenderer = obstacle.GetComponent<SpriteRenderer>();
            Texture2D obstacleTexture = obstacleRenderer.sprite.texture;

            // Determine the position on the texture
            Vector2 texturePosition = CalculateTexturePosition(obstacle.transform.position, textureWidth, textureHeight);

            // Determine the scaling factors for the obstacle
            float scaleX = obstacle.transform.localScale.x * (float)obstacleTexture.width / Screen.width;
            float scaleY = obstacle.transform.localScale.y * (float)obstacleTexture.height / Screen.height;

            texturePosition += new Vector2((textureWidth / 2) - (obstacleTexture.width * scaleX/2), (textureHeight / 2) - (obstacleTexture.height * scaleY/2));

            // Iterate over the pixels of the obstacle's texture
            for (int x = 0; x < obstacleTexture.width; x++)
            {
                for (int y = 0; y < obstacleTexture.height; y++)
                {
                    // Calculate the texture coordinate
                    int textureX = (int)(texturePosition.x + x * scaleX);
                    int textureY = (int)(texturePosition.y + y * scaleY);

                    // Check if the texture coordinate is within the texture bounds
                    if (IsWithinTextureBounds(new Vector2Int(textureX, textureY), canvasTexture))
                    {
                        // Retrieve the pixel color from the obstacle's texture
                        Color obstaclePixel = obstacleTexture.GetPixel(x, y);

                        // Set the corresponding pixel on the main texture
                        canvasTexture.SetPixel(textureX, textureY, obstaclePixel);
                    }
                }
            }
        }

    // Apply the changes to the texture
    canvasTexture.Apply();
    }

    private Vector2Int CalculateTexturePosition(Vector3 worldPosition, int textureWidth, int textureHeight)
    {
        // Convert the world position to texture coordinates
        float normalizedX = (worldPosition.x / Screen.width) * textureWidth;
        float normalizedY = (worldPosition.y / Screen.height) * textureHeight;

        // Convert the normalized coordinates to integer texture coordinates
        int x = Mathf.RoundToInt(normalizedX);
        int y = Mathf.RoundToInt(normalizedY);

        // Clamp the coordinates to ensure they are within the texture bounds
        x = Mathf.Clamp(x, 0, textureWidth - 1);
        y = Mathf.Clamp(y, 0, textureHeight - 1);

        return new Vector2Int(x, y);
    }

    private bool IsWithinTextureBounds(Vector2Int textureCoordinate, Texture2D texture)
    {
        int x = textureCoordinate.x;
        int y = textureCoordinate.y;

        // Check if the coordinates are within the texture bounds
        return x >= 0 && x < texture.width && y >= 0 && y < texture.height;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetPercentageDrawn();
        }
    }

    // implementing method from IPointerDownHandler
    public void OnPointerDown(PointerEventData eventData)
    {
        previousPosition = eventData.position;
    }

    // implementing method from IDragHandler
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 currentPosition = eventData.position - new Vector2(canvasRect.position.x, canvasRect.position.y);
        DrawOnCanvas(previousPosition, currentPosition);
        previousPosition = currentPosition;
    }

    public float GetPercentageDrawn()
    {
        // this counts all pixels that are not white as drawn. in the
        int totalPixels = canvasTexture.width * canvasTexture.height;
        int drawnPixels = 0;

        for (int x = 0; x < canvasTexture.width; x++)
        {
            for (int y = 0; y < canvasTexture.height; y++)
            {
                if (canvasTexture.GetPixel(x, y) == brushColor)
                    drawnPixels++;
            }
        }

        Debug.Log("Percentage drawn: " + (drawnPixels / (float)totalPixels) * 100 + "%");
        return (drawnPixels / (float)totalPixels) * 100;
    }

    private void DrawOnCanvas(Vector2 startPosition, Vector2 endPosition)
    {
        CheckForGameObjectObstacles();
        Vector2Int startPixelPosition = WorldToPixelCoordinates(startPosition);
        Vector2Int endPixelPosition = WorldToPixelCoordinates(endPosition);

        DrawLine(startPixelPosition, endPixelPosition, brushColor);

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

    private bool CheckForGameObjectObstacles()
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 
                                                    Mathf.Infinity, LayerMask.GetMask("Obstacle"));

        if(hitInfo.collider != null)
        {
            Debug.Log("Obstacle hit!");
            return true;
        }
        return false;
    }
}
