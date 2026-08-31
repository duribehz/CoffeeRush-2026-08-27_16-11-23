using TMPro;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class ResultScreenSetup
{
    private const string ScenePath = "Assets/Scenes/Demo_Scene.unity";
    private const string AnimationFolder = "Assets/Animations";

    public static void Generate()
    {
        AnimationClip hidden = CreateClip("Result_Hidden");
        SetCurve(hidden, string.Empty, typeof(CanvasGroup), "m_Alpha", 0f, 0f, 0.01f, 0f);
        SetCurve(hidden, "Result Card", typeof(Transform), "m_LocalScale.x", 0f, 0.82f, 0.01f, 0.82f);
        SetCurve(hidden, "Result Card", typeof(Transform), "m_LocalScale.y", 0f, 0.82f, 0.01f, 0.82f);
        SetCurve(hidden, "Result Card", typeof(Transform), "m_LocalScale.z", 0f, 1f, 0.01f, 1f);

        AnimationClip win = CreateClip("Result_Win");
        SetCurve(win, string.Empty, typeof(CanvasGroup), "m_Alpha", 0f, 0f, 0.18f, 1f, 0.4f, 1f);
        SetCurve(win, "Result Card", typeof(Transform), "m_LocalScale.x", 0f, 0.78f, 0.18f, 1.08f, 0.34f, 1f);
        SetCurve(win, "Result Card", typeof(Transform), "m_LocalScale.y", 0f, 0.78f, 0.18f, 1.08f, 0.34f, 1f);
        SetCurve(win, "Result Card", typeof(Transform), "m_LocalScale.z", 0f, 1f, 0.34f, 1f);

        AnimationClip lose = CreateClip("Result_Lose");
        SetCurve(lose, string.Empty, typeof(CanvasGroup), "m_Alpha", 0f, 0f, 0.18f, 1f, 0.45f, 1f);
        SetCurve(lose, "Result Card", typeof(Transform), "m_LocalScale.x", 0f, 0.88f, 0.22f, 1f);
        SetCurve(lose, "Result Card", typeof(Transform), "m_LocalScale.y", 0f, 0.88f, 0.22f, 1f);
        SetCurve(lose, "Result Card", typeof(Transform), "m_LocalScale.z", 0f, 1f, 0.22f, 1f);
        SetCurve(lose, "Result Card", typeof(RectTransform), "m_AnchoredPosition.x", 0f, 0f, 0.1f, -18f, 0.2f, 14f, 0.3f, -8f, 0.4f, 0f);

        AnimatorController controller = CreateController(hidden, win, lose);
        BuildSceneUi(controller);
        AssetDatabase.SaveAssets();
        Debug.Log("Animated win/lose result screen generated successfully.");
    }

    private static AnimationClip CreateClip(string name)
    {
        string path = AnimationFolder + "/" + name + ".anim";
        AssetDatabase.DeleteAsset(path);
        AnimationClip clip = new AnimationClip { name = name, frameRate = 30f, wrapMode = WrapMode.ClampForever };
        AssetDatabase.CreateAsset(clip, path);
        return clip;
    }

    private static void SetCurve(AnimationClip clip, string path, System.Type type, string property, params float[] values)
    {
        Keyframe[] keys = new Keyframe[values.Length / 2];
        for (int i = 0; i < keys.Length; i++)
            keys[i] = new Keyframe(values[i * 2], values[i * 2 + 1]);

        AnimationCurve curve = new AnimationCurve(keys);
        for (int i = 0; i < keys.Length; i++)
            AnimationUtility.SetKeyLeftTangentMode(curve, i, AnimationUtility.TangentMode.ClampedAuto);
        AnimationUtility.SetEditorCurve(clip, EditorCurveBinding.FloatCurve(path, type, property), curve);
    }

    private static AnimatorController CreateController(AnimationClip hidden, AnimationClip win, AnimationClip lose)
    {
        string path = AnimationFolder + "/ResultScreen.controller";
        AssetDatabase.DeleteAsset(path);
        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(path);
        controller.AddParameter("Win", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Lose", AnimatorControllerParameterType.Trigger);

        AnimatorStateMachine machine = controller.layers[0].stateMachine;
        AnimatorState hiddenState = machine.AddState("Hidden");
        hiddenState.motion = hidden;
        AnimatorState winState = machine.AddState("Win");
        winState.motion = win;
        AnimatorState loseState = machine.AddState("Lose");
        loseState.motion = lose;
        machine.defaultState = hiddenState;

        AnimatorStateTransition winTransition = machine.AddAnyStateTransition(winState);
        winTransition.hasExitTime = false;
        winTransition.duration = 0f;
        winTransition.AddCondition(AnimatorConditionMode.If, 0f, "Win");

        AnimatorStateTransition loseTransition = machine.AddAnyStateTransition(loseState);
        loseTransition.hasExitTime = false;
        loseTransition.duration = 0f;
        loseTransition.AddCondition(AnimatorConditionMode.If, 0f, "Lose");
        return controller;
    }

    private static void BuildSceneUi(AnimatorController controller)
    {
        EditorSceneManager.OpenScene(ScenePath);
        GameObject canvas = GameObject.Find("Coffee Rush HUD");
        GameManager manager = Object.FindFirstObjectByType<GameManager>();
        if (canvas == null || manager == null)
            throw new MissingReferenceException("Coffee Rush HUD or GameManager was not found in Demo_Scene.");

        Transform oldOverlay = canvas.transform.Find("Result Overlay");
        if (oldOverlay != null)
            Object.DestroyImmediate(oldOverlay.gameObject);

        GameObject overlay = CreateImage("Result Overlay", canvas.transform, new Color(0.12f, 0.07f, 0.04f, 0.82f));
        Stretch(overlay.GetComponent<RectTransform>());
        CanvasGroup canvasGroup = overlay.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        Animator animator = overlay.AddComponent<Animator>();
        animator.runtimeAnimatorController = controller;
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;

        GameObject card = CreateImage("Result Card", overlay.transform, new Color(1f, 0.98f, 0.94f, 1f));
        SetRect(card.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(650f, 390f));

        TextMeshProUGUI title = CreateText("Result Title", card.transform, "SERVICIO COMPLETADO", 42f, FontStyles.Bold, new Color(0.38f, 0.25f, 0.16f));
        SetRect(title.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -92f), new Vector2(-70f, 80f));

        TextMeshProUGUI description = CreateText("Result Description", card.transform, string.Empty, 24f, FontStyles.Normal, new Color(0.45f, 0.34f, 0.26f));
        SetRect(description.rectTransform, new Vector2(0f, 0.5f), new Vector2(1f, 0.5f), new Vector2(0f, -5f), new Vector2(-90f, 120f));

        Button restart = CreateButton("Result Restart Button", card.transform, "Reiniciar");
        SetRect(restart.GetComponent<RectTransform>(), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 68f), new Vector2(250f, 64f));

        SerializedObject serializedManager = new SerializedObject(manager);
        serializedManager.FindProperty("resultOverlay").objectReferenceValue = canvasGroup;
        serializedManager.FindProperty("resultTitle").objectReferenceValue = title;
        serializedManager.FindProperty("resultDescription").objectReferenceValue = description;
        serializedManager.FindProperty("resultRestartButton").objectReferenceValue = restart;
        serializedManager.FindProperty("resultAnimator").objectReferenceValue = animator;
        serializedManager.ApplyModifiedPropertiesWithoutUndo();

        EditorSceneManager.MarkSceneDirty(manager.gameObject.scene);
        EditorSceneManager.SaveScene(manager.gameObject.scene);
    }

    private static GameObject CreateImage(string name, Transform parent, Color color)
    {
        GameObject gameObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        gameObject.transform.SetParent(parent, false);
        Image image = gameObject.GetComponent<Image>();
        image.color = color;
        image.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        image.type = Image.Type.Sliced;
        return gameObject;
    }

    private static TextMeshProUGUI CreateText(string name, Transform parent, string value, float size, FontStyles style, Color color)
    {
        GameObject gameObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        gameObject.transform.SetParent(parent, false);
        TextMeshProUGUI text = gameObject.GetComponent<TextMeshProUGUI>();
        text.text = value;
        text.fontSize = size;
        text.fontStyle = style;
        text.color = color;
        text.alignment = TextAlignmentOptions.Center;
        text.raycastTarget = false;
        return text;
    }

    private static Button CreateButton(string name, Transform parent, string label)
    {
        GameObject gameObject = CreateImage(name, parent, new Color(0.82f, 0.42f, 0.16f, 1f));
        Button button = gameObject.AddComponent<Button>();
        button.targetGraphic = gameObject.GetComponent<Image>();
        ColorBlock colors = button.colors;
        colors.highlightedColor = new Color(0.72f, 0.34f, 0.12f, 1f);
        colors.pressedColor = new Color(0.58f, 0.25f, 0.08f, 1f);
        button.colors = colors;

        TextMeshProUGUI text = CreateText("TextMeshPro - Text", gameObject.transform, label, 24f, FontStyles.Bold, Color.white);
        Stretch(text.rectTransform);
        return button;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private static void SetRect(RectTransform rect, Vector2 min, Vector2 max, Vector2 position, Vector2 size)
    {
        rect.anchorMin = min;
        rect.anchorMax = max;
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
    }
}
