using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Unite
{
    public class GenericAnimation : MonoBehaviour
    {
		private Sequence _sequence;

        [Header("动画列表")]
        public List<AdditionalAnimation> additionalAnimations = new List<AdditionalAnimation>();

        public enum AnimationType
        {
            Move,
            Rotate,
            Scale,
            TextFade,
            ImageFade,
        }

        public enum AdditionalAnimationType
        {
            Move,
            Rotate,
            Scale,
            TextFade,
            ImageFade,
        }

        [System.Serializable]
        public class AdditionalAnimation
        {
            public enum FadeType
            {
                FadeIn = 0, FadeOut,
            }
            public AdditionalAnimationType type;
            public Vector3 startval;
            public Vector3 endval;
            public float duration;
            public Ease ease;
            public FadeType fadeType;
            public Text text;
            public Image image;
            public SpriteRenderer spriteRenderer;
            public Color colorToChange;
            public Color textColor = Color.white;
        }

        private void Start() { }

        public void AddAnimation()
        {
            AdditionalAnimation additionalAnimation = new AdditionalAnimation();
            additionalAnimations.Add(additionalAnimation);
        }
        public void Play()
        {
            if (_sequence != null)
            {
                _sequence.Kill();
            }
            _sequence = DOTween.Sequence();

            foreach (AdditionalAnimation additionalAnimation in additionalAnimations)
            {
                switch (additionalAnimation.type)
                {
                    case AdditionalAnimationType.Move:
                        transform.position = additionalAnimation.startval;
                        _sequence.Join(transform.DOMove(additionalAnimation.endval, additionalAnimation.duration).SetEase(additionalAnimation.ease));
                        break;
                    case AdditionalAnimationType.Rotate:
                        transform.eulerAngles = additionalAnimation.startval;
                        _sequence.Join(transform.DORotate(additionalAnimation.endval, additionalAnimation.duration).SetEase(additionalAnimation.ease));
                        break;
                    case AdditionalAnimationType.Scale:
                        transform.localScale = additionalAnimation.startval;
                        _sequence.Join(transform.DOScale(additionalAnimation.endval, additionalAnimation.duration).SetEase(additionalAnimation.ease));
                        break;
                    case AdditionalAnimationType.TextFade:
                        if (additionalAnimation.text != null && !string.IsNullOrEmpty(additionalAnimation.text.text))
                        {
                            var text = additionalAnimation.text;
                            text.color = additionalAnimation.textColor;
                            _sequence.Join(text.DOFade(0f, additionalAnimation.duration / 2f))
                                .AppendCallback(() =>
                                {
                                    text.text = text.text;
                                })
                                .Join(text.DOFade(1f, additionalAnimation.duration / 2f));
                        }
                        break;
                    case AdditionalAnimationType.ImageFade:
                        if (additionalAnimation.image != null)
                        {
                            var image = additionalAnimation.image;
                            image.color = Color.white;
                            switch (additionalAnimation.fadeType)
                            {
                                case AdditionalAnimation.FadeType.FadeIn:
                                    var color = image.color;
                                    color = new Color(color.r, color.g, color.b, 0);
                                    _sequence.Join(image.DOFade(1f, additionalAnimation.duration));
                                    break;
                                case AdditionalAnimation.FadeType.FadeOut:
                                    _sequence.Join(image.DOFade(0f, additionalAnimation.duration));
                                    break;
                            }
                        }
                        break;
                    default:
                        Debug.LogWarning("无效的动画类型: " + additionalAnimation.type);
                        break;
                }
            }

            // 添加一个延时为0的间隔，让所有动画同时播放
            _sequence.AppendInterval(0);
        }
    }

    [CustomEditor(typeof(GenericAnimation), true)]
    public class CustomAnimationEditor : Editor
    {
        private SerializedProperty _animationTypeProp;
        private SerializedProperty _additionalAnimationsProp;
        private readonly string[] ANIMATION_TYPE = {
            "移动动画","旋转动画","缩放动画","淡入淡出-文字","淡入淡出-图片","Sprite颜色"

        };

        void OnEnable()
        {
            _animationTypeProp = serializedObject.FindProperty("animationType");
            _additionalAnimationsProp = serializedObject.FindProperty("additionalAnimations");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GenericAnimation anim = target as GenericAnimation;
            //EditorGUILayout.ObjectField("Script", anim, typeof(GenericAnimation), false);

            for (int i = 0; i < anim.additionalAnimations.Count; i++)
            {
                EditorGUILayout.Space();
                SerializedProperty animProp = _additionalAnimationsProp.GetArrayElementAtIndex(i);
                SerializedProperty typeProp = animProp.FindPropertyRelative("type");

                var index = (int)anim.additionalAnimations[i].type;
                EditorGUILayout.LabelField(new GUIContent(ANIMATION_TYPE[index]));
                ++EditorGUI.indentLevel;

                EditorGUILayout.PropertyField(typeProp, new GUIContent("动画类型"));

                if (typeProp.enumValueIndex == (int)GenericAnimation.AdditionalAnimationType.Move)
                {
                    DrawRotateInspector(animProp);
                }
                else if (typeProp.enumValueIndex == (int)GenericAnimation.AdditionalAnimationType.Rotate)
                {
                    DrawRotateInspector(animProp);
                }
                else if (typeProp.enumValueIndex == (int)GenericAnimation.AdditionalAnimationType.Scale)
                {
                    DrawScaleInspector(animProp);
                }
                else if (typeProp.enumValueIndex == (int)GenericAnimation.AdditionalAnimationType.TextFade)
                {
                    DrawTextFadeInspector(animProp);
                }
                else if (typeProp.enumValueIndex == (int)GenericAnimation.AdditionalAnimationType.ImageFade)
                {
                    DrawImageFadeInspector(animProp);
                }
                --EditorGUI.indentLevel;

                if (GUILayout.Button("删除动画"))
                {
                    anim.additionalAnimations.RemoveAt(i);
                    serializedObject.ApplyModifiedProperties();
                    return;
                }
            }

            if (GUILayout.Button("添加动画"))
            {
                anim.AddAnimation();
                serializedObject.ApplyModifiedProperties();
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("动画预览（运行时）"))
            {
                anim.Play();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private const string LABLE_START_POSISITION     = "起始位置";
        private const string LABLE_END_POSISTION        = "目标位置";
        private const string LABLE_DURATION             = "动画时长";
        private const string LABLE_START_ROTATION       = "起始角度";
        private const string LABLE_END_ROTATION         = "目标角度";
        private const string LABLE_START_SCALE          = "初始大小";
        private const string LABLE_END_SCALE            = "目标大小";
        private const string LABLE_COLOR                = "目标颜色";
        private const string LABLE_EASE                 = "缓动函数";

        private void DrawRotateInspector(SerializedProperty animProp)
        {
            EditorGUILayout.PropertyField(animProp.FindPropertyRelative("startval"), new GUIContent(LABLE_START_ROTATION));
            EditorGUILayout.PropertyField(animProp.FindPropertyRelative("endval"), new GUIContent(LABLE_END_ROTATION));
            EditorGUILayout.PropertyField(animProp.FindPropertyRelative("duration"), new GUIContent(LABLE_DURATION));
            EditorGUILayout.PropertyField(animProp.FindPropertyRelative("ease"), new GUIContent(LABLE_EASE));
        }

        private void DrawScaleInspector(SerializedProperty animProp)
        {
            EditorGUILayout.PropertyField(animProp.FindPropertyRelative("startval"), new GUIContent(LABLE_START_SCALE));
            EditorGUILayout.PropertyField(animProp.FindPropertyRelative("endval"), new GUIContent(LABLE_END_SCALE));
            EditorGUILayout.PropertyField(animProp.FindPropertyRelative("duration"), new GUIContent(LABLE_DURATION));
            EditorGUILayout.PropertyField(animProp.FindPropertyRelative("ease"), new GUIContent(LABLE_EASE));
        }

        private void DrawTextFadeInspector(SerializedProperty animProp)
        {
            EditorGUILayout.PropertyField(animProp.FindPropertyRelative("text"), new GUIContent("目标文本"));
            EditorGUILayout.PropertyField(animProp.FindPropertyRelative("duration"), new GUIContent(LABLE_DURATION));
            EditorGUILayout.PropertyField(animProp.FindPropertyRelative("ease"), new GUIContent(LABLE_EASE));
        }

        private void DrawImageFadeInspector(SerializedProperty animProp)
        {
            EditorGUILayout.PropertyField(animProp.FindPropertyRelative("image"), new GUIContent("目标Image"));
            EditorGUILayout.PropertyField(animProp.FindPropertyRelative("fadeType"), new GUIContent("淡入淡出类型"));
            EditorGUILayout.PropertyField(animProp.FindPropertyRelative("duration"), new GUIContent(LABLE_DURATION));
            EditorGUILayout.PropertyField(animProp.FindPropertyRelative("ease"), new GUIContent(LABLE_EASE));
        }

        private void DrawSpriteColorInspector(SerializedProperty animProp)
        {
            EditorGUILayout.PropertyField(animProp.FindPropertyRelative("spriteRenderer"), new GUIContent("目标Sprite"));
            EditorGUILayout.PropertyField(animProp.FindPropertyRelative("colorToChange"), new GUIContent(LABLE_COLOR));
            EditorGUILayout.PropertyField(animProp.FindPropertyRelative("duration"), new GUIContent(LABLE_DURATION));
        }
    }
}
