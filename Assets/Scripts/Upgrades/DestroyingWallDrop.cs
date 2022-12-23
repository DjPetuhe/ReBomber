using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DestroyingWallDrop : MonoBehaviour
{
    [SerializeField]
    private GameObject UpgradeBlastPrefab;
    [SerializeField]
    private GameObject UpgradeBombPrefab;
    [SerializeField]
    private GameObject UpgradeHpUpPrefab;
    [SerializeField]
    private GameObject UpgradeSpeedPrefab;

    [Header("DropChance")]
    [SerializeField]
    [Range(0f, 1f)]
    private double[] UpgradesChance;

    [Header("Upgrades Coefficient")]
    [SerializeField]
    [Range(1, 10)]
    private int UpgradeBlastCoef;
    [SerializeField]
    [Range(1, 10)]
    private int UpgradeBombCoef;
    [SerializeField]
    [Range(1, 10)]
    private int UpgradeHpUpCoef;
    [SerializeField]
    [Range(1, 10)]
    private int UpgradeSpeedCoef;

    private void OnDestroy()
    {   if (transform.position == GameObject.Find("Map").GetComponent<TilemapManager>().EndingCoords) return;
        int dif = GameObject.Find("DifficultyManager").GetComponent<DifficultyManager>().DifficultyInt;
        int[] coefs = { UpgradeBlastCoef, UpgradeBombCoef, UpgradeHpUpCoef, UpgradeSpeedCoef };
        GameObject[] prefabs = { UpgradeBlastPrefab, UpgradeBombPrefab, UpgradeHpUpPrefab, UpgradeSpeedPrefab };
        int sum = coefs.Sum();
        if (Random.value <= UpgradesChance[dif])
        {
            int randomCoef = Random.Range(0, sum);
            for (int i = 0; i < coefs.Length; i++)
            {
                if (randomCoef >= coefs[i]) randomCoef -= coefs[i];
                else
                {
                    Instantiate(prefabs[i], transform.position, Quaternion.identity);
                    return;
                }
            }
        }
    }
}
