using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEditor;
using PlayableAnimation;

namespace ComboTree.Editor
{
    [CustomPreview(typeof(EditorTransitionWrapper))]
    public class TransitionPreview : ObjectPreview
    {
        #region const
        readonly static Vector3 defaultPosition = new Vector3(0, 1.25f, 0);
        readonly static Vector3 defaultRotation = new Vector3(-25, 40, 0);
        const float defaultDistance = 2;
        const float moveSpeed = 0.01f;
        const float pivotSpeed = 0.25f;
        const float scrollSpeed = 0.25f;
        const float timeScaleParameter = 0.3f;
        #endregion

        PreviewRenderUtility preview;
        GameObject instance;
        Transform cameraTransform;
        Transform cameraPivot;
        PlayableGraph graph;
        AnimationMixerPlayable transitionMixer;
        float timeScale = 1;
        float fromDuration;
        float toDuration;

        SerializedTransition t => ((EditorTransitionWrapper)target).editorTransition;
        float time
        {
            get
            {
                return transitionMixer.IsNull() ? 0 : (float)transitionMixer.GetTime();
            }
            set
            {
                if (transitionMixer.IsNull())
                    return;

                transitionMixer.SetTime(value);
                transitionMixer.GetInput(1).SetTime(value + t.transitionParameters.transitionOffset);
            }
        }
        float globalExitTime => t.transitionParameters.exitTime * fromDuration;
        float totalTime
        {
            get
            {
                return globalExitTime + t.transitionParameters.transitionTime + toDuration;
            }
        }
        bool IsValid
        {
            get
            {
                return
                    !(t.Owner.animationClip == null || t.Target.animationClip == null);
            }
        }

        public override void Initialize(Object[] targets)
        {
            base.Initialize(targets);
            if (!IsValid)
                return;


            InitializePreview();
            CreateGraph(t);
            Reset();
        }
        public override void OnPreviewSettings()
        {
            EditorGUILayout.BeginVertical();
            timeScale = GUILayout.HorizontalSlider(timeScale, 0.1f, 2, GUILayout.MinWidth(100));
            EditorGUILayout.EndVertical();
        }
        public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
        {
            TransformCamera(Event.current);
            GUI.DrawTexture(r, DrawScenePreview(r));
            Evaluate();
        }
        public override bool HasPreviewGUI()
        {
            return IsValid;
        }
        public override void Cleanup()
        {
            base.Cleanup();

            if (preview != null)
                preview.Cleanup();

            if (graph.IsValid())
                graph.Destroy();
        }
        void InitializePreview()
        {
            if (preview != null)
                preview.Cleanup();

            preview = new PreviewRenderUtility(false);
            cameraPivot = new GameObject().transform;
            var camera = preview.camera;
            cameraTransform = camera.transform;
            var lightTransform = preview.lights[0].transform;


            cameraTransform.SetParent(cameraPivot);
            cameraTransform.position = new Vector3(0, 0, defaultDistance);
            cameraTransform.forward = -Vector3.forward;

            cameraPivot.position = defaultPosition;
            cameraPivot.rotation = Quaternion.Euler(defaultRotation);

            camera.fieldOfView = 60;
            camera.farClipPlane = 1000;
            camera.nearClipPlane = 0.1f;

            lightTransform.SetParent(cameraTransform);
            lightTransform.position = Vector3.zero;
            lightTransform.forward = cameraTransform.forward;

            var go = AssetDatabase.LoadAssetAtPath<GameObject>(ComboTreeEditorWindow.AssetPath + "YBot.fbx");
            instance = Object.Instantiate(go);
            instance.transform.position = Vector3.zero;

            cameraPivot.SetParent(instance.transform);
            preview.AddSingleGO(instance);
        }
        void CreateGraph(SerializedTransition transition)
        {
            var animator = instance.GetComponent<Animator>();

            var from = transition.Owner.animationClip;
            var to = transition.Target.animationClip;

            fromDuration = from.length;
            toDuration = to.length;

            graph = PlayableGraph.Create();
            graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
            var output = AnimationPlayableOutput.Create(graph, "Animator", animator);

            transitionMixer = AnimationMixerPlayable.Create(graph, 2, true);
            transitionMixer.SetPropagateSetTime(true);

            transitionMixer.ConnectInput(0, AnimationClipPlayable.Create(graph, from), 0, 1);
            transitionMixer.ConnectInput(1, AnimationClipPlayable.Create(graph, to), 0, 0);

            output.SetSourcePlayable(transitionMixer);
            graph.Play();
        }
        void TransformCamera(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDrag:
                    if (e.button == 0)
                    {
                        var hor = e.delta.x * moveSpeed * cameraPivot.right;
                        var ver = e.delta.y * moveSpeed * cameraPivot.up;
                        cameraPivot.position += hor + ver;
                        e.Use();
                    }
                    else if (e.button == 1)
                    {
                        cameraPivot.RotateAround(cameraPivot.transform.position, Vector3.up, e.delta.x * pivotSpeed);
                        cameraPivot.RotateAround(cameraPivot.transform.position, cameraPivot.transform.right, -e.delta.y * pivotSpeed);
                        e.Use();
                    }
                    break;
                case EventType.ScrollWheel:
                    var newPos = -e.delta.y * cameraTransform.forward * scrollSpeed + cameraTransform.localPosition;
                    cameraTransform.localPosition = new Vector3(0, 0, Mathf.Clamp(newPos.z, 0.5f, 100));
                    e.Use();
                    break;
                default:
                    break;
            }
        }
        void Evaluate()
        {
            graph.Evaluate(Time.deltaTime * timeScale * timeScaleParameter);

            if (time > totalTime)
                Reset();

            var completion = Mathf.Clamp01((time - t.transitionParameters.exitTime * fromDuration) / t.transitionParameters.transitionTime);

            if (completion == 0)
                transitionMixer.GetInput(1).Pause();
            else
                transitionMixer.GetInput(1).Play();

            transitionMixer.SetInputWeight(0, 1 - completion);
            transitionMixer.SetInputWeight(1, completion);
        }
        Texture DrawScenePreview(Rect r)
        {
            if (preview is null)
                InitializePreview();

            preview.BeginPreview(r, GUIStyle.none);
            preview.camera.Render();
            return preview.EndPreview();
        }
        void Reset()
        {
            instance.transform.position = Vector3.zero;
            instance.transform.rotation = Quaternion.identity;
            time = 0;
        }
    }
}
