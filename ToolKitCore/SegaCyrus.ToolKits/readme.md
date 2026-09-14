# SegaCyrus.ToolKits


> **版本**: 1.0.0
> **作者**: Sega Cyrus  
> **目标框架**: `net45` / `net451` / `net452` / `net8.0`  
> **项目地址**: [github](https://github.com/SageCyrus/SegaCyrus.ToolKits)  
> **描述**: 一个面向 .NET 的通用工具库,提供编码/加解密、序列化、反射、随机数据构造、电子表格读写、AOP、缓存、文件压缩、表达式树等能力。按 `Kit`(核心工具)、`Extension`(便捷扩展方法)、`Package`(策略/代理框架)、`Model`(数据契约与特性)分层组织。

一个面向 .NET 的通用工具库,提供编码/加解密、序列化、反射、随机数据构造、电子表格读写、AOP、缓存、文件压缩、表达式树等能力。按 `Kit`(核心工具)、`Extension`(便捷扩展方法)、`Package`(策略/代理框架)、`Model`(数据契约与特性)分层组织。



## 目标框架

`net45` / `net451` / `net452` / `net8.0`(多目标)。`.NET 8` 下图片相关方法(`RandomKit.NextImage`)因依赖 `System.Drawing.Common` 已被条件编译排除。

---

## 一、`Kit` 命名空间(核心工具包)

### 1. AssertKit — 断言工具包
全部 `public static`,多数带 `[MethodImpl(AggressiveInlining)]`,失败抛 `ArgumentException` / `ArgumentNullException`。

| 方法签名 | 说明 |
|---|---|
| `bool AssertTrue(bool data, string paramName = "", string msg = null)` | 校验必须为 `true` |
| `bool AssertFalse(bool data, string paramName = "", string msg = null)` | 校验必须为 `false` |
| `T AssertPositive<T>(T data, string paramName = "", string msg = null) where T : IComparable` | 校验必须大于 0 |
| `T AssertGreater<T>(T data, T compare, string paramName = "", string msg = null) where T : IComparable` | 校验大于 `compare` |
| `T AssertLesser<T>(T data, T compare, string paramName = "", string msg = null) where T : IComparable` | 校验小于 `compare` |
| `T AssertEQGreater<T>(T data, T compare, string paramName = "", string msg = null) where T : IComparable` | 校验 ≥ `compare` |
| `T AssertEQLesser<T>(T data, T compare, string paramName = "", string msg = null) where T : IComparable` | 校验 ≤ `compare` |
| `T AssertRange<T>(T checkCount, T min, T max, string paramName = "", string msg = null) where T : IComparable` | 校验落在 `[min, max]` |
| `T AssertNotNull<T>(T arg, string paramName = "", string msg = null)` | 校验非 null |
| `IEnumerable<T> AssertNotEmpty<T>(IEnumerable<T> args, string paramName = "", string msg = null)` | 校验集合非空 |
| `string AssertNotEmpty(string args, string paramName = "", string msg = null)` | 校验字符串非空 |

### 2. CodeKit — 编码 / 加解密工具包
全部 `public static`。

| 方法签名 | 说明 |
|---|---|
| `string CharArrToString(char[] chars)` | `char[]` → 字符串 |
| `string RemoveUnescape(string str)` | 还原 UTF-8 转义(正则 `Unescape`) |
| `byte[] StringToUTF8(string str)` | 字符串 → UTF-8 字节 |
| `string UTF8ToString(byte[] data)` | UTF-8 字节 → 字符串 |
| `string StringToBase64(string str)` | 字符串 → Base64(先 UTF-8) |
| `string Base64ToString(string base64)` | Base64 → 字符串 |
| `string BytesToBase64(byte[] data)` | 字节 → Base64 |
| `byte[] Base64ToBytes(string base64)` | Base64 → 字节 |
| `byte[] GZipCompress(string str)` | 字符串 GZip 压缩 |
| `byte[] GZipCompress(byte[] bytes)` | 字节 GZip 压缩 |
| `string DecompressToString(byte[] bytes)` | GZip 解压为字符串 |
| `byte[] Decompress(byte[] bytes)` | GZip 解压为字节 |
| `string MD5Hash(string data)` / `string MD5Hash(byte[] data)` | MD5(小写十六进制) |
| `string SHA256Hash(string data)` / `string SHA256Hash(byte[] data)` | SHA-256(小写十六进制) |
| `byte[] AESEncrypt(byte[] source, string key = AESKey, string iv = "", PaddingMode padding = PKCS7, CipherMode mode = CBC)` | AES 加密 |
| `byte[] AESDecrypt(byte[] source, string key = AESKey, string iv = "", PaddingMode padding = PKCS7, CipherMode mode = CBC)` | AES 解密 |

> **AES 说明**(行为以源码为准):`key` 按 UTF-8 取字节后规范化为合法长度 **16/24/32 字节**(对应 AES-128/192/256);默认 key `"2C18B85E167E4023BD3802CD333B57D9"` 为 32 字符 → AES-256。当 `iv` 为空且为 CBC 模式时,自动生成密码学安全的随机 IV 并**前置拼接**到密文头部(解密时自动从头部读取);传入非空 `iv`(或非 CBC 模式)则按原格式、不前置。注意:该默认行为与旧版(固定零 IV、无前置)的密文**不兼容**,旧数据需重新加密。

### 3. CommonKit — 通用帮助类
全部 `public static`。

| 方法签名 | 说明 |
|---|---|
| `T ExecuteWithoutException<T>(Func<T> func, T defaultValue = default)` | 执行 `func`,异常时返回 `defaultValue`(显式吞异常工具) |
| `void ExecuteWithoutException(Action action)` | 执行 `action`,异常忽略 |
| `string CallTrace(int index = 0)` | 获取调用栈信息(可跳过前 `index` 层) |
| `void WriteJSONFile<T>(string path, T content)` | 序列化对象为格式化 JSON 并写文件 |
| `void WriteXMLFile<T>(string path, T content)` | 序列化对象为格式化 XML 并写文件 |
| `T ReadJSONFile<T>(string path)` | 从文件读取 JSON 并反序列化 |
| `T ReadXMLFile<T>(string path)` | 从文件读取 XML 并反序列化 |

### 4. HttpKit — HTTP 请求工具包
全部 `public static`(共享 `HttpClient` 与 `ApplyRequestHeaders` 为私有)。基于进程级共享 `HttpClient`(30s 超时),请求头设在请求消息上(线程安全),`async` 路径使用 `ConfigureAwait(false)` 避免上下文死锁。同步方法为对应 `*Async` 的 `.GetAwaiter().GetResult()` 薄包装。

| 方法签名 | 说明 |
|---|---|
| `string HttpGetString(string url, Dictionary<string,string> head = null)` | GET,返回响应体字符串(同步) |
| `Task<string> HttpGetStringAsync(string url, Dictionary<string,string> head = null)` | GET 异步,返回响应体字符串;非 2xx 抛 `HttpRequestException` |
| `string HttpPostString(string url, string json, Dictionary<string,string> head = null)` | POST(JSON 体),返回响应体字符串(同步) |
| `Task<string> HttpPostStringAsync(string url, string json, Dictionary<string,string> head = null)` | POST 异步,返回响应体字符串;非 2xx 抛 `HttpRequestException` |
| `T HttpGetJson<T>(string url, Dictionary<string,string> head = null)` | GET,响应体反序列化为 `T`(同步) |
| `Task<T> HttpGetJsonAsync<T>(string url, Dictionary<string,string> head = null)` | GET 异步,反序列化为 `T`;非 2xx 抛 `HttpRequestException` |
| `T HttpPostJson<T>(string url, string json, Dictionary<string,string> head = null)` | POST(JSON 体),反序列化为 `T`(同步) |
| `Task<T> HttpPostJsonAsync<T>(string url, string json, Dictionary<string,string> head = null)` | POST 异步,反序列化为 `T`;非 2xx 抛 `HttpRequestException` |
| `byte[] HttpGetBytes(string url, Dictionary<string,string> head = null)` | GET,返回响应体字节(同步) |
| `Task<byte[]> HttpGetBytesAsync(string url, Dictionary<string,string> head = null)` | GET 异步,返回响应体字节;非 2xx 抛 `HttpRequestException` |
| `byte[] HttpPostBytes(string url, string json, Dictionary<string,string> head = null)` | POST(JSON 体),返回响应体字节(同步) |
| `Task<byte[]> HttpPostBytesAsync(string url, string json, Dictionary<string,string> head = null)` | POST 异步,返回响应体字节;非 2xx 抛 `HttpRequestException` |
| `delegate Task<T> HTTPMessageHandle<T>(HttpResponseMessage response)` | 响应处理委托(自定义返回类型) |
| `Task<T> HttpGetAsync<T>(string url, Dictionary<string,string> head, HTTPMessageHandle<T> handle)` | GET 泛型核心,可传入自定义处理委托 |
| `Task<T> HttpPostAsync<T>(string url, string json, Dictionary<string,string> head, HTTPMessageHandle<T> handle)` | POST 泛型核心,可传入自定义处理委托 |
| `Task<byte[]> HTTPDelegateReadBytesEnsure200(HttpResponseMessage)` | 内置委托:读字节并 `EnsureSuccessStatusCode` |
| `Task<string> HTTPDelegateReadStringEnsure200(HttpResponseMessage)` | 内置委托:读字符串并 `EnsureSuccessStatusCode` |
| `Task<T> HTTPDelegateReadJsonEnsure200<T>(HttpResponseMessage)` | 内置委托:读字符串并反序列化 JSON |

> 所有 `*Async` 方法在非 2xx 状态码时抛 `HttpRequestException`;连接失败同样抛 `HttpRequestException`。

### 5. SerializeKit — 序列化 / 反序列化工具包

```net6.0后json序列化采用System.Text.Json.JsonSerializer```

| 方法签名 | 说明 |
|---|---|
| `string GetJSON<T>(T obj)` | Newtonsoft JSON 序列化(非格式化) |
| `string GetXML<T>(T obj)` | `XmlSerializer` 序列化 |
| `string GetFormatXML<T>(T obj, int spaceCount = 4)` | 格式化 XML |
| `string GetFormatJSON<T>(T obj, int spaceCount = 4)` | 格式化 JSON(`null` 时返回 `string.Empty`) |
| `T DeserializeXML<T>(string xml)` | XML → 强模型 |
| `T DeserializeJSON<T>(string json)` | JSON → 强模型 |
| `object DeserializeXML(string xml, Type type)` | XML → 指定类型 |
| `object DeserializeJSON(string json, Type type)` | JSON → 指定类型 |

### 6. CacheKit — 简单缓存工具包(基于 `MemoryCache`)
全部 `public static`。

| 方法签名 | 说明 |
|---|---|
| `void SetDefaultCacheItemPolicy(CacheItemPolicy item)` | 设置默认滑动过期策略(默认 10 分钟) |
| `T GetOrAdd<T>(string key, Func<T> data, CacheItemPolicy policy = null)` | 获取或添加缓存 |
| `void Clear()` | 清空全部缓存 |
| `bool Add(string key, object obj, CacheItemPolicy policy = null)` | 添加缓存 |
| `object Remove(string key)` | 移除缓存 |

### 7. RegexKit — 常用正则包

| 成员 | 说明 |
|---|---|
| `Regex VarialbeNamedRegeix { get; }` | 变量命名规范正则(`^[a-zA-Z_][a-zA-Z_0-9]*$`) |
| `bool CheckVariableNameSpecification(string variableName)` | 校验字符串是否符合变量命名规范 |

### 8. ConsoleKit — 控制台工具包

| 方法签名 | 说明 |
|---|---|
| `string WaitInput(object input, ConsoleColor noticColor = Cyan, ConsoleColor errorColor = Red, bool withTime = true)` | 输出提示并等待非空输入 |
| `void WaitConfirmInput(object input, ConsoleColor noticColor = Cyan, ConsoleColor errorColor = Red, bool withTime = true)` | 等待用户输入指定内容确认 |
| `void WriteConsole(object msg, ConsoleColor color = Red, bool withTime = true)` | 带颜色/时间戳输出 |
| `T ParseArgs<T>(string[] args, string helpContent = null, bool checkRequire = true)` | 解析命令行参数到强对象(基于 `CommandPropertyAttribute`) |

### 9. ExpressionKit — 表达式树工具包(替代反射,提升性能)
返回 `ExpressionGetSetModel<,>`(含编译后的 Get/Set 委托),带缓存版本经 `CacheKit` 缓存。

| 方法签名 | 说明 |
|---|---|
| `ExpressionGetSetModel<InstanceType, FieldType> CreateFieldExpressWithoutCache<InstanceType, FieldType>(Type type, string fieldName)` | 生成字段访问器(无缓存) |
| `ExpressionGetSetModel<InstanceType, FieldType> CreateFieldExpressWithoutCache<InstanceType, FieldType>(InstanceType instance, string fieldName)` | 同上(以实例推断类型) |
| `ExpressionGetSetModel<InstanceType, FieldType> CreateFieldExpress<InstanceType, FieldType>(Type type, string fieldName)` | 生成字段访问器(带缓存) |
| `ExpressionGetSetModel<InstanceType, FieldType> CreateFieldExpress<InstanceType, FieldType>(InstanceType instance, string fieldName)` | 同上(带缓存) |
| `ExpressionGetSetModel<InstanceType, PropertyType> CreatePropertyExpressWithoutCache<InstanceType, PropertyType>(Type type, string propertyName)` | 生成属性访问器(无缓存) |
| `ExpressionGetSetModel<InstanceType, PropertyType> CreatePropertyExpressWithoutCache<InstanceType, PropertyType>(InstanceType instance, string propertyName)` | 同上(以实例推断) |
| `ExpressionGetSetModel<InstanceType, PropertyType> CreatePropertyExpress<InstanceType, PropertyType>(Type type, string propertyName)` | 生成属性访问器(带缓存) |
| `ExpressionGetSetModel<InstanceType, PropertyType> CreatePropertyExpress<InstanceType, PropertyType>(InstanceType instance, string propertyName)` | 同上(带缓存) |

### 10. InvokeKit — 反射调用工具包
全部 `public static`。

| 方法签名 | 说明 |
|---|---|
| `object GetStaticProperty(Type type, string propertyName)` | 获取静态属性 |
| `void SetStaticProperty(Type type, string propertyName, object data)` | 设置静态属性 |
| `object GetStaticField(Type type, string fieldName)` | 获取静态字段 |
| `void SetStaticField(Type type, string fieldName, object data)` | 设置静态字段 |
| `object GetProperty<T>(T instance, string propertyName)` | 获取实例属性 |
| `void SetProperty<T>(T instance, string propertyName, object data)` | 设置实例属性 |
| `object GetField<T>(T instance, string fieldName)` | 获取实例字段 |
| `void SetField<T>(T instance, string fieldName, object data)` | 设置实例字段 |
| `object CallStaticMethod(string typeFullName, string funcName, params object[] p)` | 按全名调用静态方法 |
| `object CallStaticMethod(Type type, string funcName, params object[] p)` | 按类型调用静态方法 |
| `object CallMethod(object instance, string funcName, params object[] p)` | 调用实例方法 |
| `object CallGenericStaticMethod(string typeFullName, string funcName, List<Type> genericTypeList, params object[] p)` | 调用泛型静态方法 |
| `object CallGenericStaticMethod(Type type, string funcName, List<Type> genericTypeList, params object[] p)` | 同上(按类型) |
| `object CallGenericMethod(object instance, string funcName, List<Type> genericTypeList, params object[] p)` | 调用泛型实例方法 |

### 11. ReflectKit — 语言特性反射工具包
全部 `public static`(少数 `internal` 辅助,如 `TryGetType`/`GetConvertDataHandle` 为内部转换用)。

| 方法签名 | 说明 |
|---|---|
| `Type GetTypeByFullName(string typeName, bool ignoreCase = true)` | 按 FullName 在已加载程序集中查找类型 |
| `object GetDefaultValue(Type type)` | 获取类型默认值 |
| `List<KVModel<Type, TAttribute>> GetMarkAttrAndInhertClassTypes<TParentClass, TAttribute>(bool mustCouldInsntace, params Assembly[] assemblies)` | 查找被 `TAttribute` 标记且继承 `TParentClass` 的类型 |
| `T CreateInstanceWithDefaultValue<T>()` | 构造实例(构造函数参数用默认值) |
| `object CreateInstanceWithDefaultValue(Type type)` | 同上(按 Type) |
| `T CreateInstance<T>(params object[] buildConstructArgs)` | 构造实例 |
| `object CreateInstance(Type type, params object[] buildConstructArgsAction)` | 同上(按 Type) |
| `bool IsInhert<CType, ParentType>() where CType : class where ParentType : class` | 派生判断 |
| `bool IsInhert<T>(Type type) where T : class` | 派生判断 |
| `bool IsInhert(Type type, Type parentType)` | 派生判断 |
| `GetAssembliesResult GetAssemblies()` | 加载并收集引用程序集 |
| `List<Type> GetTypesByAssemble(params Assembly[] assemblies)` | 收集程序集内所有 Type |
| `bool IsList(Type type)` | 是否 `IList` |
| `bool IsEnumerable(Type type)` | 是否 `IEnumerable` |

### 12. RandomKit — 随机数据构造工具包
线程安全(共享随机源:`net8` 用 `Random.Shared`,其余用 `[ThreadStatic]`)。

**数值 / 布尔**

| 方法签名 | 说明 |
|---|---|
| `int NextInt(int min = 0, int max = int.MaxValue)` | 随机整数 `[min, max)` |
| `long NextLong(long min = 0, long max = long.MaxValue)` | 随机长整数 |
| `double NextDouble(double min = 0.0, double max = 1.0, int? decimalPlaces = null)` | 随机双精度 |
| `float NextFloat(float min = 0f, float max = 1f, int? decimalPlaces = null)` | 随机单精度 |
| `decimal NextDecimal(decimal min = 0m, decimal max = 1m, int? decimalPlaces = null)` | 随机 decimal |
| `bool NextBool(double trueProbability = 0.5)` | 随机布尔(可设 true 概率) |

**字符 / 字符串**

| 方法签名 | 说明 |
|---|---|
| `char NextChar(string pool = ConstCharPools.AlphaNumeric)` | 字符池随机取一字符 |
| `string NextString(int length, string pool = ConstCharPools.AlphaNumeric)` | 定长随机串 |
| `string NextString(int minLength, int maxLength, string pool = ConstCharPools.AlphaNumeric)` | 区间长度随机串 |
| `string NextLargeString(int length, string pool = ConstCharPools.AlphaNumeric)` | 高效构造超长随机串 |
| `string NextStringByPattern(string pattern)` | 按模式生成(`L`大写/`l`小写/`d`数字/`s`符号) |
| `T NextEnum<T>() where T : struct` | 随机枚举值 |
| `T NextDefinedEnum<T>() where T : struct` | 随机已定义枚举值 |

**Guid / 时间**

| 方法签名 | 说明 |
|---|---|
| `Guid NextGuid()` | 随机 Guid |
| `string NextGuidString(string format = "D")` | 指定格式 Guid 串 |
| `DateTime NextDateTime(DateTime? min = null, DateTime? max = null)` | 随机时间 |
| `DateTimeOffset NextDateTimeOffset(DateTimeOffset? min = null, DateTimeOffset? max = null)` | 随机 DateTimeOffset |
| `TimeSpan NextTimeSpan(TimeSpan? min = null, TimeSpan? max = null)` | 随机 TimeSpan |

**集合**

| 方法签名 | 说明 |
|---|---|
| `T NextItem<T>(T[] array)` / `T NextItem<T>(IList<T> list)` / `T NextItem<T>(IEnumerable<T> source)` | 随机取一个元素 |
| `List<T> NextSample<T>(IList<T> source, int count)` | 不重复采样(部分 Fisher-Yates) |
| `void ShuffleInPlace<T>(IList<T> list)` | 原地洗牌 |
| `List<T> Shuffle<T>(IEnumerable<T> source)` | 洗牌(返回新集合) |
| `T NextWeightedItem<T>(IList<T> items, Func<T, double> weightSelector)` | 按权重选取 |
| `List<T> NextWeightedSample<T>(IList<T> items, Func<T, double> weightSelector, int count)` | 加权不重复采样(A-ES 算法) |
| `double[] NextStratifiedSample(double min, double max, int strata)` | 分层采样 |

**颜色 / 图像**

| 方法签名 | 说明 |
|---|---|
| `Color NextColor(bool withAlpha = true)` | 随机颜色(含透明) |
| `Color NextSolidColor()` | 随机纯色(不透明) |
| `byte[] NextImage(int width, int height, ImageFormat format, int elementCount = 10, Color? backgroundColor = null)` | 随机验证码风格图片字节(**仅 net45/451/452**;net8 不可用) |

**模型构造**

| 方法签名 | 说明 |
|---|---|
| `T NextModel<T>()` / `object NextModel(Type type)` | 反射构造完全随机对象 |
| `T NextModel<T>(RandomGenerateContext context)` / `object NextModel(Type type, RandomGenerateContext context)` | 使用自定义上下文构造 |
| `RandomModelBuilder<T> BuildModel<T>()` | 进入声明式构建器 |

**工具 / 概率分布 / 国际化 / 数据模拟**

| 方法签名 | 说明 |
|---|---|
| `bool Roll(double probability)` | 按概率返回 true(= `NextBool`) |
| `T Pick<T>(params T[] items)` | 等概率随机取一 |
| `byte[] NextBytes(int length)` / `void FillBytes(byte[] buffer)` | 随机字节 |
| `double NextGaussian(double mean = 0.0, double stdDev = 1.0)` | 正态分布(Box-Muller) |
| `double NextExponential(double rate = 1.0)` | 指数分布 |
| `int NextPoisson(double lambda)` | 泊松分布(Knuth) |
| `char NextChineseChar()` / `string NextChineseText(int length)` | 随机中文 |
| `char NextHiraganaChar()` / `char NextKatakanaChar()` / `char NextKoreanChar()` | 随机日文/韩文 |
| `string NextEmoji()` | 随机 Emoji |
| `string NextPhoneNumberCN()` / `string NextEmail()` / `string NextIPv4()` / `string NextUrl()` | 模拟数据 |
| `string NextChineseName()` / `string NextChineseIdCard()` | 模拟中文姓名/身份证(含校验位) |
| `string NextLoremIpsum(int wordCount)` | 占位文本 |
| `IEnumerable<T> NextSequence<T>(Func<T> generator)` | 惰性无限序列 |
| `IEnumerable<int> NextInts(int count, ...)` / `IEnumerable<double> NextDoubles(int count, ...)` / `IEnumerable<string> NextStrings(int count, int length, ...)` | 序列生成 |
| `T NextFlags<T>(int? maxFlags = null) where T : struct` | 随机 `[Flags]` 组合 |
| `List<T> NextModels<T>(int count)` / `List<object> NextModels(Type type, int count)` | 批量构造 |

### 13. AOPKit — AOP 面向切面编程工具包
基于动态代理 + 拦截器(`.NET 8` 用 `DispatchProxy`,其余用 `RealProxy`)。

| 方法签名 | 说明 |
|---|---|
| `TInterface CreateProxy<TInterface>(TInterface target, List<IAOPInterceptor> interceptors = null, Func<MethodInfo, bool> methodFilter = null) where TInterface : class` | 为接口创建代理 |
| `TInterface CreateProxyByAttribute<TInterface>(TInterface target) where TInterface : class` | 按 `AOPAttribute` 自动扫描拦截器创建代理 |
| `TInterface CreateProxy<TInterface>(AOPProxyOptions options) where TInterface : class` | 用完整选项创建代理 |
| `List<IAOPInterceptor> GetInterceptorsFromAttribute(Type type)` | 从类型 `AOPAttribute` 提取拦截器(按 `Order` 排序) |
| `List<IAOPInterceptor> GetInterceptorsFromAttribute<T>()` | 泛型版 |
| `void ExecuteWithAOP(Action action, params IAOPInterceptor[] interceptors)` | 环绕切面执行(无返回值) |
| `T ExecuteWithAOP<T>(Func<T> func, params IAOPInterceptor[] interceptors)` | 环绕切面执行(带返回值) |

### 14. SpreadsheetKit — 电子表格工具包
基于 `SpreadSheetProxy` 策略工厂,支持 CSV / Excel / 文本表格读写。返回值类型由 `configArgs` 决定(CSV/文本 → `string`,Excel → `byte[]`)。

**写入**

| 方法签名 | 说明 |
|---|---|
| `object WriteSpreadSheet<TKey, TValue>(WriteConfigSpreadSheetArgs configArgs, IEnumerable<Dictionary<TKey, TValue>> data)` | 字典集合写入 |
| `object WriteSpreadSheet<TData>(WriteConfigSpreadSheetArgs configArgs, IEnumerable<TData> data)` | 强模型集合写入 |
| `object WriteSpreadSheet(WriteConfigSpreadSheetArgs configArgs, DataTable data)` | DataTable 写入 |
| `WriteSpreadSheetHandle WriteSpreadSheetHandle<TKey, TValue>(WriteConfigSpreadSheetArgs configArgs, IEnumerable<Dictionary<TKey, TValue>> data, WriteSpreadSheetHandle handle = null)` | 写入到句柄(可多次追加) |
| `WriteSpreadSheetHandle WriteSpreadSheetHandle<TData>(WriteConfigSpreadSheetArgs configArgs, IEnumerable<TData> data, WriteSpreadSheetHandle handle = null)` | 同上(强模型) |
| `WriteSpreadSheetHandle WriteSpreadSheetHandle(WriteConfigSpreadSheetArgs configArgs, DataTable data, WriteSpreadSheetHandle handle = null)` | 同上(DataTable) |

**读取**

| 方法签名 | 说明 |
|---|---|
| `List<TData> ReadModel<TData>(ReadSpreadSheetArgs args)` | 读取到强模型 |
| `List<dynamic> ReadDynamic(ReadSpreadSheetArgs args)` | 读取到动态(属性全为 string) |
| `List<dynamic> ReadDynamicAutoType(ReadSpreadSheetArgs args)` | 读取到动态(属性按列推断类型) |
| `List<Dictionary<string, string>> ReadDict(ReadSpreadSheetArgs args)` | 读取到字典(string) |
| `List<Dictionary<string, dynamic>> ReadDictAutoType(ReadSpreadSheetArgs args)` | 读取到字典(推断类型) |

### 15. FileKit(`Kit.File`)— 文件处理工具包
全部 `public static`,ZIP 基于 `SharpZipLib`。

| 方法签名 | 说明 |
|---|---|
| `string FilterInvalidPathChars(string path)` | 过滤非法路径字符 |
| `string FilterInvalidFileNameChars(string name)` | 过滤非法文件名字符 |
| `byte[] PackToZip(IEnumerable<ZipEntry> entries, int compressLevel = 0)` | 扁平条目打包 ZIP |
| `byte[] PackToZip(ZipItem root, int compressLevel = 0)` | 树形结构打包 ZIP(以 root.Name 为前缀) |
| `byte[] PackToZip(List<ZipItem> roots, int compressLevel = 0)` | 多根打包 ZIP(无额外包装目录) |
| `List<ZipEntry> Unzip(byte[] zipData)` | 解压 ZIP 为扁平条目 |
| `ZipItem UnzipToTree(byte[] zipData)` | 解压为树(含虚拟根) |
| `List<ZipItem> UnzipToTreeList(byte[] zipData)` | 解压为顶层条目列表 |

### 16. BigFile(`Kit.File`)— 大文件读取
`public class`(实例)。

| 成员 | 说明 |
|---|---|
| `BigFile(string fileName)` / `BigFile(Stream stream)` | 构造(只读流) |
| `int ReadOffset(ref byte[] target, long offsetIndex, int lenght)` | 按偏移读取(填充 `target`,返回实际读取量) |
| `byte[] ReadOffset(long offsetIndex, int lenght, out int size)` | 按偏移读取(返回字节与读取量) |


## 二、`Extension` 命名空间(便捷扩展方法)

### EasyCodeKit(`CodeExtension`)
对 `string`/`byte[]`/`char[]` 的扩展,内部转发 `CodeKit`。

| 方法签名 | 说明 |
|---|---|
| `string CharsToString(this char[] content)` | = `CodeKit.CharArrToString` |
| `string RemoveUnescape(this string content)` | = `CodeKit.RemoveUnescape` |
| `byte[] ToUTF8(this string args)` | = `CodeKit.StringToUTF8` |
| `string UTF8ToString(this byte[] args)` | = `CodeKit.UTF8ToString` |
| `string ToBase64(this string args)` | = `CodeKit.StringToBase64` |
| `string ToBase64(this byte[] args)` | = `CodeKit.BytesToBase64` |
| `string Bas64ToString(this string base64)` | = `CodeKit.Base64ToString` |
| `byte[] Base64ToBytes(this string base64)` | = `CodeKit.Base64ToBytes` |
| `string SHA256(this string input)` / `string SHA256(this byte[] input)` | = `CodeKit.SHA256Hash` |
| `string MD5(this string input)` / `string MD5(this byte[] input)` | = `CodeKit.MD5Hash` |
| `byte[] AESEncrypt(this byte[] input)` / `byte[] AESDecrypt(this byte[] input)` | = `CodeKit.AESEncrypt/Decrypt`(使用默认 key/iv) |

### EasyCommonKit(`CommonExtension`)
| 方法签名 | 说明 |
|---|---|
| `Stream ToStream(this byte[] buffer)` | 字节 → `MemoryStream` |
| `void Partition<T>(this IEnumerable<T> source, int capacity, Action<IEnumerable<T>> action)` | 按容量分块并执行 |
| `bool NotEmpty<T>(this IEnumerable<T> source)` | 是否非空 |
| `bool IsEmpty<T>(this IEnumerable<T> source)` | 是否为空 |
| `HashSet<TSource> ToHashSet<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource> comparer = null)` | 转 HashSet |

### EasySerializeKit(`SerializeExtension`)
| 方法签名 | 说明 |
|---|---|
| `string GetFormatJSON<T>(this T args)` | 格式化 JSON |
| `string GetFormatXML<T>(this T args)` | 格式化 XML |
| `T DesializeXML<T>(this string args)` | XML 反序列化 |
| `T DesializeJSON<T>(this string args)` | JSON 反序列化 |

### EasyInvokeKit(`InvokeExtension`)
对 `T`/`Type` 的反射扩展,转发 `InvokeKit`(`GetProperty`/`SetProperty`/`GetField`/`SetField`/`CallMethod`/`GetStaticProperty`/`SetStaticProperty`/`GetStaticField`/`SetStaticField`/`CallStaticMethod`)。

### EasyCollectionsKit(`CollectionsExtension`)
| 方法签名 | 说明 |
|---|---|
| `Dictionary<TKey, TValue> MergeDictionary<TKey, TValue>(this Dictionary<TKey, TValue> src, Dictionary<TKey, TValue> r, Func<TValue, TValue, TValue> conflictSelector = null)` | 合并字典(默认保留来源值) |

### EasyFileKit(`FileExtension`)
| 方法签名 | 说明 |
|---|---|
| `byte[] PackToZip(this Dictionary<string, byte[]> fileData, int compareLevel = 0)` | 字典打包 ZIP(字节) |
| `byte[] PackToZip(this Dictionary<string, string> fileData, int compareLevel = 0)` | 字典打包 ZIP(字符串) |
| `List<KVModel<string, byte[]>> UnPackZipToBytes(this byte[] zipFile)` | 解压为 路径→字节 |
| `List<KVModel<string, string>> UnPackZipToString(this byte[] zipFile)` | 解压为 路径→字符串 |

### EasySpreadsheetKit(`SpreadsheetExtension`)
对 `IEnumerable<T>`/`DataTable`/`WriteSpreadSheetHandle` 的扩展,转发 `SpreadsheetKit`。核心方法:
- `WriteSpreadSheetHandle WriteSpreadSheet<T>(this WriteSpreadSheetHandle handle, WriteConfigSpreadSheetArgs args, ...)` — 追加写入
- `string ToFormatStringTable<T>(...)` / `string ToCsv<T>(...)` / `byte[] ToExcel<T>(...)` — 转文本表/CSV/Excel
- `EasyModelToExcelWithConfig<TData> ToExcelWithConfig<TData>(...)` / `EasyDicToExcelWithConfig<,> ToExcelWithConfig<,>(...)` / `EasyDataTableToExcelWithConfig ToExcelWithConfig(this DataTable, ...)` — 带配置导出
- `List<dynamic> ReadExcelDynamicAutoType(this byte[] excelData, bool withHead = true)` / `ReadCsvDynamicAutoType(this string csvData, ...)` / `ReadExcelDynamic(...)` / `ReadCsvDynamic(...)` — 读取到动态
- `List<Dictionary<string, string>> ReadExcelDict(...)` / `ReadCsvDict(...)` / `List<Dictionary<string, dynamic>> ReadExcelDictAutoType(...)` / `ReadCsvDictAutoType(...)` — 读取到字典
- `List<TData> ReadExcelMapModel<TData>(this byte[] excelData, bool withHead = true)` / `ReadCsvMapModel<TData>(this string csvData, ...)` — 读取到强模型

---

## 三、`Package` 命名空间(策略 / 代理框架,扩展点)

### RandSeedProxy(`RandomPackage`)— 随机种子策略工厂
单例。通过 `[RandomSeedRegister]` 特性自动发现策略(`internal` 实现类),按「精确类型 → 继承链 → 接口 → 泛型定义 → 兜底」顺序匹配。

| 成员 | 说明 |
|---|---|
| `static RandSeedProxy Instance` | 单例 |
| `BaseRandSeed GetPolicy(Type type)` | 获取类型的随机种子策略(O(1) 带缓存) |
| `void Register(Type typeDefine, BaseRandSeed seed)` | 注册自定义策略(线程安全) |
| `RandomGenerateContext CreateContext(RandomContext randomContext = null, RandomConfigAttribute[] configAttrs = null)` | 创建生成上下文 |

### PolicyPool / SpreadSheetProxy(`Common` / `SpreadSheetPackage`)
通用策略工厂基类。`SpreadSheetProxy` 继承 `PolicyPool<SpreadSheetProxy, SpreadSheetRegisterAttribute, SpreadSheetPolicy, int, int>`。

| 成员 | 说明 |
|---|---|
| `static TPolicy Instance` | 单例 |
| `TInterface GetPolicy(TPolicyType key)` | 按 key 获取策略 |
| `void ReplacePolicy(TPoolKey policyKey, TInterface policy)` | 替换/新增策略实现 |

### BaseRandSeed(`RandomPackage`)— 随机策略基类(扩展点)
`internal abstract class`,子类通过 `[RandomSeedRegister(typeof(...))]` 注册。核心抽象方法:
- `object Generate(Type type, RandomGenerateContext context)` — 生成指定类型的随机值。

内置实现(`internal`,自动发现):`SimpleType`(Bool/DateTime/Decimal/Enum/Float/Guid/Integer/Nullable/String)、`ComplexType`(Default/Dictionary/List/Set/Tuple)。

### RandomGenerateContext(`RandomPackage`)— 生成上下文
| 成员 | 说明 |
|---|---|
| `RandomContext RandomContext { get; }` | 用户上下文 |
| `BaseRandSeed GetPolicy(Type type)` | 取策略 |
| `bool TryEnter(string key)` | 进入属性生成(死循环保护) |
| `T GetConfig<T>() where T : RandomConfigAttribute` | 提取配置标记 |
| `RandomConfigAttribute[] ConfigAttrs { get; }` | 全部配置标记 |

### RandomModelBuilder`<T>`(`RandomPackage.Builder`)— 声明式构建器
| 成员 | 说明 |
|---|---|
| `RandomModelBuilder<T> With<TProp>(Expression<Func<T, TProp>> selector, TProp value)` | 覆盖属性值 |
| `T Build()` | 生成随机对象并应用覆盖 |
| `List<T> BuildMany(int count)` | 批量构建(应用相同覆盖) |

### 电子表格策略(`SpreadSheetPackage`)
- `abstract class SpreadSheetPolicy`(及 `ISpreadSheetPolicy`):策略基类/接口,实现 `WriteSpreadSheet` / `ReadModel<T>` / `ReadDynamic` / `ReadDict` 等。
- 内置 `internal` 实现:`CsvSpreadSheetPolicy` / `ExcelSpreadSheetPolicy` / `FormatStringSpreadSheetPolicy`(及注释禁用的 `Sql`/`String` 策略)。
- 新增策略:继承 `SpreadSheetPolicy` 并标记 `[SpreadSheetRegister(TargetType)]`,由工厂自动加载;或用 `SpreadSheetProxy.Instance.ReplacePolicy(...)` 替换。

### AOP 扩展点(`AOPPackage` / `Model.AOP`)
- `interface IAOPInterceptor`:`Order` 属性 + `OnBefore` / `OnAfter` / `OnReturn` / `OnException`(接收 `AOPInterceptContext`)。
- `abstract class AOPInterceptorBase`:拦截器基类。
- 特性:`[AOPAttribute(InterceptorType)]`(类级)、`[AOPMethodAttribute(Enable)]`(方法级,可开关)、`[AOPIgnoreAttribute]`(排除拦截)。

### 注册特性
- `[RandomSeedRegister(typeof(T))]`(`Model.Random.Attributes`):注册随机策略。
- `[SpreadSheetRegister(TargetType)]`(`Model.SpreadSheet.Attributes`):注册电子表格策略。
- `[CommandPropertyAttribute]`(`Model.Console`):`ConsoleKit.ParseArgs<T>` 映射命令行参数。
- `[RandomConfigAttribute]`(`Model.Random.Attributes`):随机生成配置。

---

## 四、使用注意 / 已知事项

1. **AES(已修复)**:`key` 现为 16/24/32 字节规范化(默认 32 字符 key → AES-256),不再截断为 16 字节;`iv` 为空(CBC)时生成随机 IV 并前置。⚠️ 旧版"零 IV、无前置"的密文与新版不兼容,需用新逻辑重新加密。
2. **net8.0** 下 `RandomKit.NextImage` 等依赖 `System.Drawing.Common` 的方法不可用(条件编译排除)。
3. `SerializeKit.GetJSON` 使用 Newtonsoft 默认设置(保留 null);`GetFormatJSON` 对 `null` 返回 `string.Empty`。
4. `CommonKit.ExecuteWithoutException` 为显式吞异常工具,调用方需自行确认适用场景。
