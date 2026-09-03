namespace SegaCyrus.ToolKits.Model.Common
{
    /// <summary>
    /// KV模型
    /// </summary>
    /// <typeparam name="K">KType</typeparam>
    /// <typeparam name="V">VType</typeparam>
    public class KVModel<K, V>
    {
        /// <summary>
        /// Key
        /// </summary>
        public K Key { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public V Value { get; set; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public KVModel(K key, V value)
        {
            Key = key;
            Value = value;
        }
    }
}