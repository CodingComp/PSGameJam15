using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles resolution values and resolution changes. Making sure everything works with the lower resolution RenderTexture.
/// </summary>
public class ResolutionManager : MonoBehaviour
{
    public RenderTexture renderTex;

    public float resScaleX;
    public float resScaleY;
    
    public float pixelSize;
    public Vector3 camWorldBoundry;
    
    public Camera mainCamera;

    private int currentScreenWidth;
    private int currentScreenHeight;
    
    void Start()
    {
        mainCamera = Camera.main;
        
        UpdateResolutionVariables();
    }
    
    void Update()
    {
        // Checks to see if the screen resolution changed and updates variables if so.
        if (currentScreenWidth != Screen.width || currentScreenHeight != Screen.height)
        {
            UpdateResolutionVariables();
        }
    }

    void UpdateResolutionVariables()
    {
        currentScreenWidth = Screen.width;
        currentScreenHeight = Screen.height;
        
        resScaleX = (float)renderTex.width / currentScreenWidth;
        resScaleY = (float)renderTex.height / currentScreenHeight;

        // Pixel Testing
        camWorldBoundry = new Vector2(mainCamera.orthographicSize * 2 * currentScreenWidth / currentScreenHeight, mainCamera.orthographicSize * 2);
        pixelSize = camWorldBoundry.x / renderTex.width;
    }
}