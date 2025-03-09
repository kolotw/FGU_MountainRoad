using UnityEngine;
using TMPro; // 引入 TextMeshPro 命名空間
using System.Collections.Generic;
using UnityEngine.UI;

namespace Gley.UrbanSystem.Internal
{
    [System.Serializable]
    public class AxleInfo
    {
        public WheelCollider leftWheel;
        public WheelCollider rightWheel;
        public bool motor;
        public bool steering;
    }

    public class PlayerCar : MonoBehaviour
    {
        public bool isG29 = false;
        public bool 無敵模式 = false;
        public GameObject 方向盤;
        public List<AxleInfo> axleInfos;
        public Transform centerOfMass;
        public float maxMotorTorque = 500f; // 馬達最大扭矩
        public float maxSteeringAngle = 30f; // 最大轉向角
        public float maxBrakeTorque = 15000f; // 煞車最大扭矩
        public float reverseTorqueMultiplier = 1.5f; // 倒車時的扭矩增量

        // 摩擦力屬性
        public float wheelForwardFriction = 1.0f; // 前進摩擦力
        public float wheelSidewaysFriction = 1.0f; // 側向摩擦力

        // UI 顯示時速
        public TextMeshProUGUI speedText; // 使用 TextMeshPro 顯示時速
        public float currentSpeed; // 當前時速
        //時速表
        public float maxSpeed = 0.0f;
        public float minSpeedArrowAngle;
        public float maxSpeedArrowAngle;
        public RectTransform arrow; // The arrow in the speedometer

        IVehicleLightsComponent lightsComponent;
        bool mainLights;
        bool brake;
        bool reverse;
        bool blinkLeft;
        bool blinkRifgt;
        float realtimeSinceStartup;
        Rigidbody rb;

        UIInput inputScript;

        private KartControls controls; // Kart 控制輸入類別
        private float inputDir; // 單一軸的輸入值，用於控制左右旋轉
        public float motorInput; // 前進和後退的輸入值
        private bool resetPos = false;

        //hit something...
        public int hitCone = 0;
        public int hitWall = 0;
        public int hitCar = 0;
        public int rolling = 0;
        GameObject[] cone;
        public TMP_Text gameText;
        public TMP_Text timerText;
        public TMP_Text statusText;
        public TMP_Text scoreText;
        float currentTime=0f;
        float longDist = 50f;
        Vector3 target = Vector3.zero;
        string gameString = string.Empty;
        public float timeElapsed = 0f; // 紀錄已經過的時間

        //偵測翻滾
        private bool isFlipped = false;
        public float score = 100; //可減分數
        public Image guiIcon;
        public Sprite[] statusIcons;
        public Image scoreBar;
        
        bool GameOver = false;
        public bool isReset = false;
        public bool isPause = false;

        //G29
        float fwdPower = 0;
        float backPower = 0;
        private string actualState;
        private void Awake()
        {
            // 初始化 KartControls
            controls = new KartControls();

            // 檢查是否成功初始化
            if (controls == null)
            {
                Debug.LogError("Failed to initialize controls. Ensure the Input System is set up correctly.");
            }

            // 初始化燈光控制組件
            lightsComponent = GetComponent<VehicleLightsComponent>();
            if (lightsComponent != null)
            {
                lightsComponent.Initialize();
            }
            else
            {
                Debug.LogError("VehicleLightsComponent not found on the GameObject.");
            }

            // 初始化 UIInput
            inputScript = gameObject.AddComponent<UIInput>().Initialize();
        }

