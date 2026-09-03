using System;
using System.Collections.Generic;
using SegaCyrus.ToolKits.Model.Random.Attributes;
using SegaCyrus.ToolKits.Model.Random.Model;

namespace SegaCyrus.ToolKits.Package.RandomPackage
{
    /// <summary>
    /// 随机生成上下文
    /// </summary>
    public class RandomGenerateContext
    {
        private readonly HashSet<string> _visitedKeys;
        private readonly RandSeedProxy _proxy;

        /// <summary>属性上的配置标记（由调用方传入）。</summary>
        private RandomConfigAttribute[] _configAttrs;

        /// <summary>用户上下文缓存。</summary>
        public RandomContext RandomContext { get; private set; }

        internal RandomGenerateContext(RandomContext randomContext, RandSeedProxy proxy)
        {
            RandomContext = randomContext ?? new RandomContext();
            _proxy = proxy ?? throw new ArgumentNullException("proxy");
            _visitedKeys = new HashSet<string>();
        }

        /// <summary>
        /// 获取指定类型的随机种子策略。
        /// </summary>
        public BaseRandSeed GetPolicy(Type type)
        {
            return _proxy.GetPolicy(type);
        }

        /// <summary>
        /// 尝试进入某个属性的生成流程（死循环保护）。
        /// </summary>
        /// <returns>true = 首次访问，继续生成；false = 已访问过，应跳过。</returns>
        public bool TryEnter(string key)
        {
            return _visitedKeys.Add(key);
        }

        #region Config 提取工具

        /// <summary>
        /// 从当前上下文的配置标记中提取指定类型的第一个匹配项。
        /// 替代策略类中重复的 <c>configAttrs?.FirstOrDefault(c => c is T)</c>。
        /// </summary>
        public T GetConfig<T>() where T : RandomConfigAttribute
        {
            if (_configAttrs == null) return null;
            for (int i = 0; i < _configAttrs.Length; i++)
            {
                T match = _configAttrs[i] as T;
                if (match != null) return match;
            }

            return null;
        }

        /// <summary>
        /// 获取所有配置标记。
        /// </summary>
        public RandomConfigAttribute[] ConfigAttrs
        {
            get { return _configAttrs; }
        }

        #endregion

        #region 内部

        /// <summary>
        /// 创建子上下文：共享同一个 RandomContext 和 proxy，但使用独立的 configAttrs 和 visitSet。
        /// </summary>
        internal RandomGenerateContext CreateChild(RandomConfigAttribute[] configAttrs)
        {
            RandomGenerateContext child = new RandomGenerateContext(RandomContext, _proxy);
            child._configAttrs = configAttrs;
            return child;
        }

        /// <summary>
        /// 设置当前上下文的配置标记（由 Generate 流程内部使用）。
        /// </summary>
        internal void SetConfigAttrs(RandomConfigAttribute[] configAttrs)
        {
            _configAttrs = configAttrs;
        }

        #endregion
    }
}