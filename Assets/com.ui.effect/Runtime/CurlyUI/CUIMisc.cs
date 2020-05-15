namespace UnityEngine.UI.Extensions
{
    [System.Serializable]
    public struct Vector3_Array2D
    {
        [SerializeField]
        public Vector2[] array;

        public Vector2 this[int _idx]
        {
            get
            {
                return array[_idx];
            }
            set
            {
                array[_idx] = value;
            }
        }
    }
}