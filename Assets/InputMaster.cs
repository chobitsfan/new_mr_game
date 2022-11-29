// GENERATED AUTOMATICALLY FROM 'Assets/InputMaster.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @InputMaster : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputMaster()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputMaster"",
    ""maps"": [
        {
            ""name"": ""player"",
            ""id"": ""90b5f0ee-f77a-480e-a389-bcd9c5c24dfb"",
            ""actions"": [
                {
                    ""name"": ""ThrottleYaw"",
                    ""type"": ""PassThrough"",
                    ""id"": ""c04dee8c-82af-460a-b851-b92ef2923bf2"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""PitchRoll"",
                    ""type"": ""PassThrough"",
                    ""id"": ""73263c04-108d-4b16-bb34-2983a86c70a4"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Takeoff"",
                    ""type"": ""Button"",
                    ""id"": ""f501f11d-ec13-4e61-9411-8db34e041a5b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Shot"",
                    ""type"": ""Button"",
                    ""id"": ""d1a7289f-c938-4438-acf5-de48409e1c53"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Land"",
                    ""type"": ""Button"",
                    ""id"": ""a39a1460-c52e-4429-a109-68113fc5e611"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""AutoShot"",
                    ""type"": ""Button"",
                    ""id"": ""7b7f98a7-a945-4a8d-aca2-0872911502b5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""6a4b39f7-2190-4be8-9d9d-723ce850c604"",
                    ""path"": ""<XInputController>/rightStick"",
                    ""interactions"": """",
                    ""processors"": ""StickDeadzone"",
                    ""groups"": ""Xbox"",
                    ""action"": ""ThrottleYaw"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""ee20be69-dfdd-4064-94f0-68b1f2325f5c"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ThrottleYaw"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""6524aabd-35ab-4205-8956-ffc0c94dd68d"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ThrottleYaw"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""cafb56da-9c61-4c77-8d48-5ce574353421"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ThrottleYaw"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""53eb2a77-250c-424c-a6ff-edf8d871f156"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ThrottleYaw"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""667c0581-d13d-44bb-8d92-37167666bd1b"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ThrottleYaw"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""ae50e64f-6ebf-40a5-b817-9e211dc4dade"",
                    ""path"": ""<XInputController>/leftStick"",
                    ""interactions"": """",
                    ""processors"": ""StickDeadzone"",
                    ""groups"": ""Xbox"",
                    ""action"": ""PitchRoll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""991525d6-cd28-48f6-929a-9cbbaf630120"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PitchRoll"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""6a2038cd-016f-401b-a191-66b3ddf8363a"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PitchRoll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""a9e8a7a7-96ae-4d71-81f7-f93d29c4b4fe"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PitchRoll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""fbc298d5-900f-43f3-b922-19dff7ddd4a1"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PitchRoll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""619012db-3194-412c-b50b-ec25d72daef9"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PitchRoll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""2d1f2ce4-174c-4186-a555-eab4421679bd"",
                    ""path"": ""<XInputController>/rightTrigger"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": ""Xbox"",
                    ""action"": ""Takeoff"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""dcd8e2bf-6ef1-4b40-bbba-a61bc5820d8a"",
                    ""path"": ""<XInputController>/buttonEast"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": ""Xbox"",
                    ""action"": ""Shot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4c76f86a-9752-4bb2-9433-80b88790d617"",
                    ""path"": ""<XInputController>/buttonSouth"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": ""Xbox"",
                    ""action"": ""Shot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""89912247-59be-4a80-a5e2-b982ad1d6b23"",
                    ""path"": ""<XInputController>/buttonNorth"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": ""Xbox"",
                    ""action"": ""Shot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""85b51aa0-cacc-421b-af8d-2a354e7c3b28"",
                    ""path"": ""<XInputController>/buttonWest"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": ""Xbox"",
                    ""action"": ""Shot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""91bd1939-e53e-42c6-aee6-d3e2ea823258"",
                    ""path"": ""<XInputController>/leftTrigger"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": ""Xbox"",
                    ""action"": ""Land"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ee38cb15-2664-48b2-a84c-450ef803a351"",
                    ""path"": ""<XInputController>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Xbox"",
                    ""action"": ""AutoShot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Xbox"",
            ""bindingGroup"": ""Xbox"",
            ""devices"": [
                {
                    ""devicePath"": ""<XInputController>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // player
        m_player = asset.FindActionMap("player", throwIfNotFound: true);
        m_player_ThrottleYaw = m_player.FindAction("ThrottleYaw", throwIfNotFound: true);
        m_player_PitchRoll = m_player.FindAction("PitchRoll", throwIfNotFound: true);
        m_player_Takeoff = m_player.FindAction("Takeoff", throwIfNotFound: true);
        m_player_Shot = m_player.FindAction("Shot", throwIfNotFound: true);
        m_player_Land = m_player.FindAction("Land", throwIfNotFound: true);
        m_player_AutoShot = m_player.FindAction("AutoShot", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // player
    private readonly InputActionMap m_player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_player_ThrottleYaw;
    private readonly InputAction m_player_PitchRoll;
    private readonly InputAction m_player_Takeoff;
    private readonly InputAction m_player_Shot;
    private readonly InputAction m_player_Land;
    private readonly InputAction m_player_AutoShot;
    public struct PlayerActions
    {
        private @InputMaster m_Wrapper;
        public PlayerActions(@InputMaster wrapper) { m_Wrapper = wrapper; }
        public InputAction @ThrottleYaw => m_Wrapper.m_player_ThrottleYaw;
        public InputAction @PitchRoll => m_Wrapper.m_player_PitchRoll;
        public InputAction @Takeoff => m_Wrapper.m_player_Takeoff;
        public InputAction @Shot => m_Wrapper.m_player_Shot;
        public InputAction @Land => m_Wrapper.m_player_Land;
        public InputAction @AutoShot => m_Wrapper.m_player_AutoShot;
        public InputActionMap Get() { return m_Wrapper.m_player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @ThrottleYaw.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnThrottleYaw;
                @ThrottleYaw.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnThrottleYaw;
                @ThrottleYaw.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnThrottleYaw;
                @PitchRoll.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPitchRoll;
                @PitchRoll.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPitchRoll;
                @PitchRoll.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPitchRoll;
                @Takeoff.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTakeoff;
                @Takeoff.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTakeoff;
                @Takeoff.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTakeoff;
                @Shot.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnShot;
                @Shot.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnShot;
                @Shot.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnShot;
                @Land.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLand;
                @Land.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLand;
                @Land.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLand;
                @AutoShot.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAutoShot;
                @AutoShot.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAutoShot;
                @AutoShot.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAutoShot;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @ThrottleYaw.started += instance.OnThrottleYaw;
                @ThrottleYaw.performed += instance.OnThrottleYaw;
                @ThrottleYaw.canceled += instance.OnThrottleYaw;
                @PitchRoll.started += instance.OnPitchRoll;
                @PitchRoll.performed += instance.OnPitchRoll;
                @PitchRoll.canceled += instance.OnPitchRoll;
                @Takeoff.started += instance.OnTakeoff;
                @Takeoff.performed += instance.OnTakeoff;
                @Takeoff.canceled += instance.OnTakeoff;
                @Shot.started += instance.OnShot;
                @Shot.performed += instance.OnShot;
                @Shot.canceled += instance.OnShot;
                @Land.started += instance.OnLand;
                @Land.performed += instance.OnLand;
                @Land.canceled += instance.OnLand;
                @AutoShot.started += instance.OnAutoShot;
                @AutoShot.performed += instance.OnAutoShot;
                @AutoShot.canceled += instance.OnAutoShot;
            }
        }
    }
    public PlayerActions @player => new PlayerActions(this);
    private int m_XboxSchemeIndex = -1;
    public InputControlScheme XboxScheme
    {
        get
        {
            if (m_XboxSchemeIndex == -1) m_XboxSchemeIndex = asset.FindControlSchemeIndex("Xbox");
            return asset.controlSchemes[m_XboxSchemeIndex];
        }
    }
    public interface IPlayerActions
    {
        void OnThrottleYaw(InputAction.CallbackContext context);
        void OnPitchRoll(InputAction.CallbackContext context);
        void OnTakeoff(InputAction.CallbackContext context);
        void OnShot(InputAction.CallbackContext context);
        void OnLand(InputAction.CallbackContext context);
        void OnAutoShot(InputAction.CallbackContext context);
    }
}
