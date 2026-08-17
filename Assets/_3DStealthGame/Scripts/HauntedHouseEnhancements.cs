using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HauntedHouseEnhancements : MonoBehaviour
{
    public Transform player;
    public Transform waypointsRoot;
    public float sprintMultiplier = 1.7f;
    public float staminaDrainPerSecond = 0.35f;
    public float staminaRecoveryPerSecond = 0.22f;
    public float ghostThreatRange = 8f;
    public int collectibleGoal = 5;

    readonly List<HauntedCollectible> m_Collectibles = new List<HauntedCollectible>();
    readonly List<Light> m_GhostLights = new List<Light>();
    PlayerMovement m_PlayerMovement;
    float m_Stamina = 1f;
    float m_Threat;
    int m_Collected;
    GUIStyle m_LabelStyle;
    GUIStyle m_HeaderStyle;

    public int CollectedCount => m_Collected;
    public float Stamina01 => m_Stamina;
    public float Threat01 => m_Threat;

    void Start()
    {
        ResolveReferences();
        BuildWaypointPathVisuals();
        SpawnCollectibles();
        AddGhostVisuals();
    }

    void Update()
    {
        UpdateSprint(Time.deltaTime);
        UpdateThreat();
        AnimateCollectibles(Time.time);
    }

    void OnGUI()
    {
        EnsureStyles();

        const float panelWidth = 300f;
        Rect panel = new Rect(18f, 18f, panelWidth, 132f);
        GUI.color = new Color(0.03f, 0.02f, 0.05f, 0.82f);
        GUI.DrawTexture(panel, Texture2D.whiteTexture);
        GUI.color = Color.white;

        GUI.Label(new Rect(panel.x + 14f, panel.y + 10f, panelWidth - 28f, 28f), "Haunting Objectives", m_HeaderStyle);
        GUI.Label(new Rect(panel.x + 14f, panel.y + 42f, panelWidth - 28f, 24f), $"Memory orbs: {m_Collected}/{collectibleGoal}", m_LabelStyle);
        DrawMeter(new Rect(panel.x + 14f, panel.y + 70f, panelWidth - 28f, 14f), m_Stamina, new Color(0.3f, 0.75f, 1f), "stamina");
        DrawMeter(new Rect(panel.x + 14f, panel.y + 96f, panelWidth - 28f, 14f), m_Threat, new Color(1f, 0.22f, 0.32f), "ghosts");
    }

    public void Collect(HauntedCollectible collectible)
    {
        if (collectible == null || collectible.IsCollected)
        {
            return;
        }

        collectible.MarkCollected();
        m_Collected = Mathf.Min(collectibleGoal, m_Collected + 1);
    }

    public static float CalculateStamina(float current, bool wantsSprint, float deltaTime, float drainPerSecond, float recoveryPerSecond)
    {
        float change = wantsSprint ? -drainPerSecond : recoveryPerSecond;
        return Mathf.Clamp01(current + change * deltaTime);
    }

    public static float CalculateThreat01(float distance, float range)
    {
        if (range <= 0f)
        {
            return 0f;
        }

        return Mathf.Clamp01(1f - distance / range);
    }

    public static string GetWaypointRouteName(string waypointName)
    {
        if (string.IsNullOrEmpty(waypointName) || !waypointName.StartsWith("Waypoint_", StringComparison.Ordinal))
        {
            return string.Empty;
        }

        int secondSeparator = waypointName.IndexOf('_', "Waypoint_".Length);
        return secondSeparator < 0 ? waypointName : waypointName.Substring(0, secondSeparator);
    }

    void ResolveReferences()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.Find("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        if (player != null)
        {
            m_PlayerMovement = player.GetComponent<PlayerMovement>();
        }

        if (waypointsRoot == null)
        {
            GameObject waypointsObject = GameObject.Find("Waypoints");
            if (waypointsObject != null)
            {
                waypointsRoot = waypointsObject.transform;
            }
        }
    }

    void UpdateSprint(float deltaTime)
    {
        bool hasKeyboard = Keyboard.current != null;
        bool wantsSprint = hasKeyboard && Keyboard.current.leftShiftKey.isPressed && m_Stamina > 0.05f;
        m_Stamina = CalculateStamina(m_Stamina, wantsSprint, deltaTime, staminaDrainPerSecond, staminaRecoveryPerSecond);
        bool isSprinting = wantsSprint && m_Stamina > 0f;

        if (m_PlayerMovement != null)
        {
            m_PlayerMovement.SetSpeedMultiplier(isSprinting ? sprintMultiplier : 1f);
        }
    }

    void UpdateThreat()
    {
        if (player == null)
        {
            m_Threat = 0f;
            return;
        }

        float nearestDistance = float.PositiveInfinity;
        WaypointPatrol[] ghosts = FindObjectsByType<WaypointPatrol>(FindObjectsSortMode.None);
        for (int index = 0; index < ghosts.Length; index++)
        {
            nearestDistance = Mathf.Min(nearestDistance, Vector3.Distance(player.position, ghosts[index].transform.position));
        }

        m_Threat = float.IsPositiveInfinity(nearestDistance) ? 0f : CalculateThreat01(nearestDistance, ghostThreatRange);

        for (int index = 0; index < m_GhostLights.Count; index++)
        {
            if (m_GhostLights[index] != null)
            {
                m_GhostLights[index].intensity = Mathf.Lerp(0.75f, 2.4f, m_Threat);
            }
        }
    }

    void SpawnCollectibles()
    {
        if (player == null || collectibleGoal <= 0)
        {
            return;
        }

        Vector3[] positions = GetCollectiblePositions();
        for (int index = 0; index < Mathf.Min(collectibleGoal, positions.Length); index++)
        {
            GameObject orb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            orb.name = $"Memory Orb {index + 1}";
            orb.transform.position = positions[index];
            orb.transform.localScale = Vector3.one * 0.35f;

            Collider collider = orb.GetComponent<Collider>();
            collider.isTrigger = true;

            Renderer renderer = orb.GetComponent<Renderer>();
            renderer.sharedMaterial = CreateMaterial(new Color(0.18f, 0.95f, 1f, 0.85f));

            Light light = orb.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(0.26f, 0.9f, 1f);
            light.range = 2.5f;
            light.intensity = 1.3f;

            HauntedCollectible collectible = orb.AddComponent<HauntedCollectible>();
            collectible.Initialize(this, player);
            m_Collectibles.Add(collectible);
        }
    }

    Vector3[] GetCollectiblePositions()
    {
        if (waypointsRoot == null || waypointsRoot.childCount == 0)
        {
            return new[]
            {
                new Vector3(-3.5f, 2.35f, 1.0f),
                new Vector3(-8.9f, 2.35f, -2.5f),
                new Vector3(4.5f, 2.35f, 8.0f),
                new Vector3(7.7f, 2.35f, 8.0f),
                new Vector3(14.8f, 2.35f, 5.5f)
            };
        }

        List<Vector3> positions = new List<Vector3>();
        for (int index = 0; index < waypointsRoot.childCount && positions.Count < collectibleGoal; index++)
        {
            Transform waypoint = waypointsRoot.GetChild(index);
            positions.Add(waypoint.position + Vector3.up * 0.25f);
        }

        return positions.ToArray();
    }

    void BuildWaypointPathVisuals()
    {
        if (waypointsRoot == null)
        {
            return;
        }

        Dictionary<string, List<Transform>> routes = new Dictionary<string, List<Transform>>();
        for (int index = 0; index < waypointsRoot.childCount; index++)
        {
            Transform waypoint = waypointsRoot.GetChild(index);
            string routeName = GetWaypointRouteName(waypoint.name);
            if (string.IsNullOrEmpty(routeName))
            {
                continue;
            }

            if (!routes.TryGetValue(routeName, out List<Transform> route))
            {
                route = new List<Transform>();
                routes.Add(routeName, route);
            }

            route.Add(waypoint);
            CreateWaypointMarker(waypoint);
        }

        foreach (KeyValuePair<string, List<Transform>> route in routes)
        {
            route.Value.Sort(CompareWaypointOrder);
            CreateRouteLine(route.Key, route.Value);
        }
    }

    void CreateRouteLine(string routeName, List<Transform> route)
    {
        if (route.Count < 2)
        {
            return;
        }

        GameObject lineObject = new GameObject($"{routeName}_PatrolPath");
        lineObject.transform.SetParent(transform, false);

        LineRenderer line = lineObject.AddComponent<LineRenderer>();
        line.positionCount = route.Count;
        line.loop = true;
        line.widthMultiplier = 0.07f;
        line.material = CreateMaterial(new Color(0.45f, 0.95f, 1f, 0.7f));

        for (int index = 0; index < route.Count; index++)
        {
            line.SetPosition(index, route[index].position + Vector3.up * 0.08f);
        }
    }

    void CreateWaypointMarker(Transform waypoint)
    {
        GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        marker.name = $"{waypoint.name}_Marker";
        marker.transform.SetParent(transform, false);
        marker.transform.position = waypoint.position + Vector3.up * 0.08f;
        marker.transform.localScale = Vector3.one * 0.18f;
        marker.GetComponent<Collider>().enabled = false;
        marker.GetComponent<Renderer>().sharedMaterial = CreateMaterial(new Color(0.95f, 0.55f, 1f, 0.85f));
    }

    void AddGhostVisuals()
    {
        WaypointPatrol[] ghosts = FindObjectsByType<WaypointPatrol>(FindObjectsSortMode.None);
        for (int index = 0; index < ghosts.Length; index++)
        {
            Light light = ghosts[index].GetComponent<Light>();
            if (light == null)
            {
                light = ghosts[index].gameObject.AddComponent<Light>();
            }

            light.type = LightType.Point;
            light.color = new Color(0.8f, 0.55f, 1f);
            light.range = 3.5f;
            light.intensity = 0.75f;
            m_GhostLights.Add(light);
        }
    }

    void AnimateCollectibles(float time)
    {
        for (int index = 0; index < m_Collectibles.Count; index++)
        {
            HauntedCollectible collectible = m_Collectibles[index];
            if (collectible == null || collectible.IsCollected)
            {
                continue;
            }

            collectible.transform.Rotate(Vector3.up, 60f * Time.deltaTime, Space.World);
            float bob = Mathf.Sin(time * 2f + index) * 0.08f;
            Vector3 position = collectible.BasePosition;
            collectible.transform.position = new Vector3(position.x, position.y + bob, position.z);
        }
    }

    void DrawMeter(Rect rect, float amount, Color color, string label)
    {
        GUI.color = new Color(0.12f, 0.12f, 0.16f, 1f);
        GUI.DrawTexture(rect, Texture2D.whiteTexture);
        GUI.color = color;
        GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width * Mathf.Clamp01(amount), rect.height), Texture2D.whiteTexture);
        GUI.color = Color.white;
        GUI.Label(new Rect(rect.x, rect.y - 2f, rect.width, rect.height + 8f), label, m_LabelStyle);
    }

    void EnsureStyles()
    {
        if (m_LabelStyle != null)
        {
            return;
        }

        m_LabelStyle = new GUIStyle(GUI.skin.label);
        m_LabelStyle.fontSize = 13;
        m_LabelStyle.normal.textColor = Color.white;

        m_HeaderStyle = new GUIStyle(m_LabelStyle);
        m_HeaderStyle.fontSize = 18;
        m_HeaderStyle.fontStyle = FontStyle.Bold;
    }

    static int CompareWaypointOrder(Transform left, Transform right)
    {
        return GetWaypointSortValue(left.name).CompareTo(GetWaypointSortValue(right.name));
    }

    static int GetWaypointSortValue(string waypointName)
    {
        if (waypointName.EndsWith("_Start", StringComparison.Ordinal))
        {
            return 0;
        }

        if (waypointName.EndsWith("_End", StringComparison.Ordinal))
        {
            return 100;
        }

        return 50;
    }

    static Material CreateMaterial(Color color)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null)
        {
            shader = Shader.Find("Sprites/Default");
        }

        Material material = new Material(shader);
        material.color = color;
        return material;
    }
}

public class HauntedCollectible : MonoBehaviour
{
    HauntedHouseEnhancements m_Controller;
    Transform m_Player;

    public bool IsCollected { get; private set; }
    public Vector3 BasePosition { get; private set; }

    public void Initialize(HauntedHouseEnhancements controller, Transform player)
    {
        m_Controller = controller;
        m_Player = player;
        BasePosition = transform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (m_Player == null || other.transform == m_Player)
        {
            m_Controller?.Collect(this);
        }
    }

    public void MarkCollected()
    {
        IsCollected = true;
        gameObject.SetActive(false);
    }
}
