using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using SegaCyrus.ToolKits.Kit;
using SegaCyrus.ToolKits.Model.Common;

namespace SegaCyrus.ToolKits.Common
{
    /// <summary>
    /// 通用工厂器
    /// </summary>
    /// <typeparam name="TPolicy">策略类，会构造此类的单例</typeparam>
    /// <typeparam name="TAttribute">策略必须标记的属性</typeparam>
    /// <typeparam name="TInterface">策略必须继承的类</typeparam>
    /// <typeparam name="TPolicyType">对外策略唯一标识类型</typeparam>
    /// <typeparam name="TPoolKey">策略唯一标识类型</typeparam>
    public abstract class PolicyPool<TPolicy, TAttribute, TInterface, TPolicyType, TPoolKey>
        where TAttribute : Attribute where TPolicy : class, new() where TInterface : class
    {
        private static TPolicy InternalPolicyInstance { get; } = new TPolicy();
        private Dictionary<TPoolKey, TInterface> InternalPool { get; set; }

        /// <summary>
        /// 池资源
        /// </summary>
        protected Dictionary<TPoolKey, TInterface> Pool => InternalPool ?? RefreshPool();

        /// <summary>
        /// 刷新资源池
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected Dictionary<TPoolKey, TInterface> RefreshPool(bool checkPool = true)
        {
            if (checkPool == false || InternalPool == null)
                InternalPool = GetPolicyPool();
            return InternalPool;
        }

        /// <summary>
        /// 通过实例和属性输出此策略的唯一标识
        /// </summary>
        /// <param name="attribute">此策略实例被标记的属性</param>
        /// <param name="instnace">策略实例</param>
        /// <returns>策略key</returns>
        protected abstract TPoolKey BuildPookKey(TAttribute attribute, TInterface instnace);

        /// <summary>
        /// key转换方法
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>内部key</returns>
        protected abstract TPoolKey GetKey(TPolicyType key);

        /// <summary>
        /// 单例
        /// </summary>
        public static TPolicy Instance => InternalPolicyInstance;

        /// <summary>
        /// 根据key获取策略
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual TInterface GetPolicy(TPolicyType key)
            => AssertKit.AssertNotNull(Pool.TryGetValue(GetKey(key), out var t) ? t : default, msg: "策略不存在");

        /// <summary>
        /// 重新获取池实例
        /// </summary>
        /// <returns></returns>
        private Dictionary<TPoolKey, TInterface> GetPolicyPool()
        {
            var instancePool = new Dictionary<Guid, TInterface>();
            var data = ReflectKit
                .GetMarkAttrAndInhertClassTypes<TInterface, TAttribute>(true);
            foreach (var k in data.Select(x => x.Key))
            {
                if (instancePool.ContainsKey(k.GUID))
                    continue;
                instancePool.Add(k.GUID, (TInterface)ReflectKit.CreateInstance(k));
            }

            var res = data.Select(c => new KVModel<TAttribute, TInterface>(c.Value, instancePool[c.Key.GUID]));
            var resDic = new Dictionary<TPoolKey, TInterface>();
            foreach (var item in res)
                resDic[BuildPookKey(item.Key, item.Value)] = item.Value;

            return resDic;
        }

        /// <summary>
        /// 替换或者新增策略实现
        /// </summary>
        /// <param name="policyKey">策略KEY</param>
        /// <param name="policy">策略实现</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ReplacePolicy(TPoolKey policyKey, TInterface policy)
        {
            AssertKit.AssertNotNull(policyKey, nameof(policyKey));
            AssertKit.AssertNotNull(policy, nameof(policy));
            RefreshPool();
            InternalPool[policyKey] = policy;
        }

        /// <summary>
        /// 构造器
        /// </summary>
        protected PolicyPool()
        {
            RefreshPool();
        }
    }
}