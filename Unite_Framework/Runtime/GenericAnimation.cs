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

        [Header("�����б�")]
        public List<AdditionalAnimation> additionalAnimations = new List<AdditionalAnimation>();

        public enum AnimationType
        {
            Move,
            Rotate,
            Scale,
            TextFade,
            ImageFade,
            SpriteColor
        }

        public enum AdditionalAnimationType
        {
            Move,
            Rotate,
            Scale,
            TextFade,
            ImageFade,
            SpriteColor
        }

        [System.Serializable]
        public class AdditionalAnimation
        {
            public AdditionalAnimationType type;
            public Vector3 startval;
            public Vector3 endval;
            public float duration;
            public Ease ease;
            public Text text;
            public Image image;
            public Sprite imageSprite;
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
                            Text text = additionalAnimation.text;
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
                        if (additionalAnimation.image != null && additionalAnimation.imageSprite != null)
                        {
                            Image image = additionalAnimation.image;
                            image.color = Color.white;
                            _sequence.Join(image.DOFade(0f, additionalAnimation.duration / 2f))
                                .AppendCallback(() =>
                                {
                                    image.sprite = additionalAnimation.imageSprite;
                                })
                                .Join(image.DOFade(1f, additionalAnimation.duration / 2f));
                        }
                        break;
                    case AdditionalAnimationType.SpriteColor:
                        if (additionalAnimation.spriteRenderer != null)
                        {
                            SpriteRenderer spriteRenderer = additionalAnimation.spriteRenderer;
                            _sequence.Join(DOTween.To(() => spriteRenderer.color, x => spriteRenderer.color = x, additionalAnimation.colorToChange, additionalAnimation.duration));
                        }
                        break;
                    default:
                        Debug.LogWarning("��Ч�Ķ�������: " + additionalAnimation.type);
                        break;
                }
            }

            // ���һ����ʱΪ0�ļ���������ж���ͬʱ����
            _sequence.AppendInterval(0);
        }
    }

    [CustomEditor(typeof(GenericAnimation), true)]
    public class CustomAnimationEditor : Editor
    {
        private SerializedProperty _animationTypeProp;
        private SerializedProperty _additionalAnimationsProp;

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
                SerializedProperty additionalAnimationProp = _additionalAnimationsProp.GetArrayElementAtIndex(i);
                SerializedProperty typeProp = additionalAnimationProp.FindPropertyRelative("type");

                EditorGUILayout.LabelField("��Ӷ���" + (i + 1).ToString(), EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(typeProp);

                if (typeProp.enumValueIndex == (int)GenericAnimation.AdditionalAnimationType.Move)
                {
                    DrawRotateInspector(additionalAnimationProp);
                }
                else if (typeProp.enumValueIndex == (int)GenericAnimation.AdditionalAnimationType.Rotate)
                {
                    DrawRotateInspector(additionalAnimationProp);
                }
                else if (typeProp.enumValueIndex == (int)GenericAnimation.AdditionalAnimationType.Scale)
                {
                    DrawScaleInspector(additionalAnimationProp);
                }
                else if (typeProp.enumValueIndex == (int)GenericAnimation.AdditionalAnimationType.TextFade)
                {
                    DrawTextFadeInspector(additionalAnimationProp);
                }
                else if (typeProp.enumValueIndex == (int)GenericAnimation.AdditionalAnimationType.ImageFade)
                {
                    DrawImageFadeInspector(additionalAnimationProp);
                }
                else if (typeProp.enumValueIndex == (int)GenericAnimation.AdditionalAnimationType.SpriteColor)
                {
                    DrawSpriteColorInspector(additionalAnimationProp);
                }

                if (GUILayout.Button("ɾ������"))
                {
                    anim.additionalAnimations.RemoveAt(i);
                    serializedObject.ApplyModifiedProperties();
                    return;
                }
            }

            if (GUILayout.Button("��Ӷ���"))
            {
                anim.AddAnimation();
                serializedObject.ApplyModifiedProperties();
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("����Ԥ��������ʱ��"))
            {
                anim.Play();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private const string LABLE_START_POSISITION     = "��ʼλ��";
        private const string LABLE_END_POSISTION        = "Ŀ��λ��";
        private const string LABLE_DURATION             = "����ʱ��";
        private const string LABLE_START_ROTATION       = "��ʼ�Ƕ�";
        private const string LABLE_END_ROTATION         = "Ŀ��Ƕ�";
        private const string LABLE_START_SCALE          = "��ʼ��С";
        private const string LABLE_END_SCALE            = "Ŀ���С";
        private const string LABLE_COLOR                = "Ŀ����ɫ";
        private const string LABLE_EASE                 = "��������";

        private void DrawRotateInspector(SerializedProperty additionalAnimationProp)
        {
            EditorGUILayout.PropertyField(additionalAnimationProp.FindPropertyRelative("startval"), new GUIContent(LABLE_START_ROTATION));
            EditorGUILayout.PropertyField(additionalAnimationProp.FindPropertyRelative("endval"), new GUIContent(LABLE_END_ROTATION));
            EditorGUILayout.PropertyField(additionalAnimationProp.FindPropertyRelative("duration"), new GUIContent(LABLE_DURATION));
            EditorGUILayout.PropertyField(additionalAnimationProp.FindPropertyRelative("ease"), new GUIContent(LABLE_EASE));
        }

        private void DrawScaleInspector(SerializedProperty additionalAnimationProp)
        {
            EditorGUILayout.PropertyField(additionalAnimationProp.FindPropertyRelative("startval"), new GUIContent(LABLE_START_SCALE));
            EditorGUILayout.PropertyField(additionalAnimationProp.FindPropertyRelative("endval"), new GUIContent(LABLE_END_SCALE));
            EditorGUILayout.PropertyField(additionalAnimationProp.FindPropertyRelative("duration"), new GUIContent(LABLE_DURATION));
            EditorGUILayout.PropertyField(additionalAnimationProp.FindPropertyRelative("ease"), new GUIContent(LABLE_EASE));
        }

        private void DrawTextFadeInspector(SerializedProperty additionalAnimationProp)
        {
            EditorGUILayout.PropertyField(additionalAnimationProp.FindPropertyRelative("text"));
            EditorGUILayout.PropertyField(additionalAnimationProp.FindPropertyRelative("duration"), new GUIContent(LABLE_DURATION));
            EditorGUILayout.PropertyField(additionalAnimationProp.FindPropertyRelative("ease"), new GUIContent(LABLE_EASE));
        }

        private void DrawImageFadeInspector(SerializedProperty additionalAnimationProp)
        {
            EditorGUILayout.PropertyField(additionalAnimationProp.FindPropertyRelative("image"));
            EditorGUILayout.PropertyField(additionalAnimationProp.FindPropertyRelative("imageSprite"));
            EditorGUILayout.PropertyField(additionalAnimationProp.FindPropertyRelative("duration"), new GUIContent(LABLE_DURATION));
            EditorGUILayout.PropertyField(additionalAnimationProp.FindPropertyRelative("ease"), new GUIContent(LABLE_EASE));
        }

        private void DrawSpriteColorInspector(SerializedProperty additionalAnimationProp)
        {
            EditorGUILayout.PropertyField(additionalAnimationProp.FindPropertyRelative("spriteRenderer"));
            EditorGUILayout.PropertyField(additionalAnimationProp.FindPropertyRelative("colorToChange"), new GUIContent(LABLE_COLOR));
            EditorGUILayout.PropertyField(additionalAnimationProp.FindPropertyRelative("duration"), new GUIContent(LABLE_DURATION));
        }
    }
}