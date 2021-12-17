using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using UnityEngine;

public class sGate : MonoBehaviour
{
    [SerializeField] Animator anim;

    string openParam = "Open";

    private bool agentOpening;
    [SerializeField] private bool isDoor;
    private float doorCloseDuration = 0.25f; //Dont let the door close if an agent has opened it within this time
    private float doorCloseTimer;

    // Start is called before the first frame update
    void Start()
    {
        if (!anim)
            anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (agentOpening) doorCloseTimer += Time.deltaTime;
        if (isDoor) return;
        if (Vector3.Distance(GameManager.instance.player.position, transform.position) <= 7.5f)
        {
            anim.SetBool(openParam, true);
        }
        else if (!agentOpening)
        {
            anim.SetBool(openParam, false);
        }
    }

    public void OpenGate()
    {
        Debug.Log("OML: Open Gate");
        
        doorCloseTimer = 0f;
        agentOpening = true;
        anim.SetBool(openParam, true);
    }

    public void CloseGate()
    {
        agentOpening = false;
        anim.SetBool(openParam, false);
    }
    
}
