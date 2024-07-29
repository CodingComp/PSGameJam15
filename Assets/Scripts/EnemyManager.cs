using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
///     Enemy Manager. Deals with spawning / despawning enemies, keeping track of each enemy in the world
///     and checking if an enemy is in light when said enemy becomes visible.
/// </summary>
public class EnemyManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public LayerMask enemyMask;

    private const int RayPaddingX = 10;
    private const int RayPaddingY = 10;

    private List<Enemy> _enemies;
    private ResolutionManager _rm;
    private List<Enemy> _visibleEnemies;

    private void Awake()
    {
        _rm = GameObject.Find("GameManager").GetComponent<ResolutionManager>();

        // Finds Enemies Pre Placed In Level. 
        _enemies = FindObjectsOfType<Enemy>().ToList();
        _visibleEnemies = new List<Enemy>();

        foreach (Enemy enemy in _enemies)
        {
            enemy.em = this;
        }
    }

    private void Update()
    {

        if (_visibleEnemies.Count == 0) return;

        /*
         * Loops over each visible enemy, checking its light value from the render texture.
         */

        // Camera render texture
        Texture2D cameraRender = new Texture2D(_rm.renderTex.width, _rm.renderTex.height, TextureFormat.RGB24, false);
        RenderTexture.active = _rm.mainCamera.activeTexture;
        cameraRender.ReadPixels(new Rect(0, 0, _rm.renderTex.width, _rm.renderTex.height), 0, 0);
        cameraRender.Apply();

        foreach (Enemy enemy in _visibleEnemies)
        {
            Vector3 originScreenPos = _rm.mainCamera.WorldToScreenPoint(enemy.transform.position);
            originScreenPos.z = 0;

            bool inLight = false;

            // Loops over each ray pixel check using enemy position and a pixel padding x & y.
            for (int i = (int)originScreenPos.y - RayPaddingY; i < (int)originScreenPos.y + RayPaddingY; i++)
            {
                for (int j = (int)originScreenPos.x - RayPaddingX; j < (int)originScreenPos.x + RayPaddingX; j++)
                {

                    Vector3 rPos = _rm.mainCamera.ScreenToWorldPoint(new Vector3(j, i, 0));
                    rPos += _rm.mainCamera.transform.right * _rm.pixelSize / 2;
                    rPos += _rm.mainCamera.transform.up * _rm.pixelSize / 2;

                    // Checks if pixel raycast hits enemy. If so check light value and update inLight accordingly.
                    if (Physics.Raycast(rPos, _rm.mainCamera.transform.forward, out var hitInfo, 100.0f,
                            enemyMask))
                    {
                        if (0.2126 * cameraRender.GetPixel(j, i).r + 0.7152 * cameraRender.GetPixel(j, i).g +
                            0.0722 * cameraRender.GetPixel(j, i).b > 0.01f) inLight = true; // Break if any pixel is in light?
                    }
                }
            }

            enemy.inLight = inLight;
        }
    }

    public void EnemyDied(Enemy enemy)
    {
        _enemies.Remove(enemy);
        if (_visibleEnemies.Contains(enemy)) _visibleEnemies.Remove(enemy);

        Destroy(enemy.gameObject);
    }

    public void EnemyVisible(Enemy enemy)
    {
        _visibleEnemies.Add(enemy);
    }

    public void EnemyLeftVisibility(Enemy enemy)
    {
        _visibleEnemies.Remove(enemy);
    }
}
