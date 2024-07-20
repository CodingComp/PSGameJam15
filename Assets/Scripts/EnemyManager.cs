using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Enemy Manager. Deals with spawning / despawning enemies, keeping track of each enemy in the world
/// and checking if an enemy is in light when said enemy becomes visible.
/// </summary>
public class EnemyManager : MonoBehaviour
{
    private ResolutionManager rm;
    
    public GameObject enemyPrefab;

    private List<Enemy> enemies;
    private List<Enemy> visibleEnemies;
    public LayerMask enemyMask;
    
    private int rayPaddingX = 10;
    private int rayPaddingY = 10;

    private void Awake()
    {
        rm = GameObject.Find("GameManager").GetComponent<ResolutionManager>();
        
        // Finds Enemies Pre Placed In Level. 
        enemies = FindObjectsOfType<Enemy>().ToList();
        visibleEnemies = new List<Enemy>();
        
        foreach (Enemy enemy in enemies)
        {
            enemy.em = this;
        }
    }
    
    void Update()
    {

        if (visibleEnemies.Count == 0) return;
        
        /*
         * Loops over each visible enemy, checking its light value from the render texture.
         */
        
        // Camera render texture
        Texture2D cameraRender = new Texture2D(rm.renderTex.width, rm.renderTex.height, TextureFormat.RGB24, false);
        RenderTexture.active = rm.mainCamera.activeTexture;
        cameraRender.ReadPixels(new Rect(0, 0, rm.renderTex.width, rm.renderTex.height), 0, 0);
        cameraRender.Apply();
        
        foreach (Enemy enemy in visibleEnemies)
        {
            Vector3 originScreenPos = rm.mainCamera.WorldToScreenPoint(enemy.transform.position);
            originScreenPos.z = 0;

            bool inLight = false;
            
            // Loops over each ray pixel check using enemy position and a pixel padding x & y.
            for (int i = (int)originScreenPos.y - rayPaddingY; i < (int)originScreenPos.y + rayPaddingY; i++)
            {
                for (int j = (int)originScreenPos.x - rayPaddingX; j < (int)originScreenPos.x + rayPaddingX; j++)
                {
                        
                    Vector3 rPos = rm.mainCamera.ScreenToWorldPoint(new Vector3(j, i, 0));
                    rPos += rm.mainCamera.transform.right * rm.pixelSize / 2;
                    rPos += rm.mainCamera.transform.up * rm.pixelSize / 2;
                    
                    // Checks if pixel raycast hits enemy. If so check light value and update inLight accordingly.
                    if (Physics.Raycast(rPos, rm.mainCamera.transform.forward, out var hitInfo, 100.0f,
                            enemyMask))
                    {
                        if ((0.2126 * cameraRender.GetPixel(j, i).r + 0.7152 * cameraRender.GetPixel(j, i).g +
                             0.0722 * cameraRender.GetPixel(j, i).b) > 0.01f) inLight = true; // Break if any pixel is in light?
                    }
                }
            }

            enemy.inLight = inLight;
        }
    }

    public void EnemyDied(Enemy enemy)
    {
        enemies.Remove(enemy);
        if (visibleEnemies.Contains(enemy)) visibleEnemies.Remove(enemy);
        
        Destroy(enemy.gameObject);
    }

    public void EnemyVisible(Enemy enemy)
    {
        visibleEnemies.Add(enemy);
    }

    public void EnemyLeftVisibility(Enemy enemy)
    {
        visibleEnemies.Remove(enemy);
    }
}
