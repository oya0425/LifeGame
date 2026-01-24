using System.Linq;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    [SerializeField]private ItemData[] items; 
    public static ItemDatabase instance;

    private void Awake()
    {
        instance = this;
    }

    public ItemData GetRandomItem()
    {
        if (items.Length == 0) return null;

        return items[Random.Range(0, items.Length)];
    }


}
