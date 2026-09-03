using SegaCyrus.ToolKits.Kit;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace SegaCyrus.ToolKits.Package.RandomPackage.Builder
{
    /// <summary>
    /// 随机模型构建器
    /// </summary>
    public class RandomModelBuilder<T>
    {
        private readonly Dictionary<string, object> _overrides = new Dictionary<string, object>();

        /// <summary>
        /// 覆盖指定属性的值。
        /// </summary>
        /// <typeparam name="TProp">属性类型</typeparam>
        /// <param name="selector">属性选择器</param>
        /// <param name="value">覆盖值</param>
        public RandomModelBuilder<T> With<TProp>(Expression<Func<T, TProp>> selector, TProp value)
        {
            MemberExpression memberExpr = selector.Body as MemberExpression;
            if (memberExpr != null)
            {
                _overrides[memberExpr.Member.Name] = value;
            }
            else
            {
                throw new ArgumentException("Selector must be a member expression (e.g., x => x.PropertyName).");
            }
            return this;
        }

        /// <summary>
        /// 构建模型：先生成随机对象，再应用覆盖值。
        /// </summary>
        public T Build()
        {
            T instance = RandomKit.NextModel<T>();
            if (_overrides.Count == 0) return instance;

            Type type = typeof(T);
            foreach (KeyValuePair<string, object> kvp in _overrides)
            {
                PropertyInfo prop = type.GetProperty(kvp.Key);
                if (prop != null && prop.CanWrite)
                {
                    prop.SetValue(instance, kvp.Value, null);
                }
            }
            return instance;
        }

        /// <summary>
        /// 批量构建 count 个模型，每个模型均应用相同的覆盖值。
        /// </summary>
        public List<T> BuildMany(int count)
        {
            AssertKit.AssertPositive(count, nameof(count));
            if (count == 0) return new List<T>();

            List<T> result = new List<T>(count);
            for (int i = 0; i < count; i++)
                result.Add(Build());
            return result;
        }
    }
}
