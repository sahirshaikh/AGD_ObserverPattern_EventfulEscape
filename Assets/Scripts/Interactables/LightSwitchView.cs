using System;
using System.Collections.Generic;
using UnityEditor.MPE;
using UnityEngine;

public class LightSwitchView : MonoBehaviour, IInteractable
{
    [SerializeField] private List<Light> lightsources = new List<Light>();
    private SwitchState currentState;

    private void OnEnable()
    {
        EventService.Instance.OnLightSwitchToggled.AddListener(OnLightSwitch);
        EventService.Instance.OnLightsOffByGhostEvent.AddListener(OnLightSwitchOFF);
    }

    private void OnDisable() {
        EventService.Instance.OnLightSwitchToggled.RemoveListener(OnLightSwitch);
        EventService.Instance.OnLightsOffByGhostEvent.RemoveListener(OnLightSwitchOFF);
    }


    private void Start() => currentState = SwitchState.Off;

    public void Interact() => EventService.Instance.OnLightSwitchToggled.InvokeEvent();

    private void toggleLights()
    {
        bool lights = false;

        switch (currentState)
        {
            case SwitchState.On:
                currentState = SwitchState.Off;
                lights = false;
                break;
            case SwitchState.Off:
                currentState = SwitchState.On;
                lights = true;
                break;
            case SwitchState.Unresponsive:
                break;
        }
        foreach (Light lightSource in lightsources)
        {
            lightSource.enabled = lights;
        }
    }

    private void OnLightSwitch()
    {
        toggleLights();
        GameService.Instance.GetSoundView().PlaySoundEffects(SoundType.SwitchSound);
        GameService.Instance.GetInstructionView().HideInstruction();
    }

    private void OnLightSwitchOFF()
    {
        LightsOff(false);
        GameService.Instance.GetSoundView().PlaySoundEffects(SoundType.SwitchSound);
        GameService.Instance.GetInstructionView().ShowInstruction(InstructionType.LightsOff);
    }

    private void LightsOff(bool lights)
    {
        foreach (Light lightSource in lightsources)
        {
            lightSource.enabled = lights;
        }
        if (lights)
            currentState = SwitchState.On;
        else 
            currentState = SwitchState.Off;


    }
}
