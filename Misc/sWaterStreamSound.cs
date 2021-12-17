using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;


public class sWaterStreamSound : MonoBehaviour
{
    [SerializeField] PathCreator waterPathCreator;

    [SerializeField] Transform streamSound;

    Transform player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.instance.player;
    }

    // Update is called once per frame
    void Update()
    {
        if (streamSound != null && player != null)
        {
            streamSound.position = waterPathCreator.path.GetClosestPointOnPath(player.position);
        }
    }


}
