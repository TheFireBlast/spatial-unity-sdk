using System.Collections.Generic;
using SpatialSys.UnitySDK;
using SpatialSys.UnitySDK.Editor;
using SpatialSys.UnitySDK.VisualScripting;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditorInteractableManager : MonoBehaviour
{
    public static EditorInteractableManager INSTANCE;

    private HashSet<SpatialInteractable> ints = new();
    private SpatialInteractable activeInteractable = null;
    private List<InteractableUIElement> iconPool = new();

    private TextMeshProUGUI interactText;

    private const int MAX_ICONS = 50;

    private class InteractableUIElement
    {
        public bool active = false;
        public RawImage icon;
        public SpatialInteractable interactable;
        public float distance;
        public InteractableUIElement(RawImage icon)
        {
            this.icon = icon;
        }
        public void UpdatePosition()
        {
            icon.rectTransform.position = Camera.main.WorldToScreenPoint(interactable.transform.position);

            if (!IsVisible())
            {
                active = false;
            }
        }
        public bool IsVisible()
        {
            return active && icon.rectTransform.position.z >= 0;
        }
    }

    void OnEnable()
    {
        INSTANCE = this;

        // Make sure no interactables were initialized before this script
        RegenerateList();

        // Create canvas
        var canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        // Create interactable text display
        var textObject = new GameObject("Text");
        textObject.transform.SetParent(transform);
        interactText = textObject.AddComponent<TextMeshProUGUI>();
        interactText.fontSize = 12f;
        interactText.alignment = TextAlignmentOptions.MidlineLeft;
        interactText.rectTransform.sizeDelta = new Vector3(120f, 50f);
        interactText.rectTransform.pivot = new Vector2(0, 0.5f);
        textObject.SetActive(false);

        //TODO: add background to selected interactable

        // Instantiate a few icons into the pool
        for (int i = 0; i < 10; i++)
        {
            InstantiateIcon();
        }
    }

    InteractableUIElement InstantiateIcon()
    {
        var icon = new GameObject("Icon").AddComponent<RawImage>();
        icon.rectTransform.sizeDelta = new Vector2(20, 20);
        icon.transform.SetParent(transform);
        var el = new InteractableUIElement(icon);
        iconPool.Add(el);
        return el;
    }

    public void RegenerateList()
    {
        ints = new(FindObjectsOfType<SpatialInteractable>(true));
    }

    public void Register(SpatialInteractable interactable)
    {
        ints.Add(interactable);
    }

    void LateUpdate()
    {
        List<(SpatialInteractable i, float d)> needAllocation = new();

        var playerPos = SpatialBridge.actorService.localActor.avatar.position;
        foreach (var i in ints)
        {
            float distance = Vector3.Distance(playerPos, i.transform.position);
            if (distance <= i.visibilityRadius && i.isActiveAndEnabled)
            {
                needAllocation.Add((i, distance));
            }
        }

        needAllocation.Sort((a, b) => a.d.CompareTo(b.d));

        // Mark all icons as inactive by default
        foreach (var entry in iconPool)
        {
            entry.active = false;
        }

        // Reuse previous interactables
        needAllocation.RemoveAll(tuple =>
        {
            var entry = iconPool.Find(entry => entry.interactable == tuple.i);
            if (entry != null)
            {
                entry.active = true;
                entry.distance = tuple.d;
                entry.UpdatePosition();
                return entry.IsVisible();
            }
            return false;
        });

        // Initialize and update icons
        foreach (var tuple in needAllocation)
        {
            var entry = iconPool.FindLast(entry => !entry.active);
            if (entry == null && iconPool.Count < MAX_ICONS)
                entry = InstantiateIcon();
            if (entry == null)
                entry = iconPool.FindLast(entry => tuple.d < entry.distance);
            if (entry == null) break;

            entry.active = true;
            entry.distance = tuple.d;
            entry.interactable = tuple.i;
            entry.UpdatePosition();
            if (!entry.IsVisible())
            {
                entry.active = false;
                continue;
            }
            entry.icon.texture = LoadIconTexture(entry.interactable.iconType);
        }

        // Position and get active interactable
        activeInteractable = null;
        float closestDistance = float.MaxValue;
        foreach (var entry in iconPool)
        {
            if (!entry.active) continue;

            if (entry.distance <= entry.interactable.interactiveRadius)
            {
                float mouseDistance = Vector2.Distance(Input.mousePosition, entry.icon.rectTransform.position);
                if (mouseDistance < 60f && mouseDistance < closestDistance)
                {
                    closestDistance = mouseDistance;
                    activeInteractable = entry.interactable;
                }
            }
        }

        // Display interactables
        foreach (var entry in iconPool)
        {
            entry.icon.enabled = entry.active;
            if (!entry.active) continue;

            var c = entry.icon.color;
            if (activeInteractable == entry.interactable)
                c.a = 0.9f;
            else if (entry.distance <= entry.interactable.interactiveRadius)
                c.a = 0.6f;
            else
                c.a = 0.2f;
            entry.icon.color = c;
        }

        // Display interact text
        if (activeInteractable != null)
        {
            interactText.gameObject.SetActive(true);
            interactText.text = activeInteractable.interactText;
            var icon = iconPool.Find(entry => entry.active && entry.interactable == activeInteractable);
            interactText.rectTransform.position = icon.icon.rectTransform.position + new Vector3(15f, 0, 0);
        }
        else
        {
            interactText.gameObject.SetActive(false);
        }

        // Handle interaction
        if (Input.GetKeyDown("f") && activeInteractable != null)
        {
            activeInteractable.onInteractEvent.runtimeEvent?.Invoke();
            if (activeInteractable.onInteractEvent.hasUnityEvent)
                activeInteractable.onInteractEvent.unityEvent.Invoke();
            SpatialInteractableOnInteract.TriggerEvent(activeInteractable);
        }
    }

    Texture2D LoadIconTexture(SpatialInteractable.IconType iconType)
    {
        if (iconType == SpatialInteractable.IconType.None)
            return SpatialGUIUtility.LoadGUITexture("Icons/icon_interactable.png");
        return SpatialGUIUtility.LoadGUITexture($"InteractableIcons/{iconType.ToString().ToLower()}.png");
    }

    void Reset()
    {
        hideFlags = HideFlags.DontSaveInBuild;
    }
}