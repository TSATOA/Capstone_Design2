using UnityEngine;
using System.Collections;

public class Anvil_Strike : MonoBehaviour
{

    public GameObject anvilStrikeFX;
    private bool strikeFlag = false;

    void Start()
    {

        anvilStrikeFX.SetActive(false);

    }

    void Update()
    {

        if (Input.GetButtonDown("Fire1"))
        {

            if (strikeFlag == false)
            {
                StartCoroutine("Strike");
            }

        }

    }

    IEnumerator Strike()
    {
 
        strikeFlag = true;

        anvilStrikeFX.SetActive(true);
        yield return new WaitForSeconds(1.2f);
        anvilStrikeFX.SetActive(false);

        strikeFlag = false;

    }

}