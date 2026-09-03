using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using SegaCyrus.ToolKits.Extension.CommonExtension;
using SegaCyrus.ToolKits.Model.Common;
using SegaCyrus.ToolKits.Model.ReflectInvoke;

namespace SegaCyrus.ToolKits.Kit
{
    /// <summary>
    /// 语言特性反射工具包
    /// </summary>
    public class ReflectKit
    {
        #region

        /// <summary>
        /// 通过FullName获取类型 如果出现程序集加载失败将吃掉异常不对外暴露
        /// </summary>
        /// <param name="typeName">funame</param>
        /// <param name="ignoreCase">忽略大小写</param>
        /// <returns>类定义</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type GetTypeByFullName(string typeName, bool ignoreCase = true)
        {
            var allType = GetTypesByAssemble();
            return allType.FirstOrDefault(c =>
                string.Equals(c.FullName, typeName, StringComparison.OrdinalIgnoreCase));
        }

        #endregion

        /// <summary>
        /// 获取指定类型的默认值
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>默认值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object GetDefaultValue(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        #region 特性工具组

        /// <summary>
        /// 获取所有被标记并继承指定类的类型
        /// </summary>
        /// <typeparam name="TParentClass">继承</typeparam>
        /// <typeparam name="TAttribute">属性</typeparam>
        /// <param name="mustCouldInsntace">是否为可实例化类</param>
        /// <param name="assemblies">程序集</param>
        /// <returns>结果</returns>
        public static List<KVModel<Type, TAttribute>> GetMarkAttrAndInhertClassTypes<TParentClass, TAttribute>(
            bool mustCouldInsntace, params Assembly[] assemblies)
            where TAttribute : Attribute where TParentClass : class
        {
            var typeList = GetTypesByAssemble(assemblies);

            #region 特性校验

            Func<Attribute[], IEnumerable<TAttribute>> isExtendFilter = args =>
            {
                return args.Where(a => a is TAttribute).Select(c => (TAttribute)c);
            };

            Func<Type, Tuple<bool, Type, IEnumerable<TAttribute>>> filter = o =>
            {
                try
                {
                    return Tuple.Create(
                        IsInhert<TParentClass>(o) &&
                        (mustCouldInsntace == false || !(o.IsAbstract || o.IsInterface)),
                        o,
                        isExtendFilter(Attribute.GetCustomAttributes(o, true)));
                }
                catch
                {
                    return null;
                }
            };

            var interruptClass = typeList.Select(o => filter(o)).Where(c => c != null).ToList();

            #endregion

            #region 构造返回集

            var result = interruptClass
                .Where(c => c.Item1 && c.Item3 != null)
                .SelectMany(c => c.Item3.Select(x => new KVModel<Type, TAttribute>(c.Item2, x))).ToList();

            #endregion

            return result;
        }

        #endregion

        #region 内部类型推断

        internal static Type TryGetType(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            if (int.TryParse(value, out _))
                return typeof(int);
            if (long.TryParse(value, out _))
                return typeof(long);
            if (decimal.TryParse(value, out _))
                return typeof(decimal);
            if (float.TryParse(value, out _))
                return typeof(float);
            if (double.TryParse(value, out _))
                return typeof(double);
            if (char.TryParse(value, out _))
                return typeof(char);
            if (DateTime.TryParse(value, out _))
                return typeof(DateTime);
            if (Guid.TryParse(value, out _))
                return typeof(Guid);
            return typeof(string);
        }

        internal static Func<string, object> GetConvertDataHandle(Type type)
        {
            if (typeof(int) == type)
                return t =>
                    string.IsNullOrEmpty(t) ? (int?)null : (int.TryParse(t, out var tmp) ? tmp : default);
            if (typeof(char) == type)
                return t =>
                    string.IsNullOrEmpty(t) ? (char?)null : (char.TryParse(t, out var tmp) ? tmp : default);
            if (typeof(byte) == type)
                return t =>
                    string.IsNullOrEmpty(t) ? (byte?)null : (byte.TryParse(t, out var tmp) ? tmp : default);
            if (typeof(decimal) == type)
                return t => string.IsNullOrEmpty(t)
                    ? (decimal?)null
                    : (decimal.TryParse(t, out var tmp) ? tmp : default);
            if (typeof(long) == type)
                return t =>
                    string.IsNullOrEmpty(t) ? (long?)null : (long.TryParse(t, out var tmp) ? tmp : default);
            if (typeof(float) == type)
                return t =>
                    string.IsNullOrEmpty(t) ? (float?)null : (float.TryParse(t, out var tmp) ? tmp : default);
            if (typeof(DateTime) == type)
                return t => string.IsNullOrEmpty(t)
                    ? (DateTime?)null
                    : (DateTime.TryParse(t, out var tmp) ? tmp : default);
            if (typeof(double) == type)
                return t =>
                    string.IsNullOrEmpty(t) ? (double?)null : (double.TryParse(t, out var tmp) ? tmp : default);
            if (typeof(Guid) == type)
                return t =>
                    string.IsNullOrEmpty(t) ? (Guid?)null : (Guid.TryParse(t, out var tmp) ? tmp : default);
            if (typeof(bool)==type)
                return t => string.IsNullOrEmpty(t) ? false : (bool.Parse(t));
            if (type.IsEnum)
                return t => string.IsNullOrEmpty(t) ? null : (Enum.Parse(type, t));

            return t => t;
        }

        #endregion

        #region Instance

        /// <summary>
        /// 构造指定类型的实例,若有构造函数，则参数使用默认值
        /// </summary>
        /// <typeparam name="T">待构造类型</typeparam>
        /// <returns>构造后的实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CreateInstanceWithDefaultValue<T>()
        {
            return (T)CreateInstanceWithDefaultValue(typeof(T));
        }

        /// <summary>
        /// 构造指定类型的实例,若有构造函数，则参数使用默认值
        /// </summary>
        /// <param name="type">待构造类型</param>
        /// <returns>构造后的实例</returns>
        public static object CreateInstanceWithDefaultValue(Type type)
        {
            var t = type.GetConstructors().FirstOrDefault();
            if (t == null)
                return CreateInstance(type);
            var paramList = t.GetParameters();
            if (!paramList.NotEmpty())
                return CreateInstance(type);
            var paramObjects = new object[paramList.Length];
            for (var i = 0; i < paramList.Length; ++i)
            {
                paramObjects[i] = paramList[i].ParameterType.IsValueType
                    ? Activator.CreateInstance(paramList[i].ParameterType)
                    : null;
            }

            return CreateInstance(type, paramObjects);
        }

        /// <summary>
        /// 构造指定类型的实例
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="buildConstructArgs">构造参数</param>
        /// <returns>实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CreateInstance<T>(params object[] buildConstructArgs)
        {
            return (T)CreateInstance(typeof(T), buildConstructArgs);
        }

        /// <summary>
        /// 构造指定类型的实例
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="buildConstructArgsAction">构造参数</param>
        /// <returns>实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object CreateInstance(Type type, params object[] buildConstructArgsAction)
        {
            return Activator.CreateInstance(type, buildConstructArgsAction);
        }

        #endregion

        #region 派生继承判断

        /// <summary>
        /// 检查类型是否是T的派生
        /// </summary>
        /// <typeparam name="CType">子类</typeparam>
        /// <typeparam name="ParentType">父类</typeparam>
        /// <returns>检查结果</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInhert<CType, ParentType>() where CType : class where ParentType : class
        {
            return IsInhert<ParentType>(typeof(Type));
        }

        /// <summary>
        /// 检查类型是否是T的派生
        /// </summary>
        /// <typeparam name="T">父类</typeparam>
        /// <param name="type">子类</param>
        /// <returns>检查结果</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInhert<T>(Type type) where T : class
        {
            return IsInhert(type, typeof(T));
        }

        /// <summary>
        /// 检查是否是parentType的派生
        /// </summary>
        /// <param name="type">派生类</param>
        /// <param name="parentType">父类</param>
        /// <returns>检查结果</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInhert(Type type, Type parentType)
        {
            return parentType.IsAssignableFrom(type);
        }

        #endregion

        #region 程序集

        /// <summary>
        /// 获取并加载当前可能用到的程序集
        /// </summary>
        /// <returns></returns>
        public static GetAssembliesResult GetAssemblies()
        {
            var existAssemblies = new List<Assembly>();
            var noExistAssemblies = new List<AssemblyName>();
            var loadedAssemblies = new HashSet<string>();
            var assembliesToCheck = new Queue<Assembly>();

            assembliesToCheck.Enqueue(Assembly.GetEntryAssembly());

            while (assembliesToCheck.Count > 0)
            {
                var assemblyToCheck = assembliesToCheck.Dequeue();

                foreach (var reference in assemblyToCheck.GetReferencedAssemblies())
                {
                    if (!loadedAssemblies.Contains(reference.FullName))
                    {
                        try
                        {
                            var assembly = Assembly.Load(reference);
                            assembliesToCheck.Enqueue(assembly);
                            existAssemblies.Add(assembly);
                        }
                        catch
                        {
                            noExistAssemblies.Add(reference);
                        }
                        finally
                        {
                            loadedAssemblies.Add(reference.FullName);
                        }
                    }
                }
            }

            return new GetAssembliesResult()
            {
                ExistAssemblies = existAssemblies,
                NotExistAssemblies = noExistAssemblies
            };
        }


        /// <summary>
        /// 获取指定程序集下所有Type,若未指定则全部
        /// </summary>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static List<Type> GetTypesByAssemble(params Assembly[] assemblies)
        {
            var typeList = new List<Type>();

            #region 取asm集

            var asmList = assemblies.IsEmpty() ? AppDomain.CurrentDomain.GetAssemblies() : assemblies;

            #endregion

            #region 取类型集合

            foreach (var asm in asmList)
            {
                try
                {
                    typeList.AddRange(asm.GetTypes());
                }
                catch
                {
                }
            }

            #endregion

            return typeList;
        }

        #endregion

        #region 数组判断

        /// <summary>
        /// 是否是List
        /// </summary>
        /// <param name="type">判定类型</param>
        /// <returns>判定结果</returns>
        public static bool IsList(Type type)
        {
            if (typeof(IList).IsAssignableFrom(type))
                return true;

            foreach (var it in type.GetInterfaces())
                if (it.IsGenericType && typeof(IList<>) == it.GetGenericTypeDefinition())
                    return true;

            return false;
        }

        /// <summary>
        /// 是否是Enumerable
        /// </summary>
        /// <param name="type">判定类型</param>
        /// <returns>判定结果</returns>
        public static bool IsEnumerable(Type type)
        {
            if (type.IsArray || typeof(IEnumerable).IsAssignableFrom(type))
                return true;

            foreach (var it in type.GetInterfaces())
                if (it.IsGenericType && typeof(IEnumerable<>) == it.GetGenericTypeDefinition())
                    return true;
            return false;
        }

        #endregion
    }
}