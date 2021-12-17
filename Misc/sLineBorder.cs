using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[DisallowMultipleComponent]
public class sLineBorder : MonoBehaviour
{

    float maxEmissionRate;

    VisualEffect vfx;

    public float VFXEmissionRate
    {
        set
        {
            if (value <= 0)
            {
                value = 0;
            }
            else if (value >= maxEmissionRate)
            {
                value = maxEmissionRate;
            }
            vfx.SetFloat("EmissionRate", value);
        }
    }


    GameObject borderPrefab;

    public Transform borderParent;

    List<Transform> borders = new List<Transform>();

    List<Material> borderMats = new List<Material>();

    List<Material> BorderMaterials
    {
        get
        {
            if (borderMats.Count != borders.Count)
            {
                borderMats.Clear();
                for (int i = 0; i < borders.Count; i++)
                {
                    borderMats.Add(borders[i].GetChild(0).GetComponent<Renderer>().material);
                }
            }
            return borderMats;
        }
    }

    float borderAlpha = 0f;

    public float BorderAlpha
    {
        get
        {
            return borderAlpha; 
        }
        set
        {
            //value = Mathf.Clamp(value, 0f, 1f);
            if (value > 1)
            {
                value = 1f;
            }
            else if (value < 0f)
            {
                value = 0f;
            }

            borderAlpha = value;

            //VFXEmissionRate = maxEmissionRate * borderAlpha;

            for (int i = 0; i < BorderMaterials.Count; i++)
            {
                BorderMaterials[i].SetFloat("Alpha", borderAlpha);
            }
        }
    }


    [SerializeField, Range(0f, 3f)] public float borderHeight;

    [SerializeField, Range(0f, 20f)] public float borderSize;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateSquareBorder()
    {
        if (borderParent == null)
        {
            borderParent = new GameObject(gameObject.name + "'s Border Parent").transform; // create the parent
            borderParent.parent = transform;
        }
        borderParent.position = transform.position;
        borderParent.rotation = transform.rotation; // keep the border oriented correcly
        if (borderParent.childCount > borders.Count || borders.Count <= 0 || borders[0] == null)
        {
            int childCount = borderParent.childCount;
            for (int i = 0; i < childCount; i++)
            {
                borderParent.GetChild(0).parent = AssetFinder.instance.Trash;
            }

            borders.Clear();
            for (int i = 0; i < 4; i++)
            {
                if (borderPrefab == null)
                {
                    borderPrefab = AssetFinder.instance.plotBorder;
                }
                GameObject temp = Instantiate(borderPrefab, transform.position, Quaternion.LookRotation(transform.forward));
                temp.transform.parent = borderParent;
                borders.Add(temp.transform);
            }
        }

        borders[0].position = transform.position + (transform.forward * (borderSize * 0.5f));
        borders[1].position = transform.position + (transform.right * (borderSize * 0.5f));
        borders[2].position = transform.position + (-transform.forward * (borderSize * 0.5f));
        borders[3].position = transform.position + (-transform.right * (borderSize * 0.5f));

        for (int i = 0; i < borders.Count; i++)
        {
            Quaternion dir = Quaternion.LookRotation(transform.forward);
            dir *= Quaternion.Euler(0, 90 * i, 0);
            borders[i].rotation = dir;

        }

        for (int i = 0; i < borders.Count; i++)
        {
            borders[i].localScale = new Vector3(borderSize, borderHeight, borderSize);
            borders[i].position = new Vector3(borders[i].position.x, transform.position.y + borders[i].localScale.y * 0.5f, borders[i].position.z);
        }
    }

    public void SpawnVFX(float size, float _maxEmissionRate)
    {
        vfx = Instantiate(AssetFinder.instance.plotVFX, transform.position, transform.rotation).GetComponent<VisualEffect>();
        vfx.SetVector3("SizeOfBox", new Vector3(size - 0.1f, 0.4f, size - 0.1f));
        VFXEmissionRate = 0;
        maxEmissionRate = _maxEmissionRate;
        for (int i = 0; i < BorderMaterials.Count; i++)
        {
            BorderMaterials[i].SetVector("Tiling", new Vector4(size, 1.24f, 0, 0));
        }

        vfx.transform.parent = transform;
    }

}
