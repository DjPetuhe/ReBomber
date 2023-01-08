using System.Linq;
using UnityEngine;

public class DestroyingWallDrop : MonoBehaviour
{
    [Header("Upgrade prefabs")]
    [SerializeField] GameObject UpgradeBlastPrefab;
    [SerializeField] GameObject UpgradeBombPrefab;
    [SerializeField] GameObject UpgradeHpUpPrefab;
    [SerializeField] GameObject UpgradeSpeedPrefab;

    [Header("Drop chance")]
    [Range(0f, 1f)]
    [SerializeField] double[] UpgradesChance;

    [Header("Upgrades Coefficient")]
    [Range(1, 10)]
    [SerializeField] int UpgradeBlastCoef;
    [Range(1, 10)]
    [SerializeField] int UpgradeBombCoef;
    [Range(1, 10)]
    [SerializeField] int UpgradeHpUpCoef;
    [Range(1, 10)]
    [SerializeField] int UpgradeSpeedCoef;

    private void OnDestroy()
    {
        GameObject.FindGameObjectWithTag("TilemapManager").GetComponent<TilemapManager>().AfterWallDestroy(transform.position);
        if (transform.position == GameObject.Find("Map").GetComponent<TilemapManager>().EndingCoords) return;
        int dif = GameObject.Find("DifficultyManager").GetComponent<DifficultyManager>().DifficultyInt;
        int[] coefs = { UpgradeBlastCoef, UpgradeBombCoef, UpgradeHpUpCoef, UpgradeSpeedCoef };
        GameObject[] prefabs = { UpgradeBlastPrefab, UpgradeBombPrefab, UpgradeHpUpPrefab, UpgradeSpeedPrefab };
        int sum = coefs.Sum();
        if (Random.value > UpgradesChance[dif]) return;
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
