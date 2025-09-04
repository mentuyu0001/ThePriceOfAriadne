using UnityEngine;

public interface IKnifeFactory
{
    GameObject CreateKnife();
    GameObject CreateKnife(Vector3 position);
    GameObject CreateKnife(Vector3 position, Quaternion rotation);
}

public class KnifeFactory : IKnifeFactory
{
    private readonly GameObject knifePrefab;

    public KnifeFactory(GameObject knifePrefab)
    {
        this.knifePrefab = knifePrefab;
    }

    public GameObject CreateKnife()
    {
        return Object.Instantiate(knifePrefab);
    }

    public GameObject CreateKnife(Vector3 position)
    {
        return Object.Instantiate(knifePrefab, position, Quaternion.identity);
    }

    public GameObject CreateKnife(Vector3 position, Quaternion rotation)
    {
        return Object.Instantiate(knifePrefab, position, rotation);
    }
}
