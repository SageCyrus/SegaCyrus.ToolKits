using SegaCyrus.ToolKits.Model.Expression;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace SegaCyrus.ToolKits.Kit
{
    /// <summary>
    /// 表达式树工具包,用于代替InvokeKit反射包
    /// </summary>
    public class ExpressionKit
    {
        #region common
        private const string PropertyCacheProfixName = "SegaCyrus.ToolKit.ExpressKit.PropertyCache";
        private const string FieldCacheProfixName = "SegaCyrus.ToolKit.ExpressKit.FieldCache";
        private static string BuildPropertyCacheKey(string rITName, string pTName, string iTName, string pName)
            => $"{PropertyCacheProfixName}_{rITName}_{pTName}_{iTName}_{pName}";
        private static string BuildFieldCacheKey(string rITName, string fTName, string iTName, string fName)
            => $"{FieldCacheProfixName}_{rITName}_{fTName}_{iTName}_{fName}";
        #endregion

        #region CreateFieldExpress


        /// <summary>
        /// 生成指定类型下指定字段的Get和Set访问器 不带缓存
        /// </summary>
        /// <typeparam name="InstanceType">目标类型</typeparam>
        /// <typeparam name="FieldType">属性类型</typeparam>
        /// <param name="type">实例类型</param>
        /// <param name="fieldName">字段名</param>
        /// <returns>访问器</returns>
        public static ExpressionGetSetModel<InstanceType, FieldType> CreateFieldExpressWithoutCache<InstanceType, FieldType>(Type type, string fieldName)
        {
            AssertKit.AssertNotNull(type, nameof(type));
            var fieldInfo = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            fieldInfo = AssertKit.AssertNotNull(fieldInfo, msg: $"{type.FullName}不存在字段{fieldName}");


            var inputInstanceType = typeof(InstanceType);
            var outputField = typeof(FieldType);
            var parameter = Expression.Parameter(inputInstanceType, "obj");
            var valueExpr = Expression.Parameter(outputField, "value");

            Expression i = parameter, v = valueExpr;
            if (inputInstanceType != type)
                i = Expression.Convert(i, type);
            if (outputField != fieldInfo.FieldType)
                v = Expression.Convert(v, fieldInfo.FieldType);

            var fieldAccess = Expression.Field(i, fieldInfo);
            var assignExpr = Expression.Assign(fieldAccess, v);

            var lambda = Expression.Lambda<Action<InstanceType, FieldType>>(assignExpr, parameter, valueExpr);
            var setter = lambda.Compile();

            var getLambda = Expression.Convert(fieldAccess, outputField);
            var lambdaGet = Expression.Lambda<Func<InstanceType, FieldType>>(getLambda, parameter);
            var getter = lambdaGet.Compile();

            return new ExpressionGetSetModel<InstanceType, FieldType>(fieldName, setter, getter);
        }
        /// <summary>
        /// 生成指定类型下指定属性的Get和Set访问器 不带缓存
        /// </summary>
        /// <typeparam name="InstanceType">目标类型</typeparam>
        /// <typeparam name="FieldType">属性类型</typeparam>
        /// <param name="instance">实例</param>
        /// <param name="fieldName">字段名</param>
        /// <returns>访问器</returns>
        public static ExpressionGetSetModel<InstanceType, FieldType> CreateFieldExpressWithoutCache<InstanceType, FieldType>(InstanceType instance, string fieldName)
        {
            return CreateFieldExpressWithoutCache<InstanceType, FieldType>(typeof(InstanceType), fieldName);
        }

        /// <summary>
        /// 生成指定类型下指定字段的Get和Set访问器 带缓存
        /// </summary>
        /// <typeparam name="InstanceType">目标类型</typeparam>
        /// <typeparam name="FieldType">属性类型</typeparam>
        /// <param name="type">实例类型</param>
        /// <param name="fieldName">字段名</param>
        /// <returns>访问器</returns>
        public static ExpressionGetSetModel<InstanceType, FieldType> CreateFieldExpress<InstanceType, FieldType>(Type type, string fieldName)
        {
            AssertKit.AssertNotNull(type, nameof(type));

            var rInstanceTypeFullName = typeof(InstanceType).FullName;
            var rFieldTypeFullName = typeof(FieldType).FullName;
            var instanceTypeFullName = type.FullName;

            var propertyCachekey = BuildFieldCacheKey(rInstanceTypeFullName, rFieldTypeFullName, instanceTypeFullName, fieldName);

            return CacheKit.GetOrAdd(propertyCachekey, () => CreateFieldExpressWithoutCache<InstanceType, FieldType>(type, fieldName));
        }

        /// <summary>
        /// 生成指定类型下指定属性的Get和Set访问器 带缓存
        /// </summary>
        /// <typeparam name="InstanceType">目标类型</typeparam>
        /// <typeparam name="FieldType">属性类型</typeparam>
        /// <param name="instance">实例</param>
        /// <param name="fieldName">字段名</param>
        /// <returns>访问器</returns>
        public static ExpressionGetSetModel<InstanceType, FieldType> CreateFieldExpress<InstanceType, FieldType>(InstanceType instance, string fieldName)
        {
            var rInstanceTypeFullName = typeof(InstanceType).FullName;
            var rFieldTypeFullName = typeof(FieldType).FullName;

            var propertyCachekey = BuildFieldCacheKey(rInstanceTypeFullName, rFieldTypeFullName, rInstanceTypeFullName, fieldName);

            return CacheKit.GetOrAdd(propertyCachekey, () => CreateFieldExpressWithoutCache<InstanceType, FieldType>(instance, fieldName));
        }
        #endregion

        #region CreatePropertyExpress

        /// <summary>
        /// 生成指定类型下指定属性的Get和Set访问器 不带缓存
        /// </summary>
        /// <typeparam name="InstanceType">目标类型</typeparam>
        /// <typeparam name="PropertyType">属性类型</typeparam>
        /// <param name="type">实例类型</param>
        /// <param name="propertyName">属性名</param>
        /// <returns>访问器</returns>
        public static ExpressionGetSetModel<InstanceType, PropertyType> CreatePropertyExpressWithoutCache<InstanceType, PropertyType>(Type type, string propertyName)
        {
            AssertKit.AssertNotNull(type, nameof(type));
            var property = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            property = AssertKit.AssertNotNull(property, msg: $"{type.FullName}不存在属性{propertyName}");

            var setterFunc = property.GetSetMethod(true);
            var getterFunc = property.GetGetMethod(true);

            Action<InstanceType, PropertyType> setter =
                (i, p) => throw new NotImplementedException($"{type.FullName}下属性{propertyName}未实现set方法");
            Func<InstanceType, PropertyType> getter = 
                (i) => throw new NotImplementedException($"{type.FullName}下属性{propertyName}未实现get方法");

            if (setterFunc != null || getterFunc != null)
            {
                var inputInstanceType = typeof(InstanceType);
                var inputPropertyType = typeof(PropertyType);
                var parameter = Expression.Parameter(inputInstanceType, "obj");
                var valueExpr = Expression.Parameter(inputPropertyType, "value");


                Expression p = parameter, v = valueExpr;
                if (inputInstanceType != type)
                    p = Expression.Convert(p, type);

                if (inputPropertyType != property.PropertyType)
                    v = Expression.Convert(v, property.PropertyType);

                if (setter != null)
                {
                    var propertyCall = Expression.Call(p, setterFunc, v);
                    var lambda = Expression.Lambda<Action<InstanceType, PropertyType>>(propertyCall, parameter, valueExpr);
                    setter = lambda.Compile();
                }
                if (getterFunc != null)
                {
                    var propertyCall = Expression.Convert(Expression.Call(p, getterFunc), inputPropertyType);
                    var lambda = Expression.Lambda<Func<InstanceType, PropertyType>>(propertyCall, parameter);
                    getter = lambda.Compile();
                }
            }
            return new ExpressionGetSetModel<InstanceType, PropertyType>(propertyName, setter, getter);
        }

        /// <summary>
        /// 生成指定类型下指定属性的Get和Set访问器 不带缓存
        /// </summary>
        /// <typeparam name="InstanceType">目标类型</typeparam>
        /// <typeparam name="PropertyType">属性类型</typeparam>
        /// <param name="instance">实例</param>
        /// <param name="propertyName">属性名</param>
        /// <returns>访问器</returns>
        public static ExpressionGetSetModel<InstanceType, PropertyType> CreatePropertyExpressWithoutCache<InstanceType, PropertyType>(InstanceType instance, string propertyName)
        {
            return CreatePropertyExpressWithoutCache<InstanceType, PropertyType>(typeof(InstanceType), propertyName);
        }
        /// <summary>
        /// 生成指定类型下指定属性的Get和Set访问器 带缓存
        /// </summary>
        /// <typeparam name="InstanceType">目标类型</typeparam>
        /// <typeparam name="PropertyType">属性类型</typeparam>
        /// <param name="type">实例类型</param>
        /// <param name="propertyName">属性名</param>
        /// <returns>访问器</returns>
        public static ExpressionGetSetModel<InstanceType, PropertyType> CreatePropertyExpress<InstanceType, PropertyType>(Type type, string propertyName)
        {
            AssertKit.AssertNotNull(type, nameof(type));

            var rInstanceTypeFullName = typeof(InstanceType).FullName;
            var rPropertTypeFullName = typeof(PropertyType).FullName;
            var instanceTypeFullName = type.FullName;

            var propertyCachekey = BuildPropertyCacheKey(rInstanceTypeFullName, rPropertTypeFullName, instanceTypeFullName, propertyName);

            return CacheKit.GetOrAdd(propertyCachekey, () => CreatePropertyExpressWithoutCache<InstanceType, PropertyType>(type, propertyName));
        }

        /// <summary>
        /// 生成指定类型下指定属性的Get和Set访问器 带缓存
        /// </summary>
        /// <typeparam name="InstanceType">目标类型</typeparam>
        /// <typeparam name="PropertyType">属性类型</typeparam>
        /// <param name="instance">实例</param>
        /// <param name="propertyName">属性名</param>
        /// <returns>访问器</returns>
        public static ExpressionGetSetModel<InstanceType, PropertyType> CreatePropertyExpress<InstanceType, PropertyType>(InstanceType instance, string propertyName)
        {
            var rInstanceTypeFullName = typeof(InstanceType).FullName;
            var rPropertTypeFullName = typeof(PropertyType).FullName;

            var propertyCachekey = BuildPropertyCacheKey(rInstanceTypeFullName, rPropertTypeFullName, rInstanceTypeFullName, propertyName);

            return CacheKit.GetOrAdd(propertyCachekey, () => CreatePropertyExpressWithoutCache<InstanceType, PropertyType>(instance, propertyName));
        }
        #endregion
    }
}