using System;
using System.Runtime.CompilerServices;
using SegaCyrus.ToolKits.Kit;

namespace SegaCyrus.ToolKits.Extension.InvokeExtension
{
    /// <summary>
    /// InvokeKit扩展包
    /// </summary>
    public static class EasyInvokeKit
    {
        #region 静态成员反射

        /// <summary>
        /// 获取静态属性
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="data">此类型的某一实例</param>
        /// <param name="propertyName">属性名</param>
        /// <returns>属性</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object GetStaticProperty<T>(this T data, string propertyName)
        {
            return InvokeKit.GetStaticProperty(typeof(T), propertyName);
        }

        /// <summary>
        /// 设置静态属性
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="data">此类型的某一实例</param>
        /// <param name="propertyName">属性名</param>
        /// <param name="target">目标值</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetStaticProperty<T>(this T data, string propertyName, object target)
        {
            InvokeKit.SetStaticProperty(typeof(T), propertyName, target);
        }

        /// <summary>
        /// 获取静态字段
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="data">此类型的某一实例</param>
        /// <param name="fieldName">字段名</param>
        /// <returns>字段值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object GetStaticField<T>(this T data, string fieldName)
        {
            return InvokeKit.GetStaticField(typeof(T), fieldName);
        }

        /// <summary>
        /// 设置静态字段
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="data">此类型的某一实例</param>
        /// <param name="fieldName">字段名</param>
        /// <param name="target">目标值</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetStaticField<T>(this T data, string fieldName, object target)
        {
            InvokeKit.SetStaticField(data.GetType(), fieldName, target);
        }

        #endregion

        #region 实例成员反射

        /// <summary>
        /// 调用静态函数
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="data">此类型的某一实例</param>
        /// <param name="propertyName">函数名</param>
        /// <returns>函数返回值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object GetProperty<T>(this T data, string propertyName) where T : class
        {
            return InvokeKit.GetProperty(data, propertyName);
        }

        /// <summary>
        /// 调用静态函数
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="instance">此类型的某一实例</param>
        /// <param name="propertyName">函数名</param>
        /// <param name="data">参数列表</param>
        /// <returns>函数返回值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetProperty<T>(this T instance, string propertyName, object data) where T : class
        {
            InvokeKit.SetProperty(instance, propertyName, data);
        }

        /// <summary>
        /// 调用静态函数
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="data">此类型的某一实例</param>
        /// <param name="fieldName">函数名</param>
        /// <returns>函数返回值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object GetField<T>(this T data, string fieldName) where T : class
        {
            return InvokeKit.GetField(data, fieldName);
        }

        /// <summary>
        /// 调用静态函数
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="instance">此类型的某一实例</param>
        /// <param name="fieldName">函数名</param>
        /// <param name="data">参数列表</param>
        /// <returns>函数返回值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetField<T>(this T instance, string fieldName, object data) where T : class
        {
            InvokeKit.SetField(instance, fieldName, data);
        }

        /// <summary>
        /// 调用静态函数
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="data">此类型的某一实例</param>
        /// <param name="funcName">函数名</param>
        /// <param name="p">参数列表</param>
        /// <returns>函数返回值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object CallMethod<T>(this T data, string funcName, params object[] p) where T : class
        {
            return InvokeKit.CallMethod(data, funcName, p);
        }

        #endregion

        #region 静态函数反射

        /// <summary>
        /// 调用静态函数
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="data">此类型的某一实例</param>
        /// <param name="funcName">函数名</param>
        /// <param name="p">参数列表</param>
        /// <returns>函数返回值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object CallStaticMethod<T>(this T data, string funcName, params object[] p) where T : class
        {
            return typeof(T).CallStaticMethod(funcName, p);
        }

        /// <summary>
        /// 调用静态函数
        /// </summary>
        /// <param name="data">类型</param>
        /// <param name="funcName">函数名</param>
        /// <param name="p">参数列表</param>
        /// <returns>函数返回值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object CallStaticMethod(this Type data, string funcName, params object[] p)
        {
            return InvokeKit.CallStaticMethod(data, funcName, p);
        }

        #endregion
    }
}