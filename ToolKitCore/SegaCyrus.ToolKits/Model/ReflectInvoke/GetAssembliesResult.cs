using System.Collections.Generic;
using System.Reflection;

namespace SegaCyrus.ToolKits.Model.ReflectInvoke
{
    /// <summary>
    /// 获取程序集返回模型
    /// </summary>
    public class GetAssembliesResult
    {
        /// <summary>
        /// 当前存在的程序集信息
        /// </summary>
        public List<Assembly> ExistAssemblies { get; set; }

        /// <summary>
        /// 当前引用了但是不存在的程序集信息
        /// </summary>
        public List<AssemblyName> NotExistAssemblies { get; set; }
    }
}