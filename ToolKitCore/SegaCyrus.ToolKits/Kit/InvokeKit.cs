using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SegaCyrus.ToolKits.Extension.CommonExtension;
using SegaCyrus.ToolKits.Model.Common;

namespace SegaCyrus.ToolKits.Kit
{
    /// <summary>
    /// 反射调用工具包
    /// </summary>
    public class InvokeKit
    {
        #region base

        private static object InternalCallMethod(Type type, object instance, bool isStatic,
            string funcName, List<Type> genericTypeList, params object[] p)
        {
            var funcData = GetMethodInfo(type, isStatic, funcName, genericTypeList,
                p?.Select(c => c.GetType()).ToList());
            AssertKit.AssertNotNull(funcData,
                msg: isStatic ? $"{type.FullName}不存在静态方法{funcName}" : $"{type.FullName}不存在成员方法{funcName}");
            var func = funcData.Key;

            var paramDefineList = funcData.Value;
            var defaultValue = paramDefineList.Skip(p.Length).ToList();

            var paramList = p.ToList();
            if (defaultValue.NotEmpty())
                paramList.AddRange(defaultValue.Select(c => c.DefaultValue));

            var result = func.Invoke(isStatic ? null : instance, paramList.ToArray());
            if (func.ReturnType == null)
                return null;
            return result;
        }

