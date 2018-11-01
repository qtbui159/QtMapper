using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Qt
{
    abstract class MapperEntity
    {
        public abstract bool TypeEquals(Type outType);
        public abstract object Map(object src);
        public abstract bool IsCustom { get; }
    }

    class MapperEntity<TDst, TSrc> : MapperEntity
    {
        public Type SrcType { private set; get; }

        public override bool IsCustom
        {
            get => m_BindStep1 != null;
        }

        public Func<TSrc, TDst> BindStep1
        {
            private set => m_BindStep1 = value;
            get
            {
                if (m_BindStep1 == null)
                {
                    return BlankStep1;
                }
                return m_BindStep1;
            }
        }
        private Func<TSrc, TDst> m_BindStep1 = null;

        public Func<TDst, TSrc, TDst> BindStep2
        {
            set => m_BindStep2 = value;
            get
            {
                if (m_BindStep2 == null)
                {
                    return BlankStep2;
                }
                return m_BindStep2;
            }
        }
        private Func<TDst, TSrc, TDst> m_BindStep2 = null;

        private Func<TSrc, TDst> BlankStep1;
        private Func<TDst, TSrc, TDst> BlankStep2;

        private bool m_Reflection = false;
        private Dictionary<FieldInfo, FieldInfo> m_FieldMap = new Dictionary<FieldInfo, FieldInfo>();
        private Dictionary<PropertyInfo, PropertyInfo> m_PropertyMap = new Dictionary<PropertyInfo, PropertyInfo>();

        public MapperEntity(Type srcType, Func<TSrc, TDst> bindStep1, Func<TDst, TSrc, TDst> bindStep2)
        {
            if (srcType == null)
            {
                throw new ArgumentNullException(nameof(srcType));
            }

            BlankStep1 = new Func<TSrc, TDst>(BlankStep1Impl);
            BlankStep2 = new Func<TDst, TSrc, TDst>(BlankStep2Impl);

            SrcType = srcType;
            BindStep1 = bindStep1;
            BindStep2 = bindStep2;
        }

        public override bool TypeEquals(Type outType)
        {
            if (outType == null)
            {
                throw new ArgumentNullException(nameof(outType));
            }

            return SrcType == outType;
        }

        public override int GetHashCode()
        {
            return SrcType.GetHashCode();
        }

        public override object Map(object src)
        {
            TSrc srcObj = (TSrc)src;
            TDst dst = BindStep1(srcObj);
            BindStep2(dst, srcObj);
            return dst;
        }

        private TDst BlankStep1Impl(TSrc src)
        {
            TDst dst = (TDst)Activator.CreateInstance(typeof(TDst));

            if (!m_Reflection)
            {
                FieldInfo[] dstFields = typeof(TDst).GetFields();
                FieldInfo[] srcFields = typeof(TSrc).GetFields();
                PropertyInfo[] dstProperties = typeof(TDst).GetProperties();
                PropertyInfo[] srcProperties = typeof(TSrc).GetProperties();

                foreach (FieldInfo dstField in dstFields)
                {
                    FieldInfo srcField = srcFields.FirstOrDefault(x => x.Name == dstField.Name && x.GetType() == dstField.GetType());
                    if (srcField == default(FieldInfo))
                    {
                        continue;
                    }

                    m_FieldMap.Add(dstField, srcField);
                }
                foreach (PropertyInfo dstProperty in dstProperties)
                {
                    PropertyInfo srcProperty = srcProperties.FirstOrDefault(x => x.Name == dstProperty.Name && x.GetType() == dstProperty.GetType());
                    if (srcProperty == default(PropertyInfo))
                    {
                        continue;
                    }

                    m_PropertyMap.Add(dstProperty, srcProperty);
                }

                m_Reflection = true;
            }

            foreach (var field in m_FieldMap)
            {
                object value = field.Value.GetValue(src);
                field.Key.SetValue(dst, value);
            }
            foreach (var property in m_PropertyMap)
            {
                object value = property.Value.GetValue(src, null);
                property.Key.SetValue(dst, value, null);
            }

            return dst;
        }

        private TDst BlankStep2Impl(TDst dst, TSrc src)
        {
            //什么都不做就行了
            return dst;
        }
    }
}
