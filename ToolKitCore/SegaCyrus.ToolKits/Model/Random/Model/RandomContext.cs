using System.Collections.Generic;

namespace SegaCyrus.ToolKits.Model.Random.Model
{ 
    /// <summary>
    /// 随机上下文
    /// </summary>
    public class RandomContext
    {
        /// <summary>
        /// 随机上下文
        /// </summary>
        public RandomContext()
        {
            CacheContext = new Dictionary<string, object>();
        }

        /// <summary>
        /// 上下文缓存
        /// </summary>
        public Dictionary<string, object> CacheContext { get; private set; }
    }
}