        private void Start()
        {
            // 初始化剛體
            rb = GetComponent<Rigidbody>();
            rb.mass = 1500; // 減少車輛質量以提升加速度
            rb.drag = 0.1f; // 減少阻力，使車輛更容易加速
            rb.angularDrag = 0.5f;

            // 設置質量中心
            if (centerOfMass != null)
            {
                rb.centerOfMass = centerOfMass.localPosition;
            }
            else
            {
                Debug.LogError("CenterOfMass is not assigned in the inspector.");
            }

            // 啟用控制
            controls.Enable(); // 確保在 Start 中啟用控制

            // 綁定方向輸入事件
            if (controls.asset != null) // 使用 asset 進行 null 檢查
            {
                //機車龍頭方向
                controls.KartActionMaps.Val_Direction.performed += ctx => inputDir = ctx.ReadValue<float>();
                controls.KartActionMaps.Val_Direction.canceled += ctx => inputDir = 0; // 停止輸入時，重置為 0

                // 監聽前進按鈕
                controls.KartActionMaps.Btn_Forward.performed += ctx => motorInput = 1f; // 前進
                controls.KartActionMaps.Btn_Forward.canceled += ctx => motorInput = 0f;

                // 監聽左轉按鈕
                controls.KartActionMaps.Btn_Dir_Left.performed += ctx => inputDir = -1f; // 
                controls.KartActionMaps.Btn_Dir_Left.canceled += ctx => inputDir = 0f;

                // 監聽右轉按鈕
                controls.KartActionMaps.Btn_Dir_Right.performed += ctx => inputDir = 1f; // 
                controls.KartActionMaps.Btn_Dir_Right.canceled += ctx => inputDir = 0f;

                // 監聽倒退按鈕
                controls.KartActionMaps.Btn_Backward.performed += ctx => motorInput = -1f; // 倒退
                controls.KartActionMaps.Btn_Backward.canceled += ctx => motorInput = 0f;

                //search cone
                controls.KartActionMaps.ResetPosition.performed += ctx => resetPos = true;
                controls.KartActionMaps.ResetPosition.canceled += ctx => resetPos = false;
            }
            else
            {
                Debug.LogError("KartActionMaps is not initialized correctly. Check Input Action asset configuration.");
            }

            // 調整車輪摩擦力
            foreach (AxleInfo axleInfo in axleInfos)
            {
                AdjustWheelFriction(axleInfo.leftWheel);
                AdjustWheelFriction(axleInfo.rightWheel);
            }

            statusText.text = string.Empty;
            checkScore();
        }
        void clearStatusText()
        {
            statusText.text = string.Empty ;
        }
        private void AdjustWheelFriction(WheelCollider wheel)
        {
            WheelFrictionCurve forwardFriction = wheel.forwardFriction;
            forwardFriction.stiffness = wheelForwardFriction; // 設置前進摩擦力
            wheel.forwardFriction = forwardFriction;

            WheelFrictionCurve sidewaysFriction = wheel.sidewaysFriction;
            sidewaysFriction.stiffness = wheelSidewaysFriction; // 設置側向摩擦力
            wheel.sidewaysFriction = sidewaysFriction;
        }
        double ConvertRange(double value)
        {
            // 将原区间 [-1, 1] 映射到 [0, 1]
            return (value + 1) / 2;
        }
        float ConvertRange(float oldValue, float min, float max)
        {
            float oldMin = -32767;
            float oldMax = 32767;
            float newMin = -1;
            float newMax = 1;

            // Convert oldValue to the new range
            float newValue = ((oldValue - oldMin) / (oldMax - oldMin)) * (newMax - newMin) + newMin;
            return newValue;
        }
        float ConvertAngle(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            // 確保值位於輸入範圍內（可選）
            value = Mathf.Clamp(value, fromMin, fromMax);
            // 映射值到新範圍
            return toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
        }
        private void FixedUpdate()
        {
            if(isPause) return;

            if (isG29)
            {
                //CONTROLLER STATE
                actualState = "Steering wheel current state : \n\n";
                LogitechGSDK.DIJOYSTATE2ENGINES rec;
                rec = LogitechGSDK.LogiGetStateUnity(0);
                actualState += "x-axis position :" + rec.lX + "\n"; //forward
                actualState += "y-axis position :" + rec.lY + "\n"; //dir
                actualState += "z-axis position :" + rec.lZ + "\n"; //break
                fwdPower = -ConvertRange(rec.lY, -1,0);
                backPower = ConvertRange(rec.lRz, -1, 0);
                motorInput = fwdPower + backPower;
                actualState += "z-axis rotation :" + rec.lRz + "\n";
                inputDir = ConvertRange(rec.lX,-0.5f,0.5f);
                //Debug.Log(inputDir * 440);

                方向盤.transform.localEulerAngles = new Vector3(inputDir * 440, 0, 0);


                print(inputDir);
                //print(motorInput);
            }



            //偵測翻滾
            {
                // 取得車輛當前的歐拉角（世界座標系）
                Vector3 carEulerAngles = rb.rotation.eulerAngles;

                // 計算 X 軸和 Z 軸的角度，並將角度限制在 -180 到 180 度之間
                float pitchAngle = Mathf.DeltaAngle(0, carEulerAngles.x); // X 軸（Pitch）
                float rollAngle = Mathf.DeltaAngle(0, carEulerAngles.z);  // Z 軸（Roll）

                // 設定翻滾的角度閾值
                float pitchThreshold = 30f; // 前後傾斜的角度閾值
                float rollThreshold = 45f;  // 左右翻滾的角度閾值

                // 偵測是否翻滾
                if (Mathf.Abs(pitchAngle) > pitchThreshold || Mathf.Abs(rollAngle) > rollThreshold)
                {
                    if (!isFlipped)
                    {
                        isFlipped = true;
                        statusText.text = "翻車";
                        rolling++;
                        if(currentSpeed > 50)
                        {
                            score -= 50;
                        }
                        else
                        {
                            score -= 10;
                        }
                        
                        checkScore();
                        //Debug.Log("車輛已翻滾！");
                    }
                }
                else
                {
                    if (isFlipped)
                    {
                        isFlipped = false;
                        statusText.text = string.Empty;
                        //Debug.Log("車輛已恢復正常。");
                    }
                }
            }
            // 判斷當前車輛速度和坡度
            float motor = maxMotorTorque * motorInput;
            float steering = maxSteeringAngle * inputDir;
            float localVelocity = transform.InverseTransformDirection(rb.velocity).z;

            reverse = localVelocity < 0; // 判斷是否在倒車
            brake = false;

            // 確認車輛是否在下坡倒車，並施加更大扭矩
            if (motorInput < 0 && localVelocity > 0)
            {
                motor *= reverseTorqueMultiplier; // 增加倒車扭矩以克服重力
                brake = true; // 當車輛滑動過快時啟用煞車
            }

            foreach (AxleInfo axleInfo in axleInfos)
            {
                // 設置轉向角度
                if (axleInfo.steering)
                {
                    axleInfo.leftWheel.steerAngle = steering;
                    axleInfo.rightWheel.steerAngle = steering;
                }

                // 設置馬達扭矩和煞車扭矩
                if (axleInfo.motor)
                {
                    if (brake)
                    {
                        axleInfo.leftWheel.brakeTorque = maxBrakeTorque; // 使用最大煞車扭矩進行煞車
                        axleInfo.rightWheel.brakeTorque = maxBrakeTorque; // 使用最大煞車扭矩進行煞車
                        axleInfo.leftWheel.motorTorque = 0; // 煞車時馬達輸出為 0
                        axleInfo.rightWheel.motorTorque = 0; // 煞車時馬達輸出為 0
                    }
                    else
                    {
                        axleInfo.leftWheel.brakeTorque = 0;
                        axleInfo.rightWheel.brakeTorque = 0;
                        axleInfo.leftWheel.motorTorque = motor;
                        axleInfo.rightWheel.motorTorque = motor;
                    }
                }

                ApplyLocalPositionToVisuals(axleInfo.leftWheel);
                ApplyLocalPositionToVisuals(axleInfo.rightWheel);
            }

            // 計算車輛時速（km/h）
            currentSpeed = rb.velocity.magnitude * 3.6f; // 速度（米/秒）轉換為 km/h

            // 如果有 UI 顯示速度，則更新 Text 組件
            if (speedText != null)
            {
                speedText.text = Mathf.RoundToInt(currentSpeed).ToString() + " km/h";
                //時速文字變色
                speedTxtColor();
                if (arrow != null)
                    arrow.localEulerAngles =
                        new Vector3(0, 0, Mathf.Lerp(minSpeedArrowAngle, maxSpeedArrowAngle, currentSpeed / maxSpeed));
            }
            else
            {
                // 如果沒有 UI，則在控制台中輸出
                Debug.Log("Current Speed: " + currentSpeed.ToString("F1") + " km/h");
            }
            if (resetPos)
            {
                searchCone();
            }
            //停止
            if (Mathf.Abs(rb.velocity.magnitude) < 0.1f && rb.angularVelocity.magnitude < 0.1f)
            {
                rb.drag = 10f; // 增加線性阻尼讓車輛靜止
            }
            else
            {
                rb.drag = 0.2f; // 恢復正常阻尼
            }
        }
        void speedTxtColor()
        {
            if (currentSpeed < 30)
            {
                speedText.color = Color.white;
            }
            else if ((currentSpeed >= 30) && (currentSpeed < 40))
            {
                speedText.color = Color.yellow;
            }
            else if ((currentSpeed >= 40) && (currentSpeed < 50))
            {
                speedText.color = new Color(255,118,0);
            }
            else if ((currentSpeed >= 50) && (currentSpeed < 60))
            {
                speedText.color = Color.red;
            }
            else
            {
                speedText.color = new Color(231, 0, 255);
            }
        }
        void playAgain()
        {
            score = 100;
            hitCone = 0;
            hitCar = 0;
            hitWall = 0;
            rolling = 0;

            GameOver = false;
            gameString = "總　分：" + score + "\n越線數：" + hitCone + "\n撞牆數：" + hitWall + "\n撞車數：" + hitCar + "\n翻車數：" + rolling;
            gameText.text = gameString;
            scoreText.text = "生命值：" + score.ToString();

            Start();
        }
        private void Update()
        {
            if (isReset)
            {
                isReset=false;
                playAgain();
            }
            if (GameOver)
            {
                statusText.text = "Game Over!!!";
                
                return;
            }
            realtimeSinceStartup += Time.deltaTime;
            playTime();
            if (statusText.text.Length > 0)
            {
                currentTime += Time.deltaTime; // 增加每幀的時間
                if (currentTime > 1) // 檢查是否超過1秒
                {
                    clearStatusText();
                    currentTime = 0; // 重置計時
                }
            }
            // 主燈控制
            if (Input.GetKeyDown(KeyCode.Space))
            {
                mainLights = !mainLights;
                lightsComponent.SetMainLights(mainLights);
            }

            // 左轉燈控制
            if (Input.GetKeyDown(KeyCode.Q))
            {
                blinkLeft = !blinkLeft;
                if (blinkLeft == true)
                {
                    blinkRifgt = false;
                    lightsComponent.SetBlinker(BlinkType.BlinkLeft);
                }
                else
                {
                    lightsComponent.SetBlinker(BlinkType.Stop);
                }
            }

            // 右轉燈控制
            if (Input.GetKeyDown(KeyCode.E))
            {
                blinkRifgt = !blinkRifgt;
                if (blinkRifgt == true)
                {
                    blinkLeft = false;
                    lightsComponent.SetBlinker(BlinkType.BlinkRight);
                }
                else
                {
                    lightsComponent.SetBlinker(BlinkType.Stop);
                }
            }

            // 更新煞車和倒車燈狀態
            lightsComponent.SetBrakeLights(brake);
            lightsComponent.SetReverseLights(reverse);
            lightsComponent.UpdateLights(realtimeSinceStartup);
        }

