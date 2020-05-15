using System.Collections.Generic;
using UnityEngine;
using System;

namespace UnityEngine.UI.Effect
{
    public interface IParameterTexture
    {
        int parameterIndex { get; set; }

        ParameterTexture paramTex { get; }
    }

    [Serializable]
    public class ParameterTexture
    {
        public ParameterTexture(int channels, int instanceLimit, string propertyName)
        {
            _propertyName = propertyName;
            _channels = ((channels - 1) / 4 + 1) * 4;
            _instanceLimit = ((instanceLimit - 1) / 2 + 1) * 2;
            _data = new byte[_channels * _instanceLimit];

            _stack = new Stack<int>(_instanceLimit);
            for (var i = 1; i < _instanceLimit + 1; i++)
            {
                _stack.Push(i);
            }
        }
        
        public void Register(IParameterTexture target)
        {
            Initialize();
            if (target.parameterIndex <= 0 && 0 < _stack.Count)
            {
                target.parameterIndex = _stack.Pop();
            }
        }

        public void Unregister(IParameterTexture target)
        {
            if (0 < target.parameterIndex)
            {
                _stack.Push(target.parameterIndex);
                target.parameterIndex = 0;
            }
        }

        public void SetData(IParameterTexture target, int channelId, byte value)
        {
            var index = (target.parameterIndex - 1) * _channels + channelId;
            if (0 < target.parameterIndex && _data[index] != value)
            {
                _data[index] = value;
                _needUpload = true;
            }
        }

        public void SetData(IParameterTexture target, int channelId, float value)
        {
            SetData(target, channelId, (byte) (Mathf.Clamp01(value) * 255));
        }

        public void RegisterMaterial(Material mat)
        {
            if (_propertyId == 0)
            {
                _propertyId = Shader.PropertyToID(_propertyName);
            }

            if (mat)
            {
                mat.SetTexture(_propertyId, _texture);
            }
        }

        public float GetNormalizedIndex(IParameterTexture target)
        {
            return (target.parameterIndex - 0.5f) / _instanceLimit;
        }

        private Texture2D _texture;
        private bool _needUpload;
        private int _propertyId;
        private readonly string _propertyName;
        private readonly int _channels;
        private readonly int _instanceLimit;
        private readonly byte[] _data;
        private readonly Stack<int> _stack;
        private static List<Action> updates;

        private void Initialize()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying && UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }
#endif
            if (updates == null)
            {
                updates = new List<Action>();
                Canvas.willRenderCanvases += () =>
                {
                    var count = updates.Count;
                    for (var i = 0; i < count; i++)
                    {
                        updates[i].Invoke();
                    }
                };
            }

            if (!_texture)
            {
                var isLinear = QualitySettings.activeColorSpace == ColorSpace.Linear;
                _texture = new Texture2D(_channels / 4, _instanceLimit, TextureFormat.RGBA32, false, isLinear);
                _texture.filterMode = FilterMode.Point;
                _texture.wrapMode = TextureWrapMode.Clamp;
                updates.Add(UpdateParameterTexture);
                _needUpload = true;
            }
        }

        private void UpdateParameterTexture()
        {
            if (_needUpload && _texture)
            {
                _needUpload = false;
                _texture.LoadRawTextureData(_data);
                _texture.Apply(false, false);
            }
        }
    }
}