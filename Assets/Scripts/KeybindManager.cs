using UnityEngine;
using TMPro;

public class KeybindManager : MonoBehaviour
{
    [Header("Ссылки на дроны")]
    public DroneController drone;           // Обычный дрон
    public AcroDroneController acroDrone;   // Наш новый Acro-дрон

    [Header("UI: Обычный дрон")]
    public TextMeshProUGUI upText;
    public TextMeshProUGUI downText;
    public TextMeshProUGUI yawLeftText;
    public TextMeshProUGUI yawRightText;
    public TextMeshProUGUI forwardText;
    public TextMeshProUGUI backwardText;
    public TextMeshProUGUI strafeLeftText;
    public TextMeshProUGUI strafeRightText;
    public TextMeshProUGUI switchViewText;

    [Header("UI: Acro дрон")]
    public TextMeshProUGUI acroUpText;
    public TextMeshProUGUI acroDownText;
    public TextMeshProUGUI acroForwardText;
    public TextMeshProUGUI acroBackwardText;
    public TextMeshProUGUI acroRollLeftText;
    public TextMeshProUGUI acroRollRightText;
    public TextMeshProUGUI acroYawLeftText;
    public TextMeshProUGUI acroYawRightText;

    private string actionToRebind = "";

    void Start()
    {
        UpdateUI();
    }