        private void OnEnable()
        {
            if (controls != null)
            {
                controls.Enable(); // 啟用 Kart 控制
            }
            else
            {
                Debug.LogError("Controls is null in OnEnable. Check if controls are initialized correctly.");
            }
        }

        private void OnDisable()
        {
            if (controls != null)
            {
                controls.Disable(); // 停用 Kart 控制
            }
        }

        // 更新輪子位置和旋轉
        public void ApplyLocalPositionToVisuals(WheelCollider collider)
        {
            if (collider.transform.childCount == 0)
            {
                return;
            }

            Transform visualWheel = collider.transform.GetChild(0);

            Vector3 position;
            Quaternion rotation;
            collider.GetWorldPose(out position, out rotation);

            visualWheel.transform.position = position;
            visualWheel.transform.rotation = rotation;
        }
        private void OnTriggerEnter(Collider collision)
        {
            //print(collision.name);

            switch (collision.transform.tag) {
                case "cone":
                    hitCone++;
                    statusText.text = "越線";
                    Destroy(collision.gameObject);
                    currentTime = 0f;
                    score-=0.5f;
                    break;
                case "wall":
                    hitWall++;
                    statusText.text = "撞牆";
                    currentTime = 0f;
                    if (currentSpeed > 40)
                    {
                        score-=10;
                    }
                    else
                    {
                        score -= 2;
                    }
                    break;
                case "car":
                    hitCar++;
                    statusText.text = "撞車";
                    currentTime = 0f;
                    if(currentSpeed> 40)
                    {
                        score-=10;
                    }
                    else
                    {
                        score -= 2;
                    }
                    break ;
                case "EndPoint":
                    GameObject.Find("/GUI").GetComponent<GUI_Controller>().GameSet = true;
                    isPause = true;
                    GameOver = true;
                    break;
                default:
                    break;
            }
            
            checkScore();
        }
        void checkScore()
        {
            if (無敵模式) return;
            if ((score <= 100) && (score > 80))
            {
                guiIcon.sprite = statusIcons[0];
                scoreBar.color = new Color(118f/255f, 247f / 255f, 243f / 255f);
            }
            else if ((score <= 80) && (score > 60))
            {
                guiIcon.sprite = statusIcons[1];
                scoreBar.color = Color.yellow;
            }
            else if ((score <= 60) && (score > 40))
            {
                guiIcon.sprite = statusIcons[2];
                scoreBar.color = new Color(237f / 255f, 149f / 255f, 51f / 255f);
            }
            else if ((score <= 40) && (score > 20))
            {
                guiIcon.sprite = statusIcons[3];
                scoreBar.color = new Color(233f / 255f, 73f / 255f, 37f / 255f);
            }
            else if ((score <= 20) && (score > 0))
            {
                guiIcon.sprite = statusIcons[5];
                scoreBar.color = Color.red;
            }
            else
            {
                score = 0;
                guiIcon.sprite = statusIcons[6];
                statusText.text = "Game Over!!!";
                GameOver = true;
                GameObject.Find("/GUI").GetComponent<GUI_Controller>().GameSet = true;
                controls.Disable();
                //Destroy(this.gameObject);
            }
            gameString = "總　分：" + score + "\n越線數：" + hitCone + "\n撞牆數：" + hitWall + "\n撞車數：" + hitCar + "\n翻車數：" + rolling;
            gameText.text = gameString;
            scoreText.text = "生命值：" + score.ToString();
            float ss = (float)score / (float)100;
            if(ss < 0) ss= 0;
            scoreBar.transform.localScale = new Vector3(ss, 1, 1);

        }
        void searchCone()
        {
            float dist;
            cone = GameObject.FindGameObjectsWithTag("cone");
            if (cone != null)
            {
                foreach (GameObject go in cone)
                {
                    dist = Vector3.Distance(go.transform.position, this.transform.position);
                    if (dist < longDist)
                    {
                        longDist = dist;
                        target = go.transform.position;
                    }
                }
                target.y = target.y + 3f;
                this.transform.position = target;
                Vector3 rt = this.transform.eulerAngles;
                this.transform.eulerAngles = new Vector3(0, rt.y, 0);
                longDist = 300f;
            }
        }
        void playTime()
        {
            // 每幀更新時間
            timeElapsed += Time.deltaTime;

            // 計算分鐘和秒數
            int minutes = Mathf.FloorToInt(timeElapsed / 60f);
            int seconds = Mathf.FloorToInt(timeElapsed % 60f);

            // 格式化時間並顯示
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }    
}
