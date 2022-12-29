using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;

/*
 * Description : Gérer le mouvement du curseur pour être adaptatible avec un gamepad
 * Fait par : Tristan Lapointe
 */

public class GamePadCursor : MonoBehaviour
{
    public PlayerInput playerInput; //Inputs émis par le joueur
    public Mouse virtualMouse; //Référence à notre souris virtuelle
    private Mouse currentMouse; //Reference a notre souris/type de souris actuelle
    public RectTransform cursorTransform; //Position du curseur
    [SerializeField] private Canvas canvas; //Canvas
    [SerializeField] private RectTransform canvasTransform; //Position du canvas
    [SerializeField] private float sensibilite; //Vitesse de notre curseur
    [SerializeField] private float padding; //padding du curseur
    private bool previousMouseState; //Permet d'enregistrer l'état précédent du curseur
    private Camera mainCamera; //Reference a notre camera
    [HideInInspector] public string previousControlScheme = ""; //Enregistrer l'état actuel de notre control scheme
    [HideInInspector] public const string gamepadScheme = "Gamepad"; //Nom de notre plan de gamepad
    [HideInInspector] public const string mouseScheme = "Keyboard&Mouse"; //Nom de notre plan de clavier souris
    public Texture2D cursorTexture; //Texture du curseur
    public Texture2D blankCursor; //Texture du curseur

    private void Start()
    {
        //Faire que la texture transparente du curseur est réellement transparente
        //Ceci est un bug fix pour l'éditeur de unity
        #if UNITY_EDITOR
        Color32[] pixels = blankCursor.GetPixels32();
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = new Color32(0, 0, 0, 0);
        }
        blankCursor.SetPixels32(pixels);
        blankCursor.Apply();
        #endif
    }
    //Lorsque le script est activé
    private void OnEnable()
    {
        //Assigner notre souris de départ
        currentMouse = Mouse.current;

        //Assigner la reference a notre camera
        mainCamera = Camera.main;

        //Si on n'a pas de souris,
        if (virtualMouse == null)
        {
            //Ajouter une souris
            virtualMouse = (Mouse)InputSystem.AddDevice("VirtualMouse");
        }
        //Si ce n'est pas null, mais que nous en avons une
        else if (!virtualMouse.added)
        {
            //Ajouter la souris préexistante
            InputSystem.AddDevice(virtualMouse);
        }
        //Lier notre souris avec l'input du joueur
        InputUser.PerformPairingWithDevice(virtualMouse, playerInput.user);

        //S'assurer que nous souris est à la bonne position
        if(cursorTransform != null)
        {
            //Aller chercher la position
            Vector2 position = cursorTransform.anchoredPosition;

            //Changer l'état de la souris avec notre nouvelle position
            InputState.Change(virtualMouse.position, position);
        }

        //Ajouter notre motion de souris après chaque update
        InputSystem.onAfterUpdate += UpdateMotion;
        playerInput.onControlsChanged += OnControlsChanged;
    }

    //Lorsque le script est disabled
    private void OnDisable()
    {
        //Si nous avions une souris virtuelle
        if(virtualMouse != null && virtualMouse.added)
        {
            //Enlever le device du user
            playerInput.user.UnpairDevice(virtualMouse);

            //Enlever le device
            InputSystem.RemoveDevice(virtualMouse);
        }

        //Enlever notre motion de souris après chaque update
        InputSystem.onAfterUpdate -= UpdateMotion;
        playerInput.onControlsChanged -= OnControlsChanged;
    }

    private void UpdateMotion()
    {
        //Si nous avons pas de souris virtuelle ou de gamepad
        if(virtualMouse == null || Gamepad.current == null)
        {
            //S'en aller
            return;
        }

        //Lire la valeur du joystick droit de notre gamepad
        Vector2 deltaValue = Gamepad.current.rightStick.ReadValue();

        //La multiplier par notre sensibilite
        deltaValue *= sensibilite * Time.deltaTime;

        //Calculer la position actuelle
        Vector2 currentPosition = virtualMouse.position.ReadValue();

        //Calculer la nouvelle position
        Vector2 newPosition = currentPosition + deltaValue;

        //Clamp la position dans l'écran
        newPosition.x = Mathf.Clamp(newPosition.x, padding, Screen.width - padding);
        newPosition.y = Mathf.Clamp(newPosition.y, padding, Screen.height - padding);

        //Changer l'état de notre input
        InputState.Change(virtualMouse.position, newPosition);
        InputState.Change(virtualMouse.delta, deltaValue);

        //Créer une variable détectant si nous avons appuyé sur le bouton a du joystick
        bool aButtonIsPressed = Gamepad.current.aButton.IsPressed();

        //Si notre ancien état bouton n'était pas appuyé
        if(previousMouseState != aButtonIsPressed)
        {
            //Émettre une variable copiant l'état de notre souris virtuelle
            virtualMouse.CopyState<MouseState>(out var mouseState);

            //Changer l'état du bouton que nous appuyons
            mouseState.WithButton(MouseButton.Left, aButtonIsPressed);

            //Changer l'état de notre souris
            InputState.Change(virtualMouse, mouseState);

            //Storer notre état précédent
            previousMouseState = aButtonIsPressed;
        }

        //Changer la position du curseur
        AnchorCursor(newPosition);
    }

    //Fonction permettant de changer la position du curseur 
    private void AnchorCursor(Vector2 position)
    {
        //Variable de notre position
        Vector2 anchoredPosition;

        //Changer la position du curseur selon la taille du canvas de l'écran ainsi que son mode
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasTransform, position, canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera, out anchoredPosition);
        cursorTransform.anchoredPosition = anchoredPosition;
    }

    //Fonction permettant de changer entre nos types de contrôles
    private void OnControlsChanged(PlayerInput input)
    {
        //Checker si nos contrôleurs sont différents que les anciens
        //Si on a la souris
        if(playerInput.currentControlScheme == mouseScheme && previousControlScheme != mouseScheme)
        {
            //Disable le curseur du gamepad
            cursorTransform.gameObject.SetActive(false);
            
            //Changer la position de notre souris actuelle
            currentMouse.WarpCursorPosition(virtualMouse.position.ReadValue());

            //Enregistrer l'état
            previousControlScheme = mouseScheme;

            //Activer notre curseur de souris
            Cursor.visible = true;

            //Changer la texture lorsque nous sommes dans l'éditeur
            #if UNITY_EDITOR
            Cursor.SetCursor(cursorTexture, new Vector2(cursorTexture.width / 2, cursorTexture.height / 2), CursorMode.Auto);
            #endif
        }
        //Si on a le gamepad
        else if (playerInput.currentControlScheme == gamepadScheme && previousControlScheme != gamepadScheme)
        {
            //Enable le curseur du gamepad
            cursorTransform.gameObject.SetActive(true);

            //Changer l'état de l'input pour que la transition soit fluide
            InputState.Change(virtualMouse.position, currentMouse.position.ReadValue());
            AnchorCursor(currentMouse.position.ReadValue());

            //Enregistrer l'état
            previousControlScheme = gamepadScheme;

            //Disable les visuels de notre souris
            Cursor.visible = false;

            //Changer la texture lorsque nous sommes dans l'éditeur
            #if UNITY_EDITOR
            Cursor.SetCursor(blankCursor, Vector2.zero, CursorMode.Auto);
            #endif
        }
    }

    private void Update()
    {
        //Si jamais nous enregistrons pas le changement de contrôles
        if(previousControlScheme != playerInput.currentControlScheme)
        {
            OnControlsChanged(playerInput);
        }
        previousControlScheme = playerInput.currentControlScheme;
    }
}
