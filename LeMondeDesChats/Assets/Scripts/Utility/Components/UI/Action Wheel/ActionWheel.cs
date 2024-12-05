using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionWheel : MonoBehaviour
{
    public ActionWheelSelectable Current { get; private set; }
    public bool Active { get; private set; }

    //[Tooltip("Rect used as reference for mouse input. Usually Canvas.")]
    //[SerializeField] private bool center = true;
    [SerializeField] private List<GameObject> options;
    [SerializeField, Range(0, 1)] private float selectionThreshold = 0.5f;
    [SerializeField] private InputType inputType;
    [SerializeField] private bool clampCursorInput = true;
    [Tooltip("Keep the selection when nothing is selected.")]
    [SerializeField] private bool keepSelection = true;
    [SerializeField] private TriggerOption trigger;
    [SerializeField] private bool closeOnTrigger = true;
    [SerializeField] private GameObject content;

    [Header("Mouse")]
    [SerializeField] private bool hideCursorWhenActivated = true;

    [Header("Debug key Behaviour")]
    [SerializeField] private bool debugKeyBehaviour = false;
    [SerializeField] private KeyCode key = KeyCode.Tab;
    [SerializeField] private KeyCode controllerKey = KeyCode.JoystickButton0;
    [SerializeField] private string horizontalAxis = "Horizontal";
    [SerializeField] private string verticalAxis = "Vertical";

    [Space]
    [SerializeField] private ArticulationDriveAxis gizmoAxis;

    [Header("Sorting")]
    [SerializeField] private float circleRadius = 200;
    [SerializeField] private float startingAngle = 0;
    [SerializeField] private bool antiHoraire;

    private Vector2 _resolution => _referenceRectTransform.rect.size;

    private Vector2 _startCursorPosition;
    private Vector2 _cursorPosition;
    private ActionWheelSelectable _oldSelection;

    private RectTransform _referenceRectTransform;
    private RectTransform _rectTransform;

    private bool _lastFrameActiveState;

    enum TriggerOption { OnReleaseButton, OnStartDirection, OnReleaseDirection }

    enum InputType { Mouse, Joystick }

    private void Awake()
    {
        _referenceRectTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        _rectTransform = GetComponent<RectTransform>();

        content?.SetActive(false);
    }

    private void Update()
    {
        HandleDebugKeyBehaviour();
        HandleMouse();
        SelectItem();
        //HandleActive();
    }

    private void HandleMouse()
    {
        if (!Active)
            return;

        _cursorPosition = Input.mousePosition.ToVector2() - _startCursorPosition;

        if (clampCursorInput)
            _cursorPosition = Vector2.ClampMagnitude(_cursorPosition, circleRadius);

        //_cursorPosition.x = Input.mousePosition.x / Screen.width;
        //_cursorPosition.y = Input.mousePosition.y / Screen.height;
        
        //_cursorPosition *= _resolution;

        //if (center)
        //{
            //_cursorPosition -= _resolution / 2;
        //}
        //_cursorPosition -= transform.localPosition.ToVector2();
        //_cursorPosition -= _rectTransform.localPosition.ToVector2() + (_rectTransform.rect.center * GetScale(transform.localScale));
        //Debug.Log(_cursorPosition);
    }

    private void SelectItem()
    {
        if (!Active)
            return;

        if (Current != null)
            _oldSelection = Current;


        // Dot Check to find closest
        GameObject closestItem = null;
        float closestDot = -2f;

        var direction = GetInputDirection();

        if (direction.magnitude >= selectionThreshold)
        {
            foreach (var item in options)
            {
                //var distance = Vector3.Distance(item.transform.localPosition, direction * circleRadius);
                var dot = Vector3.Dot(direction, item.transform.localPosition.normalized);

                var isCloser = dot > closestDot;
                closestDot = isCloser ? dot : closestDot;
                closestItem = isCloser ? item : closestItem;
            }
        }

        // Set currently selected and keep selection if no selected
        if (closestItem != null)
        {
            Current = closestItem.GetComponent<ActionWheelSelectable>();
        }
        else if (!keepSelection)
        {
            Current = null;
        }

        // Visual Select and Deselect
        if (Current != _oldSelection)
        {
            _oldSelection?.OnDeselect(new BaseEventData(null));
            Debug.Log($"OnDeselect : {_oldSelection}");

            Current?.OnSelect(new BaseEventData(null));
            Debug.Log($"OnSelect : {Current}");
        }

        // On start and release direction trigger checks
        if (trigger == TriggerOption.OnReleaseDirection && _oldSelection != null && closestItem == null)
        {
            CallAction(_oldSelection);
        }
        if (trigger == TriggerOption.OnStartDirection && _oldSelection == null && closestItem != null)
        {
            CallAction(Current);
        }
    }

    private void HandleActive()
    {
        //if (trigger == ActionWheelTriggerOption.OnReleaseButton && (Active != _lastFrameActiveState && !Active) && Current != null)
        //{
        //    CallAction(Current);
        //}

        //content?.SetActive(Active);

        _lastFrameActiveState = Active;
    }

    private void CallAction(ActionWheelSelectable target)
    {
        if (target == null)
            return;

        target.InvokeTrigger();

        if (closeOnTrigger)
        {
            Deactivate(allowTrigger: false);
        }
    }

    public void Activate()
    {
        if (Active)
            return;

        Current = null;
        Current = null;
        _oldSelection = null;

        _startCursorPosition = Input.mousePosition;
        ResetCursor();

        Active = true;

        if (inputType == InputType.Mouse && hideCursorWhenActivated)
            Cursor.visible = false;

        content?.SetActive(true);
        Debug.Log("open content");
    }
    public void Deactivate(bool allowTrigger = true)
    {
        if (!Active)
            return;

        if (allowTrigger && trigger == TriggerOption.OnReleaseButton && Current != null)
        {
            CallAction(Current);
        }

        Active = false;

        if (inputType == InputType.Mouse && hideCursorWhenActivated)
            Cursor.visible = true;

        content?.SetActive(false);
        Debug.Log("close content");
    }

    public void ResetCursor()
    {
        _cursorPosition = Vector2.zero;
    }

    private void HandleDebugKeyBehaviour()
    {
        if (debugKeyBehaviour)
        {
            if (inputType == InputType.Mouse ? Input.GetKeyDown(key) : Input.GetKeyDown(controllerKey))
                Activate();

            if (inputType == InputType.Mouse ? Input.GetKeyUp(key) : Input.GetKeyUp(controllerKey))
                Deactivate();
        }
    }

    private float GetScale(Vector3 scale)
    {
        switch (gizmoAxis)
        {
            default:
            case ArticulationDriveAxis.X:
                return transform.lossyScale.x;
            case ArticulationDriveAxis.Y:
                return transform.lossyScale.y;
            case ArticulationDriveAxis.Z:
                return transform.lossyScale.z;
        }
    }

    private Vector2 GetInputDirection(bool clampMagnitude = true)
    {
        Vector2 direction;
        if (inputType == InputType.Mouse)
            direction = _cursorPosition / (circleRadius * GetScale(transform.localScale));
        else
        {
            direction = new Vector2(Input.GetAxis(horizontalAxis), Input.GetAxis(verticalAxis));
        }

        if (clampMagnitude)
            direction = Vector2.ClampMagnitude(direction, 1f);

        return direction;
    }

    private void OnDrawGizmosSelected()
    {
        float scale = GetScale(transform.lossyScale);

        _rectTransform = GetComponent<RectTransform>();
        GizmosExtention.DrawWireCircle(_rectTransform.position.ToVector2() + (_rectTransform.rect.center * GetScale(transform.localScale)), circleRadius * selectionThreshold * scale);
    }

    private void OnValidate()
    {
        if (options.Count == 0)
            return;

        float angle = 360 / options.Count;
        float currentAngle = startingAngle;

        foreach (var item in options)
        {
            if (item == null)
                continue;

            Vector3 position = Quaternion.Euler(0, 0, currentAngle) * new Vector3(0, circleRadius, 0);
            item.transform.localPosition = position;

            if (antiHoraire)
                currentAngle += angle;
            else
                currentAngle -= angle;
        }
    }
}
