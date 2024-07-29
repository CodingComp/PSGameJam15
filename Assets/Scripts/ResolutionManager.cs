using UnityEngine;

/// <summary>
///     Handles resolution values and resolution changes. Making sure everything works with the lower resolution
///     RenderTexture.
/// </summary>
public class ResolutionManager : MonoBehaviour
{
    public RenderTexture renderTex;

    public float resScaleX;
    public float resScaleY;

    public float pixelSize;
    public Vector3 camWorldBoundary;

    public Camera mainCamera;
    private int _currentScreenHeight;

    private int _currentScreenWidth;

    private void Start()
    {
        mainCamera = Camera.main;
        UpdateResolutionVariables();
    }

    private void Update()
    {
        // Checks to see if the screen resolution changed and updates variables if so.
        if (_currentScreenWidth != Screen.width || _currentScreenHeight != Screen.height)
        {
            UpdateResolutionVariables();
        }
    }

    private void UpdateResolutionVariables()
    {
        _currentScreenWidth = Screen.width;
        _currentScreenHeight = Screen.height;

        resScaleX = (float)renderTex.width / _currentScreenWidth;
        resScaleY = (float)renderTex.height / _currentScreenHeight;

        // Pixel Testing
        camWorldBoundary = new Vector2(mainCamera.orthographicSize * 2 * _currentScreenWidth / _currentScreenHeight, mainCamera.orthographicSize * 2);
        pixelSize = camWorldBoundary.x / renderTex.width;
    }

    /// <summary>
    ///     Returns mouse position on screen converted to the lowered resolution render texture.
    /// </summary>
    /// <returns>Render Texture Cursor Position</returns>
    public Vector3 GetMousePosition(float z = 0)
    {
        Vector3 cursorPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, z);
        return Vector3.Scale(cursorPos, new Vector3(resScaleX, resScaleY, 1));
    }
}
