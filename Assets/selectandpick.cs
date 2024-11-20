using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
public class selectandpick : MonoBehaviour
{
    [SerializeField]
    List<Sprite> images;
    [SerializeField]
    private OptionEnt prefab;
    [SerializeField]
    Transform Parent;

    public List<Entity> ListEnti = new List<Entity>();
    void Start()
    {
        for(int i=0; i<12; i++)
        {
            var _obj = Instantiate(prefab, Parent).GetComponent<OptionEnt>();
            GetRandomEntity(_obj);
        }

    }

    void GetRandomEntity(OptionEnt ent)
    {
        int _random = UnityEngine.Random.Range(1, 10) * 100;
        ent.CreateEnti(images[UnityEngine.Random.Range(0, ListEnti.Count)], _random);
        
    }
       

    // Update is called once per frame
    


}
