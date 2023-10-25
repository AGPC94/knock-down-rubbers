using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum BattleState { MENU, PLAYER1TURN, PLAYER2TURN, END }

public class BattleSystem : MonoBehaviour
{
    public BattleState state;

    public static BattleSystem instance;

    [Header("HUD")]
    [SerializeField] GameObject hud;
    [SerializeField] Text txtTurn;
    [SerializeField] Image imgHand;
    [SerializeField] Slider sldForce;
    [SerializeField] Gradient gradient;
    [SerializeField] Image fill;

    [Header("Menu")]
    [SerializeField] GameObject currentMenu;
    [Space]
    [SerializeField] GameObject tutorialMenu;
    [SerializeField] GameObject MainMenu;
    [SerializeField] GameObject startMenu;

    [Header("Other")]
    [SerializeField] Text txtInfo;


    [Header("Players")]
    [SerializeField] Rubber p1;
    [SerializeField] Rubber p2;
    [SerializeField] Rubber turnPlayer;


    Camera cam;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        cam = Camera.main;
    }

    void Start()
    {
        state = BattleState.MENU;
        currentMenu = MainMenu;
        turnPlayer = p1;
    }

    // Update is called once per frame
    void Update()
    {
        Showturn();
        ShowHand();
    }

    void Showturn()
    {
        switch (state)
        {
            case BattleState.MENU:
                hud.SetActive(false);
                break;

            case BattleState.PLAYER1TURN:
                hud.SetActive(true);
                txtTurn.text = "Player 1 turn";
                turnPlayer = p1;
                break;

            case BattleState.PLAYER2TURN:
                hud.SetActive(true);
                txtTurn.text = "Player 2 turn";
                turnPlayer = p2;
                break;

            case BattleState.END:
                hud.SetActive(false);
                break;
        }
    }
    void ShowHand()
    {
        Vector3 scale = imgHand.transform.localScale;

        if (turnPlayer.isRHanded)
            scale.x = 1;
        else
            scale.x = -1;

        imgHand.transform.localScale = scale;
    }

    public void StartTurn()
    {
        state = BattleState.PLAYER1TURN;
        CameraManager.instance.ChangeTarget(GameObject.Find("Eraser1").transform);
        //ChangeTurn();
    }

    public void ChangeTurn()
    {
        switch (state)
        {
            case BattleState.PLAYER1TURN:
                state = BattleState.PLAYER2TURN;
                CameraManager.instance.ChangeTarget(GameObject.Find("Eraser2").transform);
                break;

            case BattleState.PLAYER2TURN:
                state = BattleState.PLAYER1TURN;
                CameraManager.instance.ChangeTarget(GameObject.Find("Eraser1").transform);
                break;
        }
    }

    public void EndMatch()
    {
        state = BattleState.END;

        if (p1.isDead)
            txtInfo.text = "Player 2 WON!";
        else if (p2.isDead)
            txtInfo.text = "Player 1 WON!";
        else
            txtInfo.text = "DRAW!";

        AudioManager.Play("Victory");

        Invoke("Restart", 3f);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    public void ShowForce(float current, float max)
    {
        sldForce.value = current;
        sldForce.maxValue = max;
        sldForce.minValue = 0;
        fill.color = gradient.Evaluate(sldForce.normalizedValue);
    }

    #region Buttons

    public void StartMatch()
    {
        cam.GetComponent<CameraManager>().StartAnim();
        OpenMenu(startMenu);
        AudioManager.Play("Fight");
        Invoke("StartTurn", 1.3f);
    }

    public void OpenMenu(GameObject menu)
    {
        AudioManager.Play("Select");
        currentMenu.SetActive(false);
        menu.SetActive(true);
        currentMenu = menu;

    }

    public void Quit()
    {
        Application.Quit();
    }
    
    #endregion
}
