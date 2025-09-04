using UnityEngine;

public interface IWaterFactory
{
    GameObject CreateWater();
    GameObject CreateWater(Vector3 position);
    GameObject CreateWater(Vector3 position, Quaternion rotation);
}

public class WaterFactory : IWaterFactory
{
    private readonly GameObject waterPrefab;

    public WaterFactory(GameObject waterPrefab)
    {
        this.waterPrefab = waterPrefab;
    }

    public GameObject CreateWater()
    {
        return Object.Instantiate(waterPrefab);
    }

    public GameObject CreateWater(Vector3 position)
    {
        return Object.Instantiate(waterPrefab, position, Quaternion.identity);
    }

    public GameObject CreateWater(Vector3 position, Quaternion rotation)
    {
        return Object.Instantiate(waterPrefab, position, rotation);
    }
}
