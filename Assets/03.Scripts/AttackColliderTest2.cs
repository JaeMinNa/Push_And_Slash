using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECM2.Examples.Slide;
using Photon.Pun;
using Photon.Realtime;

public class AttackColliderTest2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.I.SoundManager.StartSFX("ArrowHit", other.transform.position);
            //StartCoroutine(_cameraShake.COShake(0.3f, 0.3f));
            //_effect.Play();
            //_renderer.enabled = false;

            if (!other.GetComponent<PlayerCharacter>().PhotonView.IsMine)
            {
                other.GetComponent<PlayerCharacter>().
                    PhotonView.RPC("RPCPlayerNuckback", RpcTarget.AllViaServer, transform.position, 5f);
            }
        }
       
    }
}