    void Update()
    {
        if (actionToRebind != "")
        {
            if (Input.anyKeyDown)
            {
                foreach (KeyCode k in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(k))
                    {
                        SaveNewKey(k);
                        break;
                    }
                }
            }
        }
    }

    // --- МЕТОДЫ ДЛЯ ОБЫЧНОГО ДРОНА ---
    public void StartRebindUp() { actionToRebind = "Up"; if(upText) upText.text = "..."; }
    public void StartRebindDown() { actionToRebind = "Down"; if(downText) downText.text = "..."; }
    public void StartRebindYawLeft() { actionToRebind = "YawLeft"; if(yawLeftText) yawLeftText.text = "..."; }
    public void StartRebindYawRight() { actionToRebind = "YawRight"; if(yawRightText) yawRightText.text = "..."; }
    public void StartRebindForward() { actionToRebind = "Forward"; if(forwardText) forwardText.text = "..."; }
    public void StartRebindBackward() { actionToRebind = "Backward"; if(backwardText) backwardText.text = "..."; }
    public void StartRebindStrafeLeft() { actionToRebind = "StrafeLeft"; if(strafeLeftText) strafeLeftText.text = "..."; }
    public void StartRebindStrafeRight() { actionToRebind = "StrafeRight"; if(strafeRightText) strafeRightText.text = "..."; }
    public void StartRebindSwitchView() { actionToRebind = "SwitchView"; if(switchViewText) switchViewText.text = "..."; }

    // --- МЕТОДЫ ДЛЯ ACRO ДРОНА ---
    public void StartRebindAcroUp() { actionToRebind = "AcroUp"; if(acroUpText) acroUpText.text = "..."; }
    public void StartRebindAcroDown() { actionToRebind = "AcroDown"; if(acroDownText) acroDownText.text = "..."; }
    public void StartRebindAcroForward() { actionToRebind = "AcroForward"; if(acroForwardText) acroForwardText.text = "..."; }
    public void StartRebindAcroBackward() { actionToRebind = "AcroBackward"; if(acroBackwardText) acroBackwardText.text = "..."; }
    public void StartRebindAcroRollLeft() { actionToRebind = "AcroRollLeft"; if(acroRollLeftText) acroRollLeftText.text = "..."; }
    public void StartRebindAcroRollRight() { actionToRebind = "AcroRollRight"; if(acroRollRightText) acroRollRightText.text = "..."; }
    public void StartRebindAcroYawLeft() { actionToRebind = "AcroYawLeft"; if(acroYawLeftText) acroYawLeftText.text = "..."; }
    public void StartRebindAcroYawRight() { actionToRebind = "AcroYawRight"; if(acroYawRightText) acroYawRightText.text = "..."; }

    private void SaveNewKey(KeyCode newKey)
    {
        // === Сохранение для обычного дрона ===
        if (actionToRebind == "Up") { if(drone) drone.thrustUpKey = newKey; PlayerPrefs.SetInt("key_up", (int)newKey); }
        else if (actionToRebind == "Down") { if(drone) drone.thrustDownKey = newKey; PlayerPrefs.SetInt("key_down", (int)newKey); }
        else if (actionToRebind == "YawLeft") { if(drone) drone.yawLeftKey = newKey; PlayerPrefs.SetInt("key_left", (int)newKey); }
        else if (actionToRebind == "YawRight") { if(drone) drone.yawRightKey = newKey; PlayerPrefs.SetInt("key_right", (int)newKey); }
        else if (actionToRebind == "Forward") { if(drone) drone.pitchForwardKey = newKey; PlayerPrefs.SetInt("key_forward", (int)newKey); }
        else if (actionToRebind == "Backward") { if(drone) drone.pitchBackwardKey = newKey; PlayerPrefs.SetInt("key_backward", (int)newKey); }
        else if (actionToRebind == "StrafeLeft") { if(drone) drone.rollLeftKey = newKey; PlayerPrefs.SetInt("key_strafe_left", (int)newKey); }
        else if (actionToRebind == "StrafeRight") { if(drone) drone.rollRightKey = newKey; PlayerPrefs.SetInt("key_strafe_right", (int)newKey); }
        else if (actionToRebind == "SwitchView") { if(drone) drone.switchViewKey = newKey; PlayerPrefs.SetInt("key_switch_view", (int)newKey); }

        // === Сохранение для Acro дрона ===
        else if (actionToRebind == "AcroUp") { if(acroDrone) acroDrone.thrustUpKey = newKey; PlayerPrefs.SetInt("acro_thrust_up", (int)newKey); }
        else if (actionToRebind == "AcroDown") { if(acroDrone) acroDrone.thrustDownKey = newKey; PlayerPrefs.SetInt("acro_thrust_down", (int)newKey); }
        else if (actionToRebind == "AcroForward") { if(acroDrone) acroDrone.pitchForwardKey = newKey; PlayerPrefs.SetInt("acro_pitch_fwd", (int)newKey); }
        else if (actionToRebind == "AcroBackward") { if(acroDrone) acroDrone.pitchBackwardKey = newKey; PlayerPrefs.SetInt("acro_pitch_back", (int)newKey); }
        else if (actionToRebind == "AcroRollLeft") { if(acroDrone) acroDrone.rollLeftKey = newKey; PlayerPrefs.SetInt("acro_roll_left", (int)newKey); }
        else if (actionToRebind == "AcroRollRight") { if(acroDrone) acroDrone.rollRightKey = newKey; PlayerPrefs.SetInt("acro_roll_right", (int)newKey); }
        else if (actionToRebind == "AcroYawLeft") { if(acroDrone) acroDrone.yawLeftKey = newKey; PlayerPrefs.SetInt("acro_yaw_left", (int)newKey); }
        else if (actionToRebind == "AcroYawRight") { if(acroDrone) acroDrone.yawRightKey = newKey; PlayerPrefs.SetInt("acro_yaw_right", (int)newKey); }

        PlayerPrefs.Save();
        actionToRebind = "";
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (drone != null)
        {
            if(upText) upText.text = drone.thrustUpKey.ToString();
            if(downText) downText.text = drone.thrustDownKey.ToString();
            if(yawLeftText) yawLeftText.text = drone.yawLeftKey.ToString();
            if(yawRightText) yawRightText.text = drone.yawRightKey.ToString();
            if(forwardText) forwardText.text = drone.pitchForwardKey.ToString();
            if(backwardText) backwardText.text = drone.pitchBackwardKey.ToString();
            if(strafeLeftText) strafeLeftText.text = drone.rollLeftKey.ToString();
            if(strafeRightText) strafeRightText.text = drone.rollRightKey.ToString();
            if(switchViewText) switchViewText.text = drone.switchViewKey.ToString();
        }

        if (acroDrone != null)
        {
            if(acroUpText) acroUpText.text = acroDrone.thrustUpKey.ToString();
            if(acroDownText) acroDownText.text = acroDrone.thrustDownKey.ToString();
            if(acroForwardText) acroForwardText.text = acroDrone.pitchForwardKey.ToString();
            if(acroBackwardText) acroBackwardText.text = acroDrone.pitchBackwardKey.ToString();
            if(acroRollLeftText) acroRollLeftText.text = acroDrone.rollLeftKey.ToString();
            if(acroRollRightText) acroRollRightText.text = acroDrone.rollRightKey.ToString();
            if(acroYawLeftText) acroYawLeftText.text = acroDrone.yawLeftKey.ToString();
            if(acroYawRightText) acroYawRightText.text = acroDrone.yawRightKey.ToString();
        }
    }
}