        private static PropertyInfo GetStaticPropertyInfo(Type type, string propertyName)
        {
            AssertKit.AssertNotNull(type, nameof(type));
            var property = type.GetProperty(propertyName,
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            return AssertKit.AssertNotNull(property, msg: $"{type.FullName}不存在静态属性{propertyName}");
        }

        private static FieldInfo GetStaticFieldInfo(Type type, string fieldName)
        {
            AssertKit.AssertNotNull(type, nameof(type));
            var field = type.GetField(fieldName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            return AssertKit.AssertNotNull(field, msg: $"{type.FullName}不存在静态字段{field}");
        }

        private static PropertyInfo GetPropertyInfo(Type type, string propertyName)
        {
            AssertKit.AssertNotNull(type, nameof(type));
            var property = type.GetProperty(propertyName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return AssertKit.AssertNotNull(property, msg: $"{type.FullName}不存在属性{propertyName}");
        }

        private static FieldInfo GetFieldInfo(Type type, string fieldName)
        {
            AssertKit.AssertNotNull(type, nameof(type));
            var field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return AssertKit.AssertNotNull(field, msg: $"{type.FullName}不存在字段{field}");
        }

        private static KVModel<MethodInfo, ParameterInfo[]> GetMethodInfo(Type type, bool isStatic, string methodName,
            List<Type> genericType, List<Type> paramTypes)
        {
            var genericTypeArr = genericType?.ToArray();

            AssertKit.AssertNotNull(type, nameof(type));
            Func<MethodInfo, MethodInfo> genericGenerator = (MethodInfo method) =>
            {
                try
                {
                    return method.MakeGenericMethod(genericTypeArr);
                }
                catch
                {
                    return null;
                }
            };

            paramTypes = paramTypes ?? new List<Type>();

            var funcList = type.GetMethods(BindingFlags.Static | BindingFlags.Instance |
                                           BindingFlags.Public | BindingFlags.NonPublic)
                .Where(c => c.Name == methodName)
                .Where(c => c.IsGenericMethod == (genericType != null))
                .Select(c => genericType == null ? c : genericGenerator(c))
                .Where(c => c != null)
                .Where(c => c.IsStatic == isStatic)
                .Select(c => new KVModel<MethodInfo, ParameterInfo[]>(c, c.GetParameters()))
                .Where(c => (c.Value.Length > paramTypes.Count &&
                             c.Value.Count(x => !x.HasDefaultValue) == paramTypes.Count) ||
                            c.Value.Length == paramTypes.Count)
                .OrderBy(c => c.Value.Length - paramTypes.Count)
                .ToList();

            AssertKit.AssertGreater(funcList.Count, 0, $"{type.FullName}不存在方法{methodName}");
            if (funcList.Count == 1)
                return funcList.First();

            foreach (var fun in funcList)
            {
                var isOK = true;
                for (var i = 0; i < paramTypes.Count; ++i)
                {
                    if (paramTypes[i].GUID != fun.Value[i].ParameterType.GUID &&
                        false == ReflectKit.IsInhert(paramTypes[i], fun.Value[i].ParameterType))
                    {
                        isOK = false;
                        break;
                    }
                }

                if (isOK)
                    return fun;
            }

            return AssertKit.AssertNotNull(funcList.First(), msg: $"{type.FullName}不存在方法{methodName}");
        }

        #endregion

        #region static property

        /// <summary>
        /// 获取指定type的指定静态属性
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <param name="propertyName">属性名</param>
        /// <returns>属性值</returns>
        public static object GetStaticProperty(Type type, string propertyName)
        {
            return GetStaticPropertyInfo(type, propertyName).GetValue(null);
        }

        /// <summary>
        /// 设置指定类型的静态属性
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <param name="propertyName">属性名</param>
        /// <param name="data">目标值</param>
        public static void SetStaticProperty(Type type, string propertyName, object data)
        {
            GetStaticPropertyInfo(type, propertyName).SetValue(null, data);
        }

        #endregion

        #region static field

        /// <summary>
        /// 获取指定type的指定静态字段
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <param name="fieldName">字段名</param>
        /// <returns>字段值</returns>
        public static object GetStaticField(Type type, string fieldName)
        {
            return GetStaticFieldInfo(type, fieldName).GetValue(null);
        }

        /// <summary>
        /// 设置静态字段值
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <param name="fieldName">字段名</param>
        /// <param name="data">目标值</param>
        public static void SetStaticField(Type type, string fieldName, object data)
        {
            GetStaticFieldInfo(type, fieldName).SetValue(null, data);
        }

        #endregion

        #region property

        /// <summary>
        /// 获取成员属性
        /// </summary>
        /// <typeparam name="T">实例类型</typeparam>
        /// <param name="instance">实例</param>
        /// <param name="propertyName">属性名</param>
        /// <returns>属性值</returns>
        public static object GetProperty<T>(T instance, string propertyName)
        {
            return GetPropertyInfo(instance.GetType(), propertyName).GetValue(instance);
        }

        /// <summary>
        /// 设置成员属性
        /// </summary>
        /// <typeparam name="T">实例类型</typeparam>
        /// <param name="instance">实例</param>
        /// <param name="propertyName">属性名</param>
        /// <param name="data">目标值</param>
        public static void SetProperty<T>(T instance, string propertyName, object data)
        {
            GetPropertyInfo(instance.GetType(), propertyName).SetValue(instance, data);
        }

        #endregion

        #region field

        /// <summary>
        /// 获取指定type的指定静态字段
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="fieldName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static object GetField<T>(T instance, string fieldName)
        {
            return GetFieldInfo(instance.GetType(), fieldName).GetValue(instance);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="fieldName"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        public static void SetField<T>(T instance, string fieldName, object data)
        {
            GetFieldInfo(instance.GetType(), fieldName).SetValue(instance, data);
        }

        #endregion

        #region method

        /// <summary>
        /// 调用静态函数
        /// </summary>
        /// <param name="typeFullName">调用目标全路径</param>
        /// <param name="funcName">函数名</param>
        /// <param name="p">参数列表</param>
        /// <returns>返回值</returns>
        public static object CallStaticMethod(string typeFullName, string funcName, params object[] p)
        {
            return InternalCallMethod(ReflectKit.GetTypeByFullName(typeFullName), null, true, funcName, null, p);
        }

        /// <summary>
        /// 调用静态函数
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <param name="funcName">函数名</param>
        /// <param name="p">参数列表</param>
        /// <returns>返回值</returns>
        public static object CallStaticMethod(Type type, string funcName, params object[] p)
        {
            return InternalCallMethod(type, null, true, funcName, null, p);
        }

        /// <summary>
        /// 调用成员函数
        /// </summary>
        /// <param name="instance">实例</param>
        /// <param name="funcName">函数名</param>
        /// <param name="p">参数列表</param>
        /// <returns>返回值</returns>
        public static object CallMethod(object instance, string funcName, params object[] p)
        {
            return InternalCallMethod(instance.GetType(), instance, false, funcName, null, p);
        }

        /// <summary>
        /// 调用泛型静态函数
        /// </summary>
        /// <param name="typeFullName">调用目标全路径</param>
        /// <param name="funcName">函数名</param>
        /// <param name="genericTypeList">泛型参数列表</param>
        /// <param name="p">参数列表</param>
        /// <returns>函数返回值</returns>
        public static object CallGenericStaticMethod(string typeFullName, string funcName, List<Type> genericTypeList,
            params object[] p)
        {
            return InternalCallMethod(ReflectKit.GetTypeByFullName(typeFullName), null, true, funcName, genericTypeList,
                p);
        }

        /// <summary>
        /// 调用泛型静态函数
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="funcName">函数名</param>
        /// <param name="genericTypeList">泛型参数列表</param>
        /// <param name="p">参数列表</param>
        /// <returns>函数返回值</returns>
        public static object CallGenericStaticMethod(Type type, string funcName, List<Type> genericTypeList,
            params object[] p)
        {
            return InternalCallMethod(type, null, true, funcName, genericTypeList, p);
        }

        /// <summary>
        /// 调用泛型静态函数
        /// </summary>
        /// <param name="instance">某一实例</param>
        /// <param name="funcName">函数名</param>
        /// <param name="genericTypeList">泛型参数列表</param>
        /// <param name="p">参数列表</param>
        /// <returns>函数返回值</returns>
        public static object CallGenericMethod(object instance, string funcName, List<Type> genericTypeList,
            params object[] p)
        {
            return InternalCallMethod(instance.GetType(), instance, false, funcName, genericTypeList, p);
        }

        #endregion
    }
}