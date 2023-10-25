using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum PlayerType { PLAYER1, PLAYER2 }

public class Rubber : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] PlayerType player;

    [Header("Objects")]
    GameObject point;
    [SerializeField] LayerMask whatIsRubber;

    [Header("Forces")]
    [SerializeField] float forceCurrent;
    [SerializeField] float forceMax;
    [SerializeField] float forceIncrease;

    [Header("Bools")]
    [SerializeField] bool hasThrow;
    public bool isDead;

    [Header("Mouse")]
    public bool isRHanded;

    [HideInInspector]
    public int clickWatch;
    [HideInInspector]
    public int clickThrow;



    Transform camPos;
    Rigidbody rb;
    CameraManager cam;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        point = GameObject.Find("Point");
        cam =FindObjectOfType<CameraManager>();

        isRHanded = true;
        clickWatch = 1;
        clickThrow = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
            return;

        if (
            (player == PlayerType.PLAYER1 && BattleSystem.instance.state == BattleState.PLAYER1TURN)
            ||
            (player == PlayerType.PLAYER2 && BattleSystem.instance.state == BattleState.PLAYER2TURN)
            )
        {
            BattleSystem.instance.ShowForce(forceCurrent, forceMax);
            ApplyForce();
            ChangeHanded();
            cam.MoveCamera(clickWatch);
            CheckStop();
        }
    }

    void CheckStop()
    {
        if (hasThrow && rb.velocity.sqrMagnitude == 0)
        {
            Debug.Log("Ha tirado");
            hasThrow = false;
            forceCurrent = 0;
            Invoke("ChangeTurn", 0.2f);
        }
    }

    void ChangeTurn()
    {
        BattleSystem.instance.ChangeTurn();
    }

    void ApplyForce()
    {
        if (hasThrow)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, whatIsRubber))
        {
            if (raycastHit.collider.GetComponent<Rubber>().player != player)
                return;

            point.SetActive(true);
            point.transform.position = raycastHit.point;
            point.transform.forward = -(point.transform.position - raycastHit.transform.position);

            if (Input.GetMouseButtonDown(clickThrow))
                forceCurrent = 0;

            if (Input.GetMouseButton(clickThrow))
            {
                if (forceCurrent < forceMax)
                    forceCurrent += Time.deltaTime * forceIncrease;
                else
                    forceCurrent = forceMax;
            }

            if (Input.GetMouseButtonUp(clickThrow))
            {
                Vector3 direction = transform.position - point.transform.position;
                //Vector3 direction = -raycastHit.point;
                direction.Normalize();
                rb.AddForce(direction * forceCurrent);
                rb.AddTorque(direction * forceCurrent);

                hasThrow = true;

                AudioManager.Play("HitRubber");
            }
        }
        else
            point.SetActive(false);
        
    }

    void ChangeHanded()
    {
        if (Input.GetMouseButtonDown(2))
        {
            isRHanded = !isRHanded;

            if (isRHanded)
            {
                clickWatch = 1;
                clickThrow = 0;
            }
            else
            {
                clickWatch = 0;
                clickThrow = 1;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.name == "LimitZone")
        {
            isDead = true;
            BattleSystem.instance.EndMatch();
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Rubber"))
        {
            AudioManager.Play("HitRubber");
        }
    }
}