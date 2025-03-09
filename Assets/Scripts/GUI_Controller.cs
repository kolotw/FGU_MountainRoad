using Gley.UrbanSystem.Internal;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class GUI_Controller : MonoBehaviour
{
    private UIDocument uiDocument;
    private Button button1;
    private Button button2;
    private Button button3;
    private Button buttonAgain;
    private Button buttonCredit;
    private Button buttonBack;
    private Label Lab_Result;
    private VisualElement ve_StartBoard;
    private VisualElement ve_ResultBoard;
    private VisualElement ve_CreditBoard;

    button_StartPoint startPoints;

    public bool GameSet = false;
    private string resText;
    // Start is called before the first frame update
    void Start()
    {
        startPoints = GameObject.Find("�U�a�I").GetComponent<button_StartPoint>();
        // ��� UIDocument ����
        uiDocument = GetComponent<UIDocument>();

        // ��� UI �ڤ���
        var root = uiDocument.rootVisualElement;

        // �d�� Button �M Label ����
        button1 = root.Q<Button>("Button1"); // ���]�Ĥ@�ӫ��s�W�٬� "button1"
        button2 = root.Q<Button>("Button2"); // ���]�ĤG�ӫ��s�W�٬� "button2"
        button3 = root.Q<Button>("Button3"); // ���]�ĤT�ӫ��s�W�٬� "button3"
        buttonAgain = root.Q<Button>("But_Again");
        buttonCredit = root.Q<Button>("But_Credit");
        buttonBack = root.Q<Button>("But_Back");

        Lab_Result = root.Q<Label>("Lab_Result");

        ve_StartBoard = root.Q<VisualElement>("VE_Start");
        ve_ResultBoard = root.Q<VisualElement>("VE_Result");
        ve_CreditBoard = root.Q<VisualElement>("VE_Credit");

        // ���C�ӫ��s�K�[�I���ƥ�        
        button1.clicked += OnButton1Clicked;
        button2.clicked += OnButton2Clicked;
        button3.clicked += OnButton3Clicked;
        buttonAgain.clicked += OnButtonAgainClicked;
        buttonCredit.clicked += onButtonCreditClicked;
        buttonBack.clicked += OnButtonAgainClicked;

        ve_StartBoard.style.display = DisplayStyle.Flex;
        ve_ResultBoard.style.display = DisplayStyle.None;
        ve_CreditBoard.style.display = DisplayStyle.None;

        GameObject.Find("PlayerFirstMotor").GetComponent<PlayerCar>().isPause = true;
    }
    void onButtonCreditClicked()
    {
        ve_StartBoard.style.display = DisplayStyle.None;
        ve_ResultBoard.style.display = DisplayStyle.None;
        ve_CreditBoard.style.display = DisplayStyle.Flex;
    }
    private void OnButton1Clicked()
    {

        GameObject.Find("PlayerFirstMotor").GetComponent<PlayerCar>().isPause = false;
        GameObject.Find("PlayerFirstMotor").GetComponent<PlayerCar>().timeElapsed = 0f;
        ve_StartBoard.style.display = DisplayStyle.None;
        ve_ResultBoard.style.display = DisplayStyle.None;
        ve_CreditBoard.style.display = DisplayStyle.None;
        startPoints.��();
    }

    // �ĤG�ӫ��s���I���ƥ�B�z���
    private void OnButton2Clicked()
    {
        GameObject.Find("PlayerFirstMotor").GetComponent<PlayerCar>().isPause = false;
        GameObject.Find("PlayerFirstMotor").GetComponent<PlayerCar>().timeElapsed = 0f;
        ve_StartBoard.style.display = DisplayStyle.None;
        ve_ResultBoard.style.display = DisplayStyle.None;
        ve_CreditBoard.style.display = DisplayStyle.None;
        startPoints.��();
    }

    // �ĤT�ӫ��s���I���ƥ�B�z���
    private void OnButton3Clicked()
    {
        GameObject.Find("PlayerFirstMotor").GetComponent<PlayerCar>().isPause = false;
        GameObject.Find("PlayerFirstMotor").GetComponent<PlayerCar>().timeElapsed = 0f;
        ve_StartBoard.style.display = DisplayStyle.None;
        ve_ResultBoard.style.display = DisplayStyle.None;
        ve_CreditBoard.style.display = DisplayStyle.None;
        startPoints.��();
    }
    private void OnButtonAgainClicked() 
    {
        ve_StartBoard.style.display = DisplayStyle.Flex;
        ve_ResultBoard.style.display = DisplayStyle.None;
        ve_CreditBoard.style.display = DisplayStyle.None;
        
        GameObject.Find("PlayerFirstMotor").GetComponent<PlayerCar>().isReset = true;
        GameSet = false;
    }
    private void Update()
    {
        if (GameSet)
        {
            ve_ResultBoard.style.display = DisplayStyle.Flex;
            ve_StartBoard.style.display = DisplayStyle.None;
            ve_CreditBoard.style.display = DisplayStyle.None;
            resText = GameObject.Find("/Canvas/GameText").GetComponent<TMP_Text>().text;
            Lab_Result.text = resText;
        }
        if (Input.GetKeyUp(KeyCode.Escape)) 
        {
            GameObject.Find("PlayerFirstMotor").GetComponent<PlayerCar>().isReset = true;
            GameObject.Find("PlayerFirstMotor").GetComponent<PlayerCar>().isPause = true;
            
            ve_StartBoard.style.display = DisplayStyle.Flex;
            ve_ResultBoard.style.display = DisplayStyle.None;
            ve_CreditBoard.style.display = DisplayStyle.None;
        }

    }
}
