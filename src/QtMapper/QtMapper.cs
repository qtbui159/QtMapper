using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Qt
{
    public class QtMapper : IMapper
    {
        private Dictionary<Type, HashSet<MapperEntity>> m_DstTypeMapEntity = new Dictionary<Type, HashSet<MapperEntity>>();

        /// <summary>
        /// 默认规则绑定，类型和名字相同的属性自动绑定。如果为迭代类型，
        /// 可绑定List,Array转换；超出该范围请自行用自定义规则绑定
        /// </summary>
        /// <typeparam name="TDst"></typeparam>
        /// <typeparam name="TSrc"></typeparam>
        /// <returns></returns>
        public IMapper Bind<TDst, TSrc>()
        {
            Type dstType = typeof(TDst);
            Type srcType = typeof(TSrc);

            MapperEntity<TDst, TSrc> mapperEntity = new MapperEntity<TDst, TSrc>(srcType, null, null);

            lock (m_DstTypeMapEntity)
            {
                if (GetSrcToDst(dstType,srcType) != null)
                {
                    throw new Exception($"已存在{srcType.Name}到{dstType.Name}的绑定关系");
                }

                if (m_DstTypeMapEntity.ContainsKey(dstType))
                {
                    m_DstTypeMapEntity[dstType].Add(mapperEntity);
                }
                else
                {
                    m_DstTypeMapEntity.Add(dstType, new HashSet<MapperEntity>() { mapperEntity });
                }
            }

            return this;
        }

        /// <summary>
        /// 自定义规则绑定
        /// </summary>
        /// <typeparam name="TDst"></typeparam>
        /// <typeparam name="TSrc"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public IMapper Bind<TDst, TSrc>(Func<TSrc, TDst> func)
        {
            Type dstType = typeof(TDst);
            Type srcType = typeof(TSrc);

            MapperEntity<TDst, TSrc> mapperEntity = new MapperEntity<TDst, TSrc>(srcType, func, null);

            lock (m_DstTypeMapEntity)
            {
                if (GetSrcToDst(dstType, srcType) != null)
                {
                    throw new Exception($"已存在{srcType.Name}到{dstType.Name}的绑定关系");
                }

                if (m_DstTypeMapEntity.ContainsKey(dstType))
                {
                    m_DstTypeMapEntity[dstType].Add(mapperEntity);
                }
                else
                {
                    m_DstTypeMapEntity.Add(dstType, new HashSet<MapperEntity>() { mapperEntity });
                }
            }

            return this;
        }

        /// <summary>
        /// 额外绑定，可以将默认规则以外的量进行绑定
        /// </summary>
        /// <typeparam name="TDst"></typeparam>
        /// <typeparam name="TSrc"></typeparam>
        /// <param name="bindExpression"></param>
        public void BindExtra<TDst, TSrc>(Func<TDst, TSrc, TDst> bindExpression)
        {
            Type dstType = typeof(TDst);
            Type srcType = typeof(TSrc);

            if (bindExpression == null)
            {
                throw new ArgumentNullException(nameof(bindExpression));
            }

            lock (m_DstTypeMapEntity)
            {
                MapperEntity<TDst, TSrc> mapperEntity = GetSrcToDst<TDst,TSrc>();
                if (mapperEntity == null)
                {
                    throw new Exception($"不存在{srcType.Name}到{dstType.Name}的绑定关系");
                }

                mapperEntity.BindStep2 = bindExpression;
            }
        }
        
        /// <summary>
        /// 寻找Src到Dst的MapperEntity
        /// </summary>
        /// <typeparam name="TDst"></typeparam>
        /// <typeparam name="TSrc"></typeparam>
        /// <returns></returns>
        private MapperEntity<TDst, TSrc> GetSrcToDst<TDst,TSrc>()
        {
            Type dstType = typeof(TDst);
            Type srcType = typeof(TSrc);
            if (!m_DstTypeMapEntity.ContainsKey(dstType))
            {
                return null;
            }
            MapperEntity<TDst, TSrc> mapperEntity = m_DstTypeMapEntity[dstType].FirstOrDefault(x =>
            {
                return x.TypeEquals(srcType);
            }) as MapperEntity<TDst, TSrc>;

            return mapperEntity;
        }

        /// <summary>
        /// 寻找Src到Dst的MapperEntity
        /// </summary>
        /// <param name="dstType"></param>
        /// <param name="srcType"></param>
        /// <returns></returns>
        private MapperEntity GetSrcToDst(Type dstType, Type srcType)
        {
            if (!m_DstTypeMapEntity.ContainsKey(dstType))
            {
                return null;
            }
            MapperEntity mapperEntity = m_DstTypeMapEntity[dstType].FirstOrDefault(x =>
            {
                return x.TypeEquals(srcType);
            });

            return mapperEntity;
        }

        /// <summary>
        /// 取转换结果
        /// </summary>
        /// <typeparam name="TDst"></typeparam>
        /// <typeparam name="TSrc"></typeparam>
        /// <param name="src"></param>
        /// <returns></returns>
        public TDst Get<TDst, TSrc>(TSrc src)
        {
            return Get<TDst>(src);
        }

        /// <summary>
        /// 取转换结果
        /// </summary>
        /// <typeparam name="TDst"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public TDst Get<TDst>(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            Type dstType = typeof(TDst);
            Type srcType = obj.GetType();

            MapperEntity mapperEntity = GetSrcToDst(dstType, srcType);
            if (mapperEntity == null)
            {
                throw new Exception($"不存在{srcType.Name}到{dstType.Name}的绑定关系");
            }

            if (mapperEntity.IsCustom)
            {
                //如果有自定义转换的
                return (TDst)mapperEntity.Map(obj);
            }

            if (IsArrayOrAssignFromIList(dstType) && IsArrayOrAssignFromIList(srcType))
            {
                //如果是IList或者Array的话，就把内容剥出来
                Type innerDstType = GetInnerTypeFromArrayOrIList(dstType);
                Type innerSrcType = GetInnerTypeFromArrayOrIList(srcType);

                object[] srcObjArray = GetArrayFromEnumrableObject(obj);

                MethodInfo methodInfo = (MethodBase.GetCurrentMethod() as MethodInfo).MakeGenericMethod(new Type[] { innerDstType });

                if (dstType.IsArray)
                {
                    //Array

                    Array dstArray = Array.CreateInstance(innerDstType, srcObjArray.Length);
                    for (int i = 0, count = srcObjArray.Length; i < count; ++i)
                    {
                        //递归转换
                        object mapResult = methodInfo.Invoke(this, new object[] { srcObjArray[i] });
                        dstArray.SetValue(mapResult, i);
                    }
                    return (TDst)(object)dstArray;
                }
                else
                {
                    //IList

                    IList result = (IList)Activator.CreateInstance(dstType);
                    for (int i = 0, count = srcObjArray.Length; i < count; ++i)
                    {
                        //递归转换
                        object mapResult = methodInfo.Invoke(this, new object[] { srcObjArray[i] });
                        result.Add(mapResult);
                    }

                    return (TDst)result;
                }
            }
            else
            {
                //剥到最后了
                return (TDst)mapperEntity.Map(obj);
            }
        }

        private bool IsArrayOrAssignFromIList(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type.IsArray)
            {
                return true;
            }
            return typeof(IList).IsAssignableFrom(type);
        }

        private object[] GetArrayFromEnumrableObject(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            IEnumerator etor = (obj as IEnumerable).GetEnumerator();
            List<object> objList = new List<object>();
            
            while(etor.MoveNext())
            {
                objList.Add(etor.Current);
            }
            return objList.ToArray();
        }

        private Type GetInnerTypeFromArrayOrIList(Type type)
        {
            if (type.IsArray)
            {
                //Array
                return type.GetElementType();
            }
            else
            {
                //IList
                return type.GetGenericArguments()[0];
            }
        }
        
    }
